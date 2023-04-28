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
using System.Collections;
using System.Data.Entity;
using System.Globalization;
using System.Web.WebPages;

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
			if (!_user!.Email!.Contains("wazawareCookie6.542-Email"))
				_isCookieUser = false;
			else
				_isCookieUser = true;
			ViewBag.isCookie = _isCookieUser;
			if (!_isCookieUser)
				ViewBag.FirstName = _user.FirstName;
			// Load Product Categories (GAVE UP.... 10HRS)
			// var productCategories = LoadCategories(1);
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
					Cart = userShoppingCart,
					dropDown = productCategories
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
					if (!_user!.Email!.Contains("wazawareCookie6.542-Email"))
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
					if (!_user!.Email!.Contains("wazawareCookie6.542-Email"))
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
		public async Task<IActionResult> Products(string search, string manufacturer, int page, string category, string filter)
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
			var userShoppingCart = LoadShoppingCart(_user.UserId).ToList();
			var productIds = userShoppingCart.Select(c => c.ProductId).ToList();
			var productCount = userShoppingCart.GroupBy(c => c.ProductId)
				.ToDictionary(g => g.Key, g => g.ToList());

			var productsInCart = _context.Products
				.Where(p => productIds.Contains(p.ProductId))
				.Select(x => new ProductsInCartModel
				{
					ProductId = x.ProductId,
					ProductName = x.ProductName,
					ProductPriceBase = x.ProductPriceBase,
					ProductPriceSale = x.ProductPriceSale,
					ProductImageUrl = x.ProductImageUrl,
					ProductCount = productCount.ContainsKey(x.ProductId) ? productCount[x.ProductId].Count : 0,
					ProductPic = x.ProductPic
				}).ToList();

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
					Cart = productsInCart
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
							Cart = productsInCart
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
							Cart = productsInCart
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
							Cart = productsInCart
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
			if (!_user!.Email!.Contains("wazawareCookie6.542-Email"))
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
		/// <summary>
		/// Loads Categories used mainly for Nav-Bar but is versatile as it populates ViewModel
		/// START
		/// Database Query : Get Products Based on loadCode
		/// Select Variables for Product Categories & convert to List
		/// ForEach through List : add productId and ProductName to Dictionary
		/// ForEach through Dictionary : Split the product name & add each word to a List data structure
		/// ForEach through each word compare word to list of product names and find the most common word
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public IList<dropDown> LoadCategories(int loadCode)
		{
			var productsLoadCode2 = _context.Products
				.Where(p => p.ProductCategory!
				.Equals("Notebooks")).ToList();
			Dictionary<string, int> keyValuePairs = new Dictionary<string, int>();
			switch (loadCode)
			{
				case 1:
					// Database Query : Products Table -> Category == Laptops
					List<string> dd = new();
					var productsLoadCode1 = _context.Products
						.Where(p => p.ProductCategory
						.Equals("Notebooks")).ToList();
					Dictionary<string, int> kv = new();
					foreach (var product in productsLoadCode1)
					{
						var splitSearch = product.ProductName!.Split(' ');
						for (int i = 0; i < splitSearch.Length; i++)						
						{				
							if (kv.ContainsKey(splitSearch[i]))
							{
								kv[splitSearch[i]] += 1;
							}
							else
							{
								kv.Add(splitSearch[i], 1);														
							}
						}
						foreach(KeyValuePair<string, int> pair in kv.Distinct())
						{
							if (pair.Value > 9 && pair.Key.Length > 5 && pair.Key.IsInt() == false)
								dd.Add(pair.Key);
						}
					}
					var viewModel = dd.Select(d => new dropDown
					{
						ProductKeyWord = d
					});
					foreach(var a in viewModel.Distinct())
					{
						Console.WriteLine(a.ProductKeyWord + "\n\n\n\n");
					}
					return viewModel.ToList();
			}
			List<string> aa = new();
			aa.Add("Hello World");
			var viewModel1 = aa.Select(d => new dropDown
			{
				ProductKeyWord = d
			}).Take(8);
			return viewModel1.ToList();
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
			search = search.ToLower();
			var splitSearch = search.Split(' ');
			var products = _context.Products.ToList();

			var productsSearchString = products.Where(p => p.ProductName!.ToLower().Contains(search));
			var productsSearchKeyWord = products.Where(p => p.ProductName!.ToLower().Contains(splitSearch[0]));
			for (int i = 1; i < splitSearch.Length; i++)
			{
				productsSearchKeyWord = productsSearchKeyWord.Where(p => p.ProductName!.ToLower().Contains(splitSearch[i]));
			}
			var productSearchNoWhiteSpaceString = products.Where(p => p.ProductName!.ToLower().Replace(" ", "").Contains(search.Replace(" ", "")));
			var productsSearchCategories = products.Where(p => p.ProductCategory!.ToLower().Contains(search));
			var productsJoined = productsSearchString
				.Concat(productsSearchKeyWord)
				.Concat(productsSearchCategories)
				.Concat(productSearchNoWhiteSpaceString)
				.Distinct();
			return productsJoined.ToPagedList();
		}
	}
}