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
		public async Task<IActionResult> Products(string search, string manufacturer, int page, string category, bool filter, string filterSort, string filterOther)
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
			// Load Products
			var products = new List<Product>();
			// Load Filters
			var viewSortBy = new List<Filter_Sortby>();
			var viewManufacturers = new List<Filter_Manufacturer>();
			var viewSubCategories = new List<Filter_Category>();
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
							.Contains("amd") ||	s.ProductName!.ToLower()
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
				//	Populate sub Sort By for sub nav-bar
				Dictionary<string, string> sortBy = new()
						{
							{ "sort_best_deals", "Best Discounts" },
							{ "sort_low_high_price", "Lowest Price" },
							{ "sort_high_low_price", "Highest Price" },
							{ "sort_new_deals", "New Deals" }
						};
				viewSortBy = sortBy.Select(r => new Filter_Sortby
				{
					FilterId = r.Key,
					FilterName = r.Value
				}).ToList();

				ViewBag.Category = category;
				Dictionary<string, string> manufacturers = new();
				Dictionary<string, string> subCategories = new();

				switch (category.ToLower())
				{
					/// Category = HARDWARE
					case "hardware_parent":

						ViewBag.Title = "Hardware";
						products = _context.Products.ToList();
						//	IF Hardware : Populate sub Manufacturers in sub nav-bar
						manufacturers = new Dictionary<string, string>
						{
							{ "intel", "Intel" },
							{ "amd", "AMD" },
							{ "gigabyte", "Gigabyte" }
						};
						// Generate View Model Key, Value
						viewManufacturers = manufacturers.Select(r => new Filter_Manufacturer
						{
							FilterId = r.Key,
							FilterName = r.Value
						}).ToList();
						//	IF Hardware : Populate sub Categories in sub nav-bar
						subCategories = new Dictionary<string, string>
						{
							{ "memory", "Memory" },
							{ "psus", "PSUs" },
							{ "case fans", "Case Fans" },
							{ "motherboards", "Motherboards" },
							{ "cpu coolers", "CPU Coolers" }
						};
						// Generate View Model Key, Value
						viewSubCategories = subCategories.Select(r => new Filter_Category
						{
							FilterId = r.Key,
							FilterName = r.Value
						}).ToList();
						break;
					/// </category>
					/// ---------------------------------------------------------
					/// Category = GPUS
					case "gpus_parent":
						products = _context.Products
							.Where(p => p.ProductCategory!.ToLower()
							.Equals("gpus")).ToList();
						break;
					/// </category>
					/// ---------------------------------------------------------
					/// Category = GPUS
					case "deals_parent":
						products = _context.Products.ToList();
						break;
					/// </category>
					/// ---------------------------------------------------------
					/// Category = GPUS
					case "pc_parent":
						products = _context.Products
							.Where(p => p.ProductCategory!.ToLower()
							.Equals("SSD")).ToList();
						break;
					/// </category>
					/// ---------------------------------------------------------
					/// Category = GPUS
					case "laptops_parent":
						products = _context.Products
							.Where(p => p.ProductCategory!.ToLower()
							.Equals("notebooks")).ToList();
						break;
					/// </category>
				}
				if (filter)
				{
					if(filterOther != null)
					{
						bool isManufactor = manufacturers.TryGetValue(filterOther.ToLower(), out string? manufactor);
						bool isCategory = subCategories.TryGetValue(filterOther.ToLower(), out string? cat);
						if(isManufactor)
						{
							products = products.Where(p => p.ProductName!.ToLower()
							.Contains(manufactor.ToLower())).ToList();
							ViewBag.FilterOtherTitle = manufactor;
						}
						else
						{
							products = products.Where(p => p.ProductCategory!.ToLower()
							.Equals(cat.ToLower())).ToList();
							ViewBag.FilterOtherTitle = cat;
						}
					}
					if (filterSort != null)
					{
						bool isSortBy = sortBy.TryGetValue(filterSort, out string? item);
						if (isSortBy)
						{							
							products = filterSort switch
							{
								"sort_best_deals" => products
								.Where(p =>
								p.ProductPriceBase / 2m < p.ProductPriceSale)
								.OrderByDescending(p => p.ProductPriceSale).ToList(),
								"sort_low_high_price" => products
								.OrderBy(p => p.ProductPriceSale).ToList(),
								"sort_high_low_price" => products
								.OrderByDescending(p => p.ProductPriceSale).ToList(),
								"sort_price" => products
								.OrderBy(p => p.ProductPriceSale).ToList(),
								_ => throw new NotImplementedException()
							};
							ViewBag.FilterSortTitle = item;
						}
						else
						{
							var a = sortBy.Where(s => s.Value.Equals(filterSort)).First().Key;							
							products = a switch
							{
								"sort_best_deals" => products
								.Where(p =>
								p.ProductPriceBase / 2m < p.ProductPriceSale)
								.OrderByDescending(p => p.ProductPriceSale).ToList(),
								"sort_low_high_price" => products
								.OrderBy(p => p.ProductPriceSale).ToList(),
								"sort_high_low_price" => products
								.OrderByDescending(p => p.ProductPriceSale).ToList(),
								"sort_price" => products
								.OrderBy(p => p.ProductPriceSale).ToList(),
								_ => throw new NotImplementedException()
							};
							ViewBag.FilterSortTitle = filterSort;
						}
					}

					//switch (filter)
					//{						
					//	case "sort_best_deals":
					//		if (ViewBag.FilterTitle0 != null)

					//		else if (ViewBag.FilterTitle1 != null)
					//			ViewBag.FilterTitle1 = "Sort By: Best Deals";
					//		else
					//			ViewBag.FilterTitle2 = "Sort By: Best Deals";
					//		break;
					//	case "sort_new_deals":
					//		if (ViewBag.FilterTitle0 != null)
					//			ViewBag.FilterTitle0 = "Sort By: New Deals";
					//		else if (ViewBag.FilterTitle1 != null)
					//			ViewBag.FilterTitle1 = "Sort By: New Deals";
					//		else
					//			ViewBag.FilterTitle2 = "Sort By: New Deals";
					//		break;
					//	case "sort_low_high_price":
					//		if (ViewBag.FilterTitle0 != null)
					//			ViewBag.FilterTitle0 = "Sort By: Best Deals";
					//		else if (ViewBag.FilterTitle1 != null)
					//			ViewBag.FilterTitle1 = "Sort By: New Deals";
					//		else
					//			ViewBag.FilterTitle2 = "Sort By: Best Deals";
					//		break;
					//	case "sort_high_low_price":
					//		if (ViewBag.FilterTitle0 != null)
					//			ViewBag.FilterTitle0 = "Sort By: Best Deals";
					//		else if (ViewBag.FilterTitle1 != null)
					//			ViewBag.FilterTitle1 = "Sort By: Best Deals";
					//		else
					//			ViewBag.FilterTitle2 = "Sort By: Best Deals";
					//		break;
					//	default:
					//		if (ViewBag.FilterTitle0 != null)
					//			ViewBag.FilterTitle0 = "Sort By: Best Deals";
					//		else if (ViewBag.FilterTitle1 != null)
					//			ViewBag.FilterTitle1 = "Sort By: Best Deals";
					//		else
					//			ViewBag.FilterTitle2 = "Sort By: Best Deals";
					//		break;
					//}

				}
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
				FilterManufacturer = viewManufacturers,
				FilterCategory = viewSubCategories
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
			search = search.ToLower();
			//
			var splitSearch = search.Split(' ');
			//
			var products = _context.Products.ToList();
			//
			var productsSearchString = products.Where(p => p.ProductName!.ToLower().Contains(search));
			//
			var productSearchNoWhiteSpaceString = products.Where(p => p.ProductName!.ToLower().Replace(" ", "").Contains(search.Replace(" ", "")));
			//
			var productsSearchCategories = products.Where(p => p.ProductCategory!.ToLower().Contains(search));
			//
			var productsJoined = productsSearchString
				.Concat(productsSearchCategories)
				.Concat(productSearchNoWhiteSpaceString)
				.Distinct();
			//
			return productsJoined.ToPagedList();
		}
	}
}