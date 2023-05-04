using Microsoft.AspNetCore.Mvc;
using WazaWare.co.za.Models;
using X.PagedList;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.CodeAnalysis;
using WazaWare.co.za.DAL;
using Microsoft.AspNetCore.Identity;
using WazaWare.co.za.Services;
using System.Configuration;
using System.Linq;
using static WazaWare.co.za.Models.ViewModels.PrimaryModel;
using System.Collections;
using System.Data.Entity;
using System.Globalization;
using System.Web.WebPages;
using Microsoft.AspNetCore.Html;

namespace WazaWare.co.za.Controllers
{
	public class ShopController : Controller
	{
		private readonly ILogger<ShopController> _logger;
		private readonly WazaWare_db_context _context;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private static int basketCounter;
		private static UserModel? _user;
		private static Boolean _isCookieUser;
		private static List<Product> products;

		public ShopController(ILogger<ShopController> logger, WazaWare_db_context context, IHttpContextAccessor httpContextAccessor)
		{
			_logger = logger;
			_context = context;
			_httpContextAccessor = httpContextAccessor;
			ViewBag.isCookie = _isCookieUser;
		}
		/// <heading></heading>
		/// <summary>
		/// ...
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public async Task<IActionResult> Index(string search, int productId, int actionCode, int loadCode)
		{
			var cookieResponse = await SyncUserCookieAsync();
			if (cookieResponse != null)
				// Sets User Model so that {_user} can be called
				await SetUserModel(int.Parse(cookieResponse));
			/// Update ViewBags Appropriately ///				
			// Check if user is a Cookie or Registered
			if (!_user!.Email!.Contains("WazaWareCookie6.542-Email"))
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
			if (productId != 0 && actionCode != 0)
			{
				switch (actionCode)
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
						Console.WriteLine("\n------->!!\nActionCode not found...\n!!<-------\n");
						break;
				}
			}
			while (_user != null)
			{
				// Load Shopping Cart
				var userShoppingCart = LoadShoppingCart(_user.UserId).ToList();

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
					.Where(p => p.ProductVisibility!.Equals("ProductVisibility") && p.ProductPriceBase > 20000 && p.ProductPriceBase < 30000)
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
					Cart = userShoppingCart		
				};
				return View(viewModel1);
			}
			return View();
		}
		/// <summary>
		/// Responsible for the Cart RazorView
		/// </summary>
		/// <START>
		/// WHILE true:
		///		IF User IS NOT Null:
		///			Display View : Appropriate Cookie ViewBage
		///			LoadShoppingCart(userId) into Variable userShoppingCart
		///			RETURN[BREAK WHILE LOOP // STOP] viewModel { ProductsInCartModel = userShoppingCart }
		///		ELSE:
		///			User requires server to re-sync cookies : Trying Again
		///			[RE-SYNC COOKIES WITH SEVER]
		///			Sync Current User & Cookies WITH Controller
		///			Display View : Appropriate Cookie ViewBage
		///			CONTINUE
		///	</STOP>
		[HttpGet]
		public async Task<IActionResult> Cart()
		{
			while (true)
			{
				// Check if user requires cookies to be synced with sever
				if (_user != null)
				{
					// Check if user is a Cookie or Registered
					if (!_user!.Email!.Contains("WazaWareCookie6.542-Email"))
						_isCookieUser = false;
					else
						_isCookieUser = true;
					// Send Cookie to View
					ViewBag.isCookie = _isCookieUser;
					Console.WriteLine("USER Successfully Loaded!");
					// Load User Shopping Cart into Variable
					var userShoppingCart = LoadShoppingCart(_user.UserId);
					var viewModel = new ProductsViewModel
					{
						Cart = userShoppingCart
					};
					return View(viewModel);
				}
				else
				{
					// User requires server to re-sync cookies : Trying Again
					Console.WriteLine("USER IS NULL : Syncing User with server side Cookies and Trying Again...");
					var cookieResponse = await SyncUserCookieAsync();
					if (cookieResponse != null)
						// Sets User Model so that {_user} can be called
						await SetUserModel(int.Parse(cookieResponse));
					/// Update ViewBags Appropriately ///				
					// Check if user is a Cookie or Registered
					if (!_user!.Email!.Contains("WazaWareCookie6.542-Email"))
						_isCookieUser = false;
					else
						_isCookieUser = true;
					// Send Cookie to View
					ViewBag.isCookie = _isCookieUser;
					continue;
				}
			}
		}
		/// <heading></heading>
		/// <summary>
		/// ...
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public async Task<IActionResult> Products(string search, string category, string manufacturer, int page, string filterSort, string filterMan)
		{
			var cookieResponse = await SyncUserCookieAsync();
			if (cookieResponse != null)
				await SetUserModel(int.Parse(cookieResponse)); // Sets User Model so that {_user} can be called				
															   // Check if user is a Cookie or Registered
			if (!_user!.Email!.Contains("WazaWareCookie6.542-Email"))
				_isCookieUser = false;
			else
				_isCookieUser = true;
			ViewBag.isCookie = _isCookieUser;
			if (!_isCookieUser)
				ViewBag.FirstName = _user.FirstName;
			var viewproducts = new List<Product>();
			// Load Filters
			var viewSortBy = new List<Filter_Sortby>();
			var viewManufacturers = new List<Filter_Manufacturer>();
			var viewSubCategories = new List<Filter_Category>();
			Dictionary<string, string> manufactorsDict = new();
			// Load Shopping Cart
			var userShoppingCart = LoadShoppingCart(_user.UserId).ToList();
			// Handles Search Directs & Results
			if (search != null)
			{
				if (page == 0)
					page = 1;
				products = SearchProducts(search).ToList();
				ViewBag.CountProducts = products.Count();
				ViewBag.Search = search;
			}
			// Handles Directs for Shop Side Cards
			if (manufacturer != null)
			{
				ViewBag.Manufacturer = manufacturer.ToUpper();
				switch (manufacturer)
				{
					case "amd":
						if (page == 0)
							page = 1;
						products = _context.Products
							.Where(s => s.ProductName!.ToLower()
							.Contains("amd") || s.ProductName!.ToLower()
							.Contains("ryzen")).ToList();
						break;
					case "intel":
						if (page == 0)
							page = 1;
						products = _context.Products
							.Where(s => s.ProductName!.ToLower()
							.Contains("intel") || s.ProductName!.ToLower()
							.Contains("i7") || s.ProductName!.ToLower()
							.Contains("i5")).ToList();
						break;
					case "nvidia":
						if (page == 0)
							page = 1;
						products = _context.Products
							.Where(s => s.ProductName!.ToLower()
							.Contains("nvidia") || s.ProductName!.ToLower()
							.Contains("rtx") || s.ProductName!.ToLower()
							.Contains("gsync") || s.ProductName!.ToLower()
							.Contains("g-sync")).ToList();
						break;
					default:
						Console.Write("404");
						break;
				}
			}
			// Handles Nav-bar Directs & Drop-down Directs
			if (category != null)
			{
				Dictionary<string, string> sortByDict = new()
				{
					{ "price_high_low", "Highest Price" },
					{ "price_low_high", "Lowest Price" },
					{ "deals", "Best Deals" },
					{ "new", "Latest Arrivals" }
				};
					viewSortBy = sortByDict.Select(s => new Filter_Sortby
					{
						FilterId = s.Key,
						FilterName = s.Value
					}).ToList();

				ViewBag.Category = category;
				switch (category)
				{
					case "essential_hardware":
						ViewBag.PageTitle = "Essential Hardware";
						products = _context.Products.Where(p =>
						p.ProductName!.ToLower().Contains("memory") ||
						p.ProductName!.ToLower().Contains("ram") ||
						p.ProductName!.ToLower().Contains("cpu cooler") ||
						p.ProductName!.ToLower().Contains("psu") ||
						p.ProductName!.ToLower().Contains("power supply") ||
						p.ProductName!.ToLower().Contains("ddr4") ||
						p.ProductName!.ToLower().Contains("ddr5") ||
						p.ProductName!.ToLower().Contains("ddr5") ||
						p.ProductName!.ToLower().Contains("intel") ||
						p.ProductName!.ToLower().Contains("amd") ||
						p.ProductDescription!.ToLower().Contains("rgb") ||
						p.ProductDescription!.ToLower().Contains("led") ||
						p.ProductDescription!.ToLower().Contains("graphic card"))
							.OrderBy(p => p.ProductPriceSale).ToList();
						manufactorsDict = new()
						{
							{ "intel", "Intel" },
							{ "amd", "AMD" },
							{ "nvidia", "Nvidia" },
							{ "msi", "MSI" },
							{ "samsung", "Samsung" }
						};
						foreach (KeyValuePair<string, string> ks in manufactorsDict)
							viewManufacturers = manufactorsDict.Select(s => new Filter_Manufacturer
							{
								FilterId = s.Key,
								FilterName = s.Value
							}).ToList();						
						break;
					case "latest_gpus":
						ViewBag.PageTitle = "Latest GPUs";
						products = _context.Products.Where(p =>
						p.ProductName!.ToLower().Contains("rtx") ||
						p.ProductName!.ToLower().Contains("amd") ||
						p.ProductName!.ToLower().Contains("nvidia") ||
						p.ProductName!.ToLower().Contains("gtx") ||
						p.ProductName!.ToLower().Contains("radeon")).ToList();
						products = products.Where(p => p.ProductName!.ToLower().Contains("graphics card"))
						.OrderByDescending(p => p.ProductPriceSale).ToList();
						manufactorsDict = new()
						{
							{ "amd", "AMD" },
							{ "nvidia", "Nvidia" },
							{ "rtx", "RTX" },
							{ "gtx", "GTX" },
							{ "radeon", "Radeon" },
							{ "palit", "Palit" },
							{ "zotac","Zotac" }
						};
						foreach (KeyValuePair<string, string> ks in manufactorsDict)
							viewManufacturers = manufactorsDict.Select(s => new Filter_Manufacturer
							{
								FilterId = s.Key,
								FilterName = s.Value
							}).ToList();
						break;
					case "great_deals":
						ViewBag.PageTitle = "Great Deals";
						products = _context.Products.Where(p => p.ProductPriceSale / 2 >= p.ProductPriceBase).OrderByDescending(p => p.ProductPriceSale).ToList();
						manufactorsDict = new()
						{
							{ "amd", "AMD" },
							{ "nvidia", "Nvidia" },
							{ "samsung", "Samsung" },
							{ "gigabyte", "Gigabyte" },
							{ "dell", "Dell" }
						};
						foreach (KeyValuePair<string, string> ks in manufactorsDict)
							viewManufacturers = manufactorsDict.Select(s => new Filter_Manufacturer
							{
								FilterId = s.Key,
								FilterName = s.Value
							}).ToList();
						break;
					case "pc_chassis":
						ViewBag.PageTitle = "ATX & ITX Chassis";
						products = _context.Products.Where(p =>
						p.ProductName!.ToLower().Contains("chassis") ||
						p.ProductName!.ToLower().Contains("pc chassis") ||
						p.ProductName!.ToLower().Contains("pc case") ||
						p.ProductDescription!.ToLower().Contains("chassis") ||
						p.ProductDescription!.ToLower().Contains("atx") ||
						p.ProductDescription!.ToLower().Contains("itx") ||
						p.ProductDescription!.ToLower().Contains("pc chassis") ||
						p.ProductDescription!.ToLower().Contains("pc case"))
						.OrderByDescending(p => p.ProductPriceSale).ToList();
						manufactorsDict = new()
						{
							{ "phanteks", "Phanteks" },
							{ "fractel design", "Fractel Design" },
							{ "super flow", "Super Flow" },
							{ "EVGA", "EVGA" },
							{ "thermaltake", "Thermaltake" },
							{ "montech", "Montech" },
							{ "corsair", "Corsair" }
						};
						foreach (KeyValuePair<string, string> ks in manufactorsDict)
							viewManufacturers = manufactorsDict.Select(s => new Filter_Manufacturer
							{
								FilterId = s.Key,
								FilterName = s.Value
							}).ToList();
						break;
					case "data_storage":
						ViewBag.PageTitle = "HDD, SSD & RAM";
						products = _context.Products.Where(p =>
						p.ProductName!.ToLower().Contains("internal hard drive") ||
						p.ProductName!.ToLower().Contains("hard drive") ||
						p.ProductName!.ToLower().Contains("portal hard drive") ||
						p.ProductName!.ToLower().Contains("solid state drive") ||
						p.ProductName!.ToLower().Contains("portal hard drive"))
						.OrderByDescending(p => p.ProductPriceSale).ToList();
						manufactorsDict = new()
						{
							{ "samsung", "Samsung" },
							{ "seagate", "Seagate" },
							{ "mushkin", "Mushkin" }
						};
						foreach (KeyValuePair<string, string> ks in manufactorsDict)
							viewManufacturers = manufactorsDict.Select(s => new Filter_Manufacturer
							{
								FilterId = s.Key,
								FilterName = s.Value
							}).ToList();
						break;
					case "performace_laptops":
						ViewBag.PageTitle = "High-end Laptops & Accessories";
						products = _context.Products.Where(p =>
						p.ProductName!.ToLower().Contains("gaming laptop") ||
						p.ProductName!.ToLower().Contains("notebook"))
						.OrderByDescending(p => p.ProductPriceSale).ToList();
						manufactorsDict = new()
						{
							{ "msi", "MSI" },
							{ "asus", "ASUS" },
							{ "hp", "HP" },
							{ "dell", "Dell" },
							{ "gigabyte", "Gigabyte" },
						};
						foreach (KeyValuePair<string, string> ks in manufactorsDict)
							viewManufacturers = manufactorsDict.Select(s => new Filter_Manufacturer
							{
								FilterId = s.Key,
								FilterName = s.Value
							}).ToList();
						break;
				}
			}
			if (filterSort != null)
			{
				ViewBag.filterSort = filterSort;
				switch(filterSort)
				{
					case "price_high_low":
						products = products.OrderByDescending(p => p.ProductPriceSale).ToList(); 
						break;
					case "price_low_high":
						products = products.OrderBy(p => p.ProductPriceSale).ToList();
						break;
					case "deals":
						products = products.OrderByDescending(p => p.ProductPriceBase / 2 > p.ProductPriceSale).ToList();
						break;
					case "new":
						products = products.OrderByDescending(p => p.ProductPriceBase / 2 < p.ProductPriceSale).ToList();
					break;
				}
			}
			if (filterMan != null)
			{
				ViewBag.filterMan = filterMan;
				products = products.Where(p =>
				p.ProductName!.ToLower().Contains(filterMan.ToLower())).ToList();
			}
			// Cast Products to Paged AND prepare ViewModel
			if (page < 1)
				page = 1;
			int items = 16;
			if (products.Count > 32)
				items = products.Count / 2;
			var viewProducts = products
				.GroupBy(p => p.ProductCategory!.ToLower())
				.Select(g => g.Take(10))
				.SelectMany(g => g).ToPagedList(page, items);
			var viewModel = new ProductsViewModel
			{
				Products = viewProducts,
				Cart = userShoppingCart,
				FilterSortBy = viewSortBy,
				FilterManufacturer = viewManufacturers
				//FilterCategory = viewSubCategories
			};
			return View(viewModel);
		}
		/// <heading></heading>
		/// <summary>
		/// ...
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public async Task<IActionResult> Product(int id)
		{
			var cookieResponse = await SyncUserCookieAsync();
			if (cookieResponse != null)
				await SetUserModel(int.Parse(cookieResponse)); // Sets User Model so that {_user} can be called
			/// Update ViewBags Appropriately ///				
			// Check if user is a Cookie or Registered
			if (!_user!.Email!.Contains("WazaWareCookie6.542-Email"))
				_isCookieUser = false;
			else
				_isCookieUser = true;
			ViewBag.isCookie = _isCookieUser;
			// Load Shopping Cart
			var userShoppingCart = LoadShoppingCart(_user.UserId).ToList();

			var product = _context.Products.Where(p => p.ProductId!.Equals(id)).FirstOrDefault(); 
			if (product != null)
			{
				ViewBag.Oops = false;
				var viewModel = new ProductsViewModel
				{
					Cart = userShoppingCart,
					Product = product
				};
				return View(viewModel);
			}
			else
			{
				ViewBag.Oops = true;
				var viewModel = new ProductsViewModel
				{
					Cart = userShoppingCart
				};
				return View(viewModel);
			}
		}
		/// <heading></heading>
		/// <summary>
		/// ...
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public IList<ProductsInCartModel> LoadShoppingCart(int userId)
		{
			// Query Database for User's Shopping Cart using variable userId
			// Convert Database response to a List using : .ToList()
			var userShoppingCart = _context.UsersShoppingCarts
				.Where(c => c.UserId == userId)
				.ToList();
			// Select Product Ids in Shopping Cart List
			var productIds = userShoppingCart
				.Select(c => c.ProductId)
				.ToList();
			// Query Database for Products related to User's Shopping Cart
			// Convert Database response to a List using : .ToList()
			var products = _context.Products
				.Where(p => productIds.Contains(p.ProductId))
				.ToList();
			// Join Products & Shopping Cart Lists
			var joint = userShoppingCart
				.Join(products, c => c.ProductId, p => p.ProductId, (c, p) => new { ShoppingCart = c, Product = p })
				.ToList();
			// Queue Product Total Values 
			Queue<decimal?> totalBase = new();
			Queue<decimal?> totalSale = new();
			foreach (var product in joint)
			{
				totalBase.Enqueue(product.Product.ProductPriceBase * product.ShoppingCart.ProductCount);
				totalSale.Enqueue(product.Product.ProductPriceSale * product.ShoppingCart.ProductCount);
			}
			// Calculations & Building View Model:
			decimal? shippingThreshold = 1500;
			decimal? shippingCost = 0;
			decimal? tryGetFreeShipping = 0;
			// Calculate : Shipping Costs
			if (totalSale.Sum() < shippingThreshold)
			{
				shippingCost = 450;
				tryGetFreeShipping = 1500 - totalSale.Sum();
			}
			// Calculate Sum : Cart Base Total
			decimal? cartBaseTotal = totalBase.Sum() + shippingCost;
			// Calculate Sum : Cart Sale Total
			decimal? cartSaleTotal = totalSale.Sum() + shippingCost;
			// Calculate Difference : Cart Base Total - Cart Sale Total
			decimal? cartBaseSaleDiff = cartBaseTotal - cartSaleTotal;
			// Build Model
			var productsInCart = products
				.Select(x => new ProductsInCartModel
				{
					ProductId = x.ProductId,
					ProductName = x.ProductName,
					ProductPriceBase = x.ProductPriceBase,
					ProductPriceSale = x.ProductPriceSale,
					ProductImageUrl = x.ProductImageUrl,
					// Count : Quantity of a Product in Cart
					ProductCount = userShoppingCart
					.Where(p => p.ProductId == x.ProductId)
					.Select(s => s.ProductCount)
					.First(),
					// Calculate : Products Sale Total
					ProductTotal = userShoppingCart
					.Where(p => p.ProductId == x.ProductId)
					.Select(s => s.ProductCount)
					.First() * x.ProductPriceSale,
					ProductBaseSaleDiff = x.ProductPriceBase - x.ProductPriceSale,
					CartBaseTotal = cartBaseTotal,
					CartSaleTotal = cartSaleTotal,
					CartBaseSaleDiff = cartBaseSaleDiff,
					ShippingCost = shippingCost,
					TryGetFreeShipping = tryGetFreeShipping,
					ProductPic = x.ProductPic
				});
			// Return User's Cart Information in a List Data Type to Relevant Action Method
			return productsInCart.ToList();
		}
		/// <heading></heading>
		/// <summary>
		/// ...
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		public async Task AddToCart(int productId)
		{
			var product = _context.Products.Where(p => p.ProductId.Equals(productId)).First();
			var existingEntry = _context.UsersShoppingCarts.FirstOrDefault(c => c.UserId == _user!.UserId && c.ProductId == productId);
			if (existingEntry != null)
			{
				existingEntry.ProductCount += 1;
				existingEntry.ProductTotal += product.ProductPriceSale;
				existingEntry.CartEntryDate = DateTime.Now;
				_context.Update(existingEntry);
			}
			else
			{
				var newEntry = new UserShoppingCart
				{
					UserId = _user!.UserId,
					ProductId = productId,
					ProductCount = 1,
					ProductTotal = product.ProductPriceSale,
					CartEntryDate = DateTime.Now
				};
				_context.Add(newEntry);
			}
			await _context.SaveChangesAsync();

			//var product = _context.Products.FirstOrDefault(s => s.ProductId == productId);
			//var cart = _context.UsersShoppingCarts.ToList();
			//int count = 0;
			//foreach(var cartProduct in cart)
			//{
			//	if(cartProduct.ProductId == productId)
			//	{
			//		count += 1;
			//	}				
			//}
			//if(count > 0)
			//{
			//	var updateProductInShoppingCart = new UserShoppingCart
			//	{
			//		UserId = _user!.UserId,
			//		ProductId = productId,
			//		ProductCount = count,
			//		CartEntryDate = DateTime.Now
			//	};
			//	_context.Update(updateProductInShoppingCart);
			//	await _context.SaveChangesAsync();
			//}
			//else
			//{
			//	var addProductToShoppingCart = new UserShoppingCart
			//	{
			//		UserId = _user!.UserId,
			//		ProductId = productId,
			//		CartEntryDate = DateTime.Now
			//	};
			//	_context.Attach(addProductToShoppingCart);
			//	_context.Entry(addProductToShoppingCart).State = EntityState.Added;
			//	await _context.SaveChangesAsync();
			//}
		}
		/// <heading></heading>
		/// <summary>
		/// ...
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		public async Task RemoveFromCart(int productId)
		{
			var userShoppingCart = _context.UsersShoppingCarts
				.Where(s => s.UserId == _user!.UserId && s.ProductId == productId).First();
			_context.UsersShoppingCarts.Attach(userShoppingCart);
			_context.Remove(userShoppingCart);
			await _context.SaveChangesAsync();
		}
		/// <heading></heading>
		/// <summary>
		/// ...
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public IActionResult WebsiteCritical()
		{
			return View();
		}
		/// <heading></heading>
		/// <summary>
		/// ...
		/// </summary>
		/// <returns></returns>
		public async Task<string> SyncUserCookieAsync()
		{
			UserManagerService ums = new(_context, _httpContextAccessor);
			while (true)
			{
				const string cookieName = "WazaWarecookie7";
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
		/// <heading></heading>
		/// <summary>
		/// ...
		/// </summary>
		/// <returns></returns>
		public async Task SetUserModel(int cookieResponse)
		{
			UserManagerService ums = new(_context, _httpContextAccessor);
			_user = await ums.GetCurrentUserModel(cookieResponse);
		}
		/// <heading></heading>
		/// <summary>
		/// ...
		/// </summary>
		/// <returns></returns>
		public IPagedList<Product> SearchProducts(string search)
		{
			//
			var products = _context.Products;
			var normalSearch = products.Where(p => p.ProductName!.Contains(search));
			var caseSensitiveSearch = products.Where(p => p.ProductName!.ToLower().Contains(search.ToLower()));
			var deeperSearch = products.Where(p => p.ProductDescription!.ToLower().Contains(search.ToLower()));
			//
			var productsJoined = normalSearch.Union(caseSensitiveSearch).Union(deeperSearch).ToList();
			//
			return productsJoined.ToPagedList();
		}
	}
}