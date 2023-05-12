using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using System.Data;
using System.Data.Entity;
using wazaware.co.za.Services;
using WazaWare.co.za.DAL;
using WazaWare.co.za.Models;
using X.PagedList;
using static WazaWare.co.za.Models.UserManagerViewModels;

namespace WazaWare.co.za.Controllers
{
	public class ShopController : Controller
	{
		private readonly ILogger<ShopController> _logger;
		private readonly WazaWare_db_context _context;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private static List<Product> products;

		public ShopController(ILogger<ShopController> logger, WazaWare_db_context context, IHttpContextAccessor httpContextAccessor)
		{
			_logger = logger;
			_context = context;
			_httpContextAccessor = httpContextAccessor;

		}
		/// <heading></heading>
		/// <summary>
		/// ...
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public async Task<IActionResult> Index(string search, int productId, int actionCode, string message)
		{
			ViewBag.IsCookie = false;
			WebServices services = new(_context); var userModel = CookieTime();
			var cartModel = services.LoadCart(userModel!.UserId);
			var UserView = new UserModelView
			{FirstName = userModel.FirstName!,
				LastName = userModel.LastName!,
				Email = userModel.Email!,
				Phone = userModel.Phone!
			};
			if (userModel == null)
			{
				ViewBag.isCookie = true;
			}
			else
			{
				if (userModel.Email!.Contains("@wazaware.co.za"))
				{
					ViewBag.isCookie = true;
				}
				else
				{
					ViewBag.isCookie = false;
				}
			}
			if (message != null)
			{
				ViewBag.Message = message;
			}
			// Search For Products
			if (!string.IsNullOrEmpty(search))
				return RedirectToAction("Products", new { search });
			// Dynamic Functions
			if (productId != 0 && actionCode != 0)
			{
				switch (actionCode)
				{
					case 1:
						services.LoadCart(userModel!.UserId);
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
			while (userModel != null)
			{
				// Load Shopping Cart
				var userShoppingCart = services.LoadCart(userModel.UserId).ToList();

				// Load Latest Arrivals
				var latestArrivalProducts = _context.Products!
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

				// ViewModels Model to return
				var viewModel1 = new ViewModels
				{
					LatestArrivals = latestArrivalProducts,
					LimitedStock = limitedStockProducts,
					TrendingProducts = trendingProducts,
					Cart = userShoppingCart,
					UserView = UserView
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
		///		IF UserAccount IS NOT Null:
		///			Display ViewModels : Appropriate Cookie ViewBage
		///			LoadShoppingCart(userId) into Variable userShoppingCart
		///			RETURN[BREAK WHILE LOOP // STOP] viewModel { ProductsInCartModel = userShoppingCart }
		///		ELSE:
		///			UserAccount requires server to re-sync cookies : Trying Again
		///			[RE-SYNC COOKIES WITH SEVER]
		///			Sync Current UserAccount & Cookies WITH Controller
		///			Display ViewModels : Appropriate Cookie ViewBage
		///			CONTINUE
		///	</STOP>
		[HttpGet]
		public IActionResult Cart()
		{
			ViewBag.IsCookie = false;
			WebServices services = new(_context);
			var userModel = CookieTime();
			var cartModel = services.LoadCart(userModel!.UserId);
			var UserView = new UserModelView
			{FirstName = userModel.FirstName!,
				LastName = userModel.LastName!,
				Email = userModel.Email!,
				Phone = userModel.Phone!
			};
			if (userModel == null)
			{
				ViewBag.isCookie = true;
			}
			else
			{
				if (userModel.Email!.Contains("@wazaware.co.za"))
				{
					ViewBag.isCookie = true;
				}
				else
				{
					ViewBag.isCookie = false;
				}
			}
			var viewModel = new ViewModels
			{
				Cart = cartModel,
				UserView = UserView
			};
			return View(viewModel);
		}
		/// <heading></heading>
		/// <summary>
		/// ...
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public IActionResult Products(string search, string category, string manufacturer, int page, string filterSort, string filterMan)
		{
			ViewBag.IsCookie = false;
			WebServices services = new(_context);
			var userModel = CookieTime();
			var cartModel = services.LoadCart(userModel!.UserId);
			var UserView = new UserModelView
			{
				FirstName = userModel.FirstName!,
				LastName = userModel.LastName!,
				Email = userModel.Email!,
				Phone = userModel.Phone!
			};
			if (userModel == null)
			{
				ViewBag.isCookie = true;
			}
			else
			{
				if (userModel.Email!.Contains("@wazaware.co.za"))
				{
					ViewBag.isCookie = true;
				}
				else
				{
					ViewBag.isCookie = false;
				}
			}
			var viewproducts = new List<Product>();
			// Load Filters
			var viewSortBy = new List<Filter_Sortby>();
			var viewManufacturers = new List<Filter_Manufacturer>();
			Dictionary<string, string> manufactorsDict = new();
			// Load Shopping Cart
			var userShoppingCart = services.LoadCart(userModel!.UserId).ToList();
			// Handles Search Directs & Results
			if (search != null)
			{
				if (page == 0)
					page = 1;
				products = SearchProducts(search).ToList();
				ViewBag.CountProducts = products.Count;
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
						products = _context.Products!
							.Where(s => s.ProductName!.ToLower()
							.Contains("amd") || s.ProductName!.ToLower()
							.Contains("ryzen")).ToList();
						break;
					case "intel":
						if (page == 0)
							page = 1;
						products = _context.Products!
							.Where(s => s.ProductName!.ToLower()
							.Contains("intel") || s.ProductName!.ToLower()
							.Contains("i7") || s.ProductName!.ToLower()
							.Contains("i5")).ToList();
						break;
					case "nvidia":
						if (page == 0)
							page = 1;
						products = _context.Products!
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
				switch (filterSort)
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
			var viewModel = new ViewModels
			{
				Products = viewProducts,
				Cart = userShoppingCart,
				FilterSortBy = viewSortBy,
				FilterManufacturer = viewManufacturers,
				UserView = UserView
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
		public IActionResult Product(int id)
		{
			ViewBag.IsCookie = false;
			WebServices services = new(_context);
			var userModel = CookieTime();
			var cartModel = services.LoadCart(userModel!.UserId);
			var UserView = new UserModelView
			{
				FirstName = userModel.FirstName!,
				LastName = userModel.LastName!,
				Email = userModel.Email!,
				Phone = userModel.Phone!
			};
			if (userModel == null)
			{
				ViewBag.isCookie = true;
			}
			else
			{
				if (userModel.Email!.Contains("@wazaware.co.za"))
				{
					ViewBag.isCookie = true;
				}
				else
				{
					ViewBag.isCookie = false;
				}
			}
			var product = _context.Products.Where(p => p.ProductId!.Equals(id)).FirstOrDefault();
			var viewModel = new ViewModels
			{
				Product = product!,
				Cart = cartModel,
				UserView = UserView
			};
			return View(viewModel);
		}
		/// <heading></heading>
		/// <summary>
		/// ...
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		public async Task AddToCart(int productId)
		{
			var user = CookieTime();
			var product = _context.Products!.Where(p => p.ProductId.Equals(productId)).First();
			var existingEntry = _context.UsersShoppingCarts!.FirstOrDefault(c => c.UserId == user!.UserId && c.ProductId == productId);
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
					UserId = user!.UserId,
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
			//		UserId = user!.UserId,
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
			//		UserId = user!.UserId,
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
			var user = CookieTime();
			var userShoppingCart = _context.UsersShoppingCarts!
				.Where(s => s.UserId == user!.UserId && s.ProductId == productId).First();
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
		public UserModel? CookieTime()
		{
			WebServices services = new(_context);
			UserModel? model = null;
			const string cookieName = "wazaware.co.za-auto-sign-in";
			var requestCookies = HttpContext.Request.Cookies;
			var intialRequest = requestCookies[cookieName];
			var cookieOptions = new CookieOptions
			{
				Expires = DateTimeOffset.Now.AddDays(7),
				IsEssential = true
			};
			if (intialRequest != null)
			{
				if (_context.Users.Any(u => u.Email == intialRequest))
				{
					model = _context.Users.Where(x => x.Email == intialRequest).FirstOrDefault();
				}
				else
				{
					var email = services.CreateCookieReferance().Result;
					HttpContext.Response.Cookies
						.Append(cookieName, email, cookieOptions);
					model = _context.Users
						.Where(x => x.Email == email).FirstOrDefault();
				}
			}
			else
			{
				var email = services.CreateCookieReferance().Result;
				HttpContext.Response.Cookies
					.Append(cookieName, email, cookieOptions);
				model = _context.Users!
					.Where(x => x.Email == email).FirstOrDefault();
			}
			return model;
		}
	}
}