using Microsoft.AspNetCore.Mvc;
using wazaware.co.za.Models;
using X.PagedList;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.CodeAnalysis;
using wazaware.co.za.DAL;
using Microsoft.AspNetCore.Identity;
using wazaware.co.za.Services;
using System.Configuration;
using System.Linq;
using static wazaware.co.za.Models.ViewModels.PrimaryModel;

namespace wazaware.co.za.Controllers
{
	public class ShopController : Controller
	{
		private readonly ILogger<ShopController> _logger;
		private readonly wazaware_db_context _context;
		private readonly IHttpContextAccessor _httpContextAccessor;
		//private static string userEmail;
		//private static string userFirstName;
		private static int basketCounter;
		private static UserModel? _user;
		private static Boolean _isCookieUser;	

		public ShopController(ILogger<ShopController> logger, wazaware_db_context context, IHttpContextAccessor httpContextAccessor)
		{
			_logger = logger;
			_context = context;
			_httpContextAccessor = httpContextAccessor;
			ViewBag.isCookie = _isCookieUser;
		}
		/// <summary>
		/// Responsible for the Shop RazorView
		/// ViewBags that need to be Fulfilled:
		/// ViewBag.IsCookie; ViewBag.FirstName; ViewBag.ItemCount
		/// </summary>
		[HttpGet]
		public async Task<IActionResult> Index(string search, int productId, int actionCode)
		{
			var cookieResponse = await SyncUserCookieAsync();
			if (cookieResponse != null)
				await SetUserModel(int.Parse(cookieResponse)); // Sets User Model so that {_user} can be called
			/// Update ViewBags Appropriately ///				
			// Check if user is a Cookie or Registered
			if (!_user!.Email!.Contains("wazawareCookie6.542-Email"))
				_isCookieUser = false;
			else
				_isCookieUser = true;
			ViewBag.isCookie = _isCookieUser;
			if (!_isCookieUser)
				ViewBag.FirstName = _user.FirstName;
			// Search For Products
			if (!string.IsNullOrEmpty(search))
				return RedirectToAction("Products", new { search });
			// Dynamic Functions
			if(productId != 0 && actionCode != 0)
			{
				switch(actionCode)
				{
					case 1:
						LoadShoppingCart(_user.UserId);
						break;
					// Add Product to Shopping Cart
					case 5:
						await AddToCart(productId);
						break;
					// Remove from Shopping Cart
					case 10:
						await RemoveFromCart(productId);
						break;
					default:
						Console.WriteLine("Thats Not A ActionCode!");
						break;
				}
			}
			while (_user != null)
			{
				// Load Shopping Cart
				var shoppingCart = LoadShoppingCart(_user.UserId);

				// Checks for products in Shopping Cart
				if (shoppingCart == null)
				{
					ViewBag.ItemCount = 0;
					List<Product> shoppingCartEmpty = new()
					{
						new Product
						{
							ProductName = "non",
							ProductStock = "non",
							ProductDescription = "non",
							ProductCategory = "non",
							ProductPriceBase = 0,
							ProductPriceSale = 0,
							ProductVendorName = "non",
							ProductVendorUrl = "non",
							ProductVisibility = "visible",
							ProductDataBatchNo = "non",
							ProductImageUrl = "non"
						}
					};
				}
				else
					ViewBag.ItemCount = shoppingCart.Count;
				// Load Latest Arrivals
				var latestArrivalProducts = _context.Products
				.Where(p => p.ProductVisibility!.Equals("ProductVisibility") && p.ProductPriceBase < 20000 && p.ProductPriceBase > 10000)
				.OrderByDescending(p => p.ProductPriceBase)
				.Take(8)
				.Select(p => new LatestArrivalModel
				{
					ProductId = p.ProductId,
					ProductName = p.ProductName,
					ProductDescription = p.ProductDescription,
					ProductCategory = p.ProductCategory,
					ProductStock = p.ProductStock,
					ProductPriceBase = p.ProductPriceBase,
					ProductPriceSale = p.ProductPriceSale,
					ProductPic = p.ProductPic
				}).ToPagedList(1, 8);

				// Load Limited Stock
				var limitedStockProducts = _context.Products
					.Where(p => p.ProductVisibility!.Equals("ProductVisibility") && p.ProductPriceBase < 10000 && p.ProductPriceBase > 5000)
					.OrderByDescending(p => p.ProductPriceBase)
					.Take(8)
					.Select(p => new LimitedStockModel
					{
						ProductId = p.ProductId,
						ProductName = p.ProductName,
						ProductDescription = p.ProductDescription,
						ProductCategory = p.ProductCategory,
						ProductStock = p.ProductStock,
						ProductPriceBase = p.ProductPriceBase,
						ProductPriceSale = p.ProductPriceSale,
						ProductPic = p.ProductPic

					}).ToPagedList(1, 8);

				// Load Trending Products
				var trendingProducts = _context.Products
					.Where(p => p.ProductVisibility!.Equals("ProductVisibility") && (p.ProductPriceBase > 20000 &&
					(p.ProductCategory!.Equals("GPUs") ||
					p.ProductCategory!.Equals("Notebooks") ||
					p.ProductCategory.Equals("Monitors"))))
					.OrderByDescending(p => p.ProductPriceBase)
					.Take(8)
					.Select(p => new TrendingProductsModel
					{
						ProductId = p.ProductId,
						ProductName = p.ProductName,
						ProductDescription = p.ProductDescription,
						ProductCategory = p.ProductCategory,
						ProductStock = p.ProductStock,
						ProductPriceBase = p.ProductPriceBase,
						ProductPriceSale = p.ProductPriceSale,
						ProductPic = p.ProductPic

					}).ToPagedList(1, 8);
				
				// View Model to return
				var viewModel1 = new ProductsViewModel
				{
					LatestArrivals = latestArrivalProducts,
					LimitedStock = limitedStockProducts,
					TrendingProducts = trendingProducts,
					Cart = shoppingCart
				};
				return View(viewModel1);
			}
			return View();
		}
		/// <summary>
		/// Responsible for the Cart RazorView
		/// </summary>
		[HttpGet]
		public async Task<IActionResult> Cart()
		{
			var cookieResponse = await SyncUserCookieAsync();
			if (cookieResponse != null)
				await SetUserModel(int.Parse(cookieResponse)); // Sets User Model so that {_user} can be called
			/// Update ViewBags Appropriately ///				
			// Check if user is a Cookie or Registered
			if (!_user.Email!.Contains("wazawareCookie6.542-Email"))
				_isCookieUser = false;
			else
				_isCookieUser = true;
			ViewBag.isCookie = _isCookieUser;
			Queue<decimal?> basePrice = new();
			Queue<decimal?> salePrice = new();
			Queue<decimal?> cartTotal = new();
			decimal? total = 0;
			var userId = _user!.UserId;
			var basket = _context.UsersShoppingCarts
							.Where(b => b.UserId == userId)
							.Select(s => s.ProductId);
			var products = _context.Products.Where(p => basket.Contains(p.ProductId))
				.OrderByDescending(p => p.ProductPriceBase)
				.Select(p => new PrimaryUserCartModel
				{
					ProductId = p.ProductId,
					ProductName = p.ProductName,
					ProductPriceBase = p.ProductPriceBase,
					ProductPriceSale = p.ProductPriceSale,
					ProductPic = p.ProductPic

				}).ToList();
			List<int> productId = new();

			foreach (var item in products)
			{
				basePrice.Enqueue(item.ProductPriceBase);
				salePrice.Enqueue(item.ProductPriceSale);
			}
			if (salePrice.Sum() > 1500)
				ViewBag.ShippingCost = "FREE";
			else
			{
				total = 1;
				ViewBag.ShippingCost = 300;
				ViewBag.GetFreeShipping = 1000 - salePrice.Sum();
			}
			if (total == 1)
				ViewBag.CartTotalSale = salePrice.Sum() + 300;
			else
				ViewBag.CartTotalSale = salePrice.Sum();
			ViewBag.BasketCount = products.Count();
			ViewBag.Savings = basePrice.Sum() - salePrice.Sum();
			ViewBag.CartTotalBase = basePrice.Sum();
			basePrice.Clear();
			salePrice.Clear();
			// View Model to return
			var viewModel1 = new PrimaryViewModel
			{
				ShoppingCart = products
			};
			return View(viewModel1);
		}
		/// <summary>
		/// Responsible for 
		/// </summary>
		[HttpGet]
		public async Task<IActionResult> Products(string search, string manufacturer, int page)
		{
			var cookieResponse = await SyncUserCookieAsync();
			if (cookieResponse != null)
				await SetUserModel(int.Parse(cookieResponse)); // Sets User Model so that {_user} can be called
			/// Update ViewBags Appropriately ///				
			// Check if user is a Cookie or Registered
			if (!_user!.Email!.Contains("wazawareCookie6.542-Email"))
				_isCookieUser = false;
			else
				_isCookieUser = true;
			ViewBag.isCookie = _isCookieUser;
			if (!_isCookieUser)
				ViewBag.FirstName = _user.FirstName;
			// Load Shopping Cart
			// Checks for products in Shopping Cart
			var shoppingCart = LoadShoppingCart(_user.UserId);
			if (shoppingCart == null)
			{
				ViewBag.ItemCount = 0;
				List<Product> shoppingCartEmpty = new()
					{
						new Product
						{
							ProductName = "non",
							ProductStock = "non",
							ProductDescription = "non",
							ProductCategory = "non",
							ProductPriceBase = 0,
							ProductPriceSale = 0,
							ProductVendorName = "non",
							ProductVendorUrl = "non",
							ProductVisibility = "visible",
							ProductDataBatchNo = "non",
							ProductImageUrl = "non"
						}
					};
			}
			else
				ViewBag.ItemCount = shoppingCart.Count;
			if (search != null)
			{
				if (page == 0)
					page = 1;
				var products = SearchProducts(search);
				ViewBag.CountProducts = products.Count();
				ViewBag.Search = search;
				// View Model to return
				var viewModelSearch = new ProductsViewModel
				{
					Products = products.ToPagedList(page, 15),
					Cart = shoppingCart
				};
				return View(viewModelSearch);				
			}
			// Advanced Search Functions
			if(manufacturer != null)
			{
				ViewBag.Manufacturer = manufacturer.ToUpper();
				switch (manufacturer)
				{
					case "amd":
						if (page == 0)
							page = 1;
						var productsAmd = _context.Products
							.Where(s => s.ProductName!.ToLower().Contains("amd") ||
							s.ProductName!.ToLower().Contains("ryzen"));			
						var viewModelManufacturerAmd = new ProductsViewModel
						{
							Products = productsAmd.ToPagedList(page, 16),
							Cart = shoppingCart
						};
						return View(viewModelManufacturerAmd);
					case "intel":
						if (page == 0)
							page = 1;
						var productsIntel = _context.Products
							.Where(s => s.ProductName!.ToLower().Contains("intel") ||
							s.ProductName!.ToLower().Contains("i7") || s.ProductName!.ToLower().Contains("i5"));
						var viewModelManufacturerIntel = new ProductsViewModel
						{
							Products = productsIntel.ToPagedList(page, 16),
							Cart = shoppingCart
						};
						return View(viewModelManufacturerIntel);
					case "nvidia":
						if (page == 0)
							page = 1;
						var productsN = _context.Products
							.Where(s => s.ProductName!.ToLower().Contains("nvidia") ||
							s.ProductName!.ToLower().Contains("rtx") || s.ProductName!.ToLower().Contains("gsync") || s.ProductName!.ToLower().Contains("g-sync"));
						var viewModelManufacturerN = new ProductsViewModel
						{
							Products = productsN.ToPagedList(page, 16),
							Cart = shoppingCart
						};
						return View(viewModelManufacturerN);
					default:
						Console.Write("404");
						break;
				}					

			}
			// If we get this far.. something has gone terribly wrong... lol :/ 
			return View();
		}
		/// <summary>
		/// Responsible for 
		/// </summary>
		[HttpGet]
		public async Task<IActionResult> Upgrade(string filter)
		{
			var products = _context.Products.Where(s => s.ProductVisibility!.Equals("ProductVisibility"));
			switch (filter)
			{
				case "Kits":
					ViewBag.Filter = "Upgrade Kits are Coming Soon!";
					return View();			
				case "SSDs":
					products = _context.Products.Where(s => s.ProductCategory!.Equals("SSDs"));
					ViewBag.Filter = "SSDs";
					break;
				case "Case Fans":
					products = _context.Products.Where(s => s.ProductCategory!.Equals("Case Fans"));
					ViewBag.Filter = "Case Fans";
					break;
				case "Motherboards":
					products = _context.Products.Where(s => s.ProductCategory!.Equals("Motherboards"));
					ViewBag.Filter = "Motherboards";
					break;
				case "PSUs":
					products = _context.Products.Where(s => s.ProductCategory!.Equals("PSUs"));
					ViewBag.Filter = "PSUs";
					break;
				case "Memory":
					products = _context.Products.Where(s => s.ProductCategory!.Equals("Memory"));
					ViewBag.Filter = "Memory";
					break;
				case "CPU Coolers":
					products = _context.Products.Where(s => s.ProductCategory!.Equals("CPU Coolers"));
					ViewBag.Filter = "CPU Coolers";
					break;					
				default :		
					products = _context.Products.Where(s => s.ProductCategory!.Equals(filter));
					if(products != null)
						ViewBag.Filter = filter;
					else
						ViewBag.Filter = "404: No Products Found!";
					break;
			}		
			return View(products.ToPagedList(1, 100));
		}
		/// <summary>
		/// Responsible for the Product RazorView
		/// </summary>
		[HttpGet]
		public IActionResult Product(int id)
		{
			var product = _context.Products.Where(p => p.ProductId!.Equals(id)).FirstOrDefault();
	
			var viewModel = new ProductsViewModel
			{
				Product = product!
			};
			return View(viewModel);
		}
		/// <summary>
		/// Responsible for Shopping UserShoppingCart Overlay
		/// </summary>
		[HttpGet]
		public IList<Product> LoadShoppingCart(int userId)
		{
			Queue<decimal?> basePrice = new();
			Queue<decimal?> salePrice = new();
			Queue<decimal?> cartTotal = new();

			// Call Shopping Cart : Select Product Id's from Database
			var basket = _context.UsersShoppingCarts
				.Where(b => b.UserId == userId)
				.Select(s => s.ProductId);
			var products = _context.Products.Where(p => basket.Contains(p.ProductId)).ToList();
			// Check if there are products in Shopping Cart
			if (products != null)
			{
				// Get base and sale prices for each product
				foreach (var p in products)
				{
					if (p.ProductPriceBase != null)
						basePrice.Enqueue(p.ProductPriceBase.Value);

					if (p.ProductPriceSale != null)
						salePrice.Enqueue(p.ProductPriceSale.Value);
				}

				// Calculate shipping cost and display information in ViewBag
				if (salePrice.Sum() > 1500)
					ViewBag.ShippingCost = "FREE";
				else
				{
					ViewBag.ShippingCost = 300;
					ViewBag.GetFreeShipping = 1000 - salePrice.Sum();
				}

				// Display cart count, savings, cart total sale, and cart total base in ViewBag
				ViewBag.BasketCount = products.Count();
				ViewBag.Savings = basePrice.Sum() - salePrice.Sum();
				ViewBag.CartTotalSale = salePrice.Sum();
				ViewBag.CartTotalBase = basePrice.Sum();

				// Display handling fee in ViewBag
				if (salePrice.Sum() > 5000)
					ViewBag.HandlingFee = "R250";
				else
					ViewBag.HandlingFee = "R50";

				// Clear basePrice and salePrice queues
				basePrice.Clear();
				salePrice.Clear();
			}				
			return products;
		}
		/// <summary>
		/// External Function Responsible for Adding Products to User Shopping UserShoppingCart & Cart
		/// </summary>
		[HttpPost]
		public async Task AddToCart(int productId)
		{
			var product = _context.Products.FirstOrDefault(s => s.ProductId == productId);
			var addProductToShoppingCart = new UserShoppingCart
			{	
				UserId = _user!.UserId,
				ProductId = productId,
				CartEntryDate = DateTime.Now
			};
			_context.Attach(addProductToShoppingCart);
			_context.Entry(addProductToShoppingCart).State = EntityState.Added;
			await _context.SaveChangesAsync();
		}
		/// <summary>
		/// External Function Responsible for Adding Products to User Shopping UserShoppingCart & Cart
		/// </summary>
		[HttpPost]
		public async Task RemoveFromCart(int productId)
		{
			var userShoppingCart = _context.UsersShoppingCarts.FirstOrDefault(s => s.UserId == _user!.UserId);
			userShoppingCart = _context.UsersShoppingCarts.FirstOrDefault(s => s.ProductId == productId);
			_context.UsersShoppingCarts.Attach(userShoppingCart);
			_context.Remove(userShoppingCart);
			await _context.SaveChangesAsync();
		}
		/// <summary>
		/// Responsible for the WebsiteCritical RazorView
		/// </summary>
		[HttpGet]
		public async Task<IActionResult> WebsiteCritical()
		{
			return View();
		}
		/// <summary>
		/// ...
		/// </summary>
		public async Task<string> SyncUserCookieAsync()
		{
			UserManagerService ums = new(_context, _httpContextAccessor);
			while (true)
			{
				const string cookieName = "wazawarecookie7";
				var requestCookies = HttpContext.Request.Cookies;
				var intialRequest = requestCookies[cookieName];
				if (intialRequest != null)
					return intialRequest;
				var cookieOptions = new CookieOptions
				{
					Expires = DateTimeOffset.Now.AddDays(7),
					IsEssential = true
				};
				// Checks for User Cookie
				if (!requestCookies.ContainsKey(cookieName))
				{
					var userId = await ums.CreateCookieReferance();
					// Create Cookie if CreateCookieReferance() was Successful
					if (userId > 0)
						HttpContext.Response.Cookies.Append(cookieName, userId.ToString(), cookieOptions);
					if (userId == 0)
						continue;
					else
						return userId.ToString();
				}
			}
		}
		/// <summary>
		/// ...
		/// </summary>
		public async Task SetUserModel(int cookieResponse)
		{
			UserManagerService ums = new(_context, _httpContextAccessor);
			_user = await ums.GetCurrentUserModel(cookieResponse);
		}
		public IPagedList<Product> SearchProducts(string search)
		{
			search = search.ToLower();
			var splitSearch = search.Split(' ');
			var products = _context.Products
				.Where(p => p.ProductName!.ToLower().Contains(search) || p.ProductName!.ToLower().Contains(splitSearch[0]))	
				.OrderByDescending(p => p.ProductPriceBase).ToPagedList();
			for(int i = 1; i > splitSearch.Length; i++)
			{
				products = _context.Products
					.Where(p => p.ProductName!.ToLower().Contains(search) || p.ProductName!.ToLower().Contains(splitSearch[i]))
					.OrderByDescending(p => p.ProductPriceBase).ToPagedList();
			}
			return products;
		}
	}
}