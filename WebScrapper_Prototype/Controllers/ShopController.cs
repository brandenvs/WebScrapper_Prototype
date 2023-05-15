using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using System.Data;
using System.Data.Entity;
using wazaware.co.za.Models.DatabaseModels;
using wazaware.co.za.Models.ViewModels;
using wazaware.co.za.Services;
using wazaware.co.za.DAL;
using X.PagedList;

namespace wazaware.co.za.Controllers
{
	public class ShopController : Controller
	{
		private readonly ILogger<ShopController> _logger;
		private readonly wazaware_db_context _DbContext;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public ShopController(ILogger<ShopController> logger, wazaware_db_context context, IHttpContextAccessor httpContextAccessor)
		{
			_logger = logger;
			_DbContext = context;
			_httpContextAccessor = httpContextAccessor;

		}
		/// <heading></heading>
		/// <summary>
		/// ...
		/// </summary>
		/// <returns></returns>
		public IActionResult Index()
		{
			return View();
		}
		[HttpGet]
		public async Task<IActionResult> Home(string search, int productId, int actionCode, string message)
		{
			// 
            ViewBag.IsCookie = false;
			// 
            WebServices services = new(_DbContext, _httpContextAccessor);
			// 
			var user = services.LoadDbUser();
			// 
            var cartModel = services.LoadCart(user!.UserId);
			// 
            if (user == null)
				ViewBag.isCookie = true;
			else
            {
                if (user.Email!.Contains("@wazaware.co.za"))
					ViewBag.isCookie = true;
				else
					ViewBag.isCookie = false;
			}
			// 
            if (message != null)
            {
                ViewBag.Message = message;
            }
            // 
            if (!string.IsNullOrEmpty(search))
                return RedirectToAction("ProductDb", new { search });
            // 
            if (productId != 0 && actionCode != 0)
            {
                switch (actionCode)
                {
                    case 1:
                        services.LoadCart(user!.UserId);
                        break;
                    // Add ProductInfomation to Shopping Cart
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
            // 
			while (user != null)
            {
                // Load Shopping Cart
                var cart = services.LoadCart(user.UserId).ToList();
                // Load Latest Arrivals
                var latestArrivalviewproducts = _DbContext.ProductDb!
                .Where(p => p.ProductVisibility!.Equals("ProductVisibility") && p.ProductPriceBase < 20000 && p.ProductPriceBase > 10000)
                .OrderByDescending(p => p.ProductPriceBase)
                .Take(8)
                .Select(p => new LatestArrival
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
                var limitedStockviewproducts = _DbContext.ProductDb!
                    .Where(p => p.ProductVisibility!.Equals("ProductVisibility") && p.ProductPriceBase < 10000 && p.ProductPriceBase > 5000)
                    .OrderByDescending(p => p.ProductPriceBase)
                    .Take(8)
                    .Select(p => new LimitedStock
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
                // Load Trending ProductDb
                var trendingviewproducts = _DbContext.ProductDb!
                    .Where(p => p.ProductVisibility!.Equals("ProductVisibility") && p.ProductPriceBase > 20000 && p.ProductPriceBase < 30000)
                    .OrderByDescending(p => p.ProductPriceBase)
                    .Take(8)
                    .Select(p => new TrendingProduct
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
				var viewUserAccount = new UserView
				{
					FirstName = user.FirstName!,
					LastName = user.LastName!,
					Email = user.Email!,
					Phone = user.Phone!
				};
				var viewPartial = new PartialView
				{
					ShoppingCart = cart,
					User = viewUserAccount
				};
				// ShopViewModel Model to return
				var view = new ShopViewModel
				{
					LatestArrivals = latestArrivalviewproducts,
					LimitedStocks = limitedStockviewproducts,
					TrendingProducts = trendingviewproducts,
					ShoppingCart = cart,
					User = viewUserAccount,
					PartialView = viewPartial
				};
				// 
                return View(view);
            }
            // 
			return View();
        }
		/// <summary>
		/// Responsible for the Cart RazorView
		/// </summary>
		/// <START>
		/// WHILE true:
		///		IF UserAccount IS NOT Null:
		///			Display ShopViewModel : Appropriate Cookie ViewBage
		///			LoadShoppingCart(userId) into Variable cart
		///			RETURN[BREAK WHILE LOOP // STOP] view { ShoppingCartView = cart }
		///		ELSE:
		///			UserAccount requires server to re-sync cookies : Trying Again
		///			[RE-SYNC COOKIES WITH SEVER]
		///			Sync Current UserAccount & Cookies WITH Controller
		///			Display ShopViewModel : Appropriate Cookie ViewBage
		///			CONTINUE
		///	</STOP>
		[HttpGet]
		public IActionResult Cart()
		{
			ViewBag.IsCookie = false;
			WebServices services = new(_DbContext, _httpContextAccessor);
			var user = services.LoadDbUser();
			var cart = services.LoadCart(user!.UserId);
			// 
			if (user == null)
				ViewBag.isCookie = true;
			else
			{
				if (user.Email!.Contains("@wazaware.co.za"))
					ViewBag.isCookie = true;
				else
					ViewBag.isCookie = false;
			}
			var viewUserAccount = new UserView
			{
				FirstName = user!.FirstName!,
				LastName = user.LastName!,
				Email = user.Email!,
				Phone = user.Phone!
			};
			var viewPartial = new PartialView
			{
				ShoppingCart = cart,
				User = viewUserAccount
			};
			var view = new UserViewModel
			{
				ShoppingCart = cart,
				User = viewUserAccount,
				PartialView = viewPartial
			};
			return View(view);
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
			            WebServices services = new(_DbContext, _httpContextAccessor);
			var user = services.LoadDbUser();
			var cart = services.LoadCart(user!.UserId);
			var viewUserAccount = new UserView
			{
				FirstName = user!.FirstName!,
				LastName = user.LastName!,
				Email = user.Email!,
				Phone = user.Phone!
			};
			// 
			if (user == null)
				ViewBag.isCookie = true;
			else
			{
				if (user.Email!.Contains("@wazaware.co.za"))
					ViewBag.isCookie = true;
				else
					ViewBag.isCookie = false;
			}
			var products = new List<ProductInfomation>();
			// Load Filters
			var viewSortBy = new List<FilterSortby>();
			var viewManufacturers = new List<FilterManufacturer>();
			Dictionary<string, string> manufactorsDict = new();
			// Load Shopping Cart
			var ShoppingCart = services.LoadCart(user!.UserId).ToList();
			// Handles Search Directs & Results
			if (search != null)
			{
				if (page == 0)
					page = 1;
				products = Searchviewproducts(search).ToList();
				ViewBag.Countviewproducts = products.Count;
				ViewBag.Search = search;
			}
			// Handles Directs for Home Side Cards
			if (manufacturer != null)
			{
				ViewBag.Manufacturer = manufacturer.ToUpper();
				switch (manufacturer)
				{
					case "amd":
						if (page == 0)
							page = 1;
						products = _DbContext.ProductDb!
							.Where(s => s.ProductName!.ToLower()
							.Contains("amd") || s.ProductName!.ToLower()
							.Contains("ryzen"))
							.Select(p => new ProductInfomation
							{
								ProductId = p.ProductId,
								ProductName = p.ProductName,
								ProductDescription = p.ProductDescription,
								ProductCategory = p.ProductCategory,
								ProductStock = p.ProductStock,
								ProductPriceBase = p.ProductPriceBase,
								ProductPriceSale = p.ProductPriceSale,
								ProductPic = p.ProductPic
							}).ToList();
						break;
					case "intel":
						if (page == 0)
							page = 1;
						products = _DbContext.ProductDb!
							.Where(s => s.ProductName!.ToLower()
							.Contains("intel") || s.ProductName!.ToLower()
							.Contains("i7") || s.ProductName!.ToLower()
							.Contains("i5"))
							.Select(p => new ProductInfomation
							{
								ProductId = p.ProductId,
								ProductName = p.ProductName,
								ProductDescription = p.ProductDescription,
								ProductCategory = p.ProductCategory,
								ProductStock = p.ProductStock,
								ProductPriceBase = p.ProductPriceBase,
								ProductPriceSale = p.ProductPriceSale,
								ProductPic = p.ProductPic
							}).ToList();
						break;
					case "nvidia":
						if (page == 0)
							page = 1;
						products = _DbContext.ProductDb!
							.Where(s => s.ProductName!.ToLower()
							.Contains("nvidia") || s.ProductName!.ToLower()
							.Contains("rtx") || s.ProductName!.ToLower()
							.Contains("gsync") || s.ProductName!.ToLower()
							.Contains("g-sync"))
							.Select(p => new ProductInfomation
							{
								ProductId = p.ProductId,
								ProductName = p.ProductName,
								ProductDescription = p.ProductDescription,
								ProductCategory = p.ProductCategory,
								ProductStock = p.ProductStock,
								ProductPriceBase = p.ProductPriceBase,
								ProductPriceSale = p.ProductPriceSale,
								ProductPic = p.ProductPic
							}).ToList();
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
				viewSortBy = sortByDict.Select(s => new FilterSortby
				{
					FilterId = s.Key,
					FilterName = s.Value
				}).ToList();

				ViewBag.Category = category;
				switch (category)
				{
					case "essential_hardware":
						ViewBag.PageTitle = "Essential Hardware";
						products = _DbContext.ProductDb!.Where(p =>
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
							.OrderBy(p => p.ProductPriceSale)
							.Select(p => new ProductInfomation
							{
								ProductId = p.ProductId,
								ProductName = p.ProductName,
								ProductDescription = p.ProductDescription,
								ProductCategory = p.ProductCategory,
								ProductStock = p.ProductStock,
								ProductPriceBase = p.ProductPriceBase,
								ProductPriceSale = p.ProductPriceSale,
								ProductPic = p.ProductPic
							}).ToList();
						manufactorsDict = new()
						{
							{ "intel", "Intel" },
							{ "amd", "AMD" },
							{ "nvidia", "Nvidia" },
							{ "msi", "MSI" },
							{ "samsung", "Samsung" }
						};
						foreach (KeyValuePair<string, string> ks in manufactorsDict)
							viewManufacturers = manufactorsDict.Select(s => new FilterManufacturer
							{
								FilterId = s.Key,
								FilterName = s.Value
							}).ToList();
						break;
					case "latest_gpus":
						ViewBag.PageTitle = "Latest GPUs";
						products = _DbContext.ProductDb!.Where(p =>
						p.ProductName!.ToLower().Contains("rtx") ||
						p.ProductName!.ToLower().Contains("amd") ||
						p.ProductName!.ToLower().Contains("nvidia") ||
						p.ProductName!.ToLower().Contains("gtx") ||
						p.ProductName!.ToLower().Contains("radeon"))
							.Select(p => new ProductInfomation
							{
								ProductId = p.ProductId,
								ProductName = p.ProductName,
								ProductDescription = p.ProductDescription,
								ProductCategory = p.ProductCategory,
								ProductStock = p.ProductStock,
								ProductPriceBase = p.ProductPriceBase,
								ProductPriceSale = p.ProductPriceSale,
								ProductPic = p.ProductPic
							}).ToList();
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
							viewManufacturers = manufactorsDict.Select(s => new FilterManufacturer
							{
								FilterId = s.Key,
								FilterName = s.Value
							}).ToList();
						break;
					case "great_deals":
						ViewBag.PageTitle = "Great Deals";
						products = _DbContext.ProductDb!.Where(p => p.ProductPriceSale / 2 >= p.ProductPriceBase).OrderByDescending(p => p.ProductPriceSale)
							.Select(p => new ProductInfomation
							{
								ProductId = p.ProductId,
								ProductName = p.ProductName,
								ProductDescription = p.ProductDescription,
								ProductCategory = p.ProductCategory,
								ProductStock = p.ProductStock,
								ProductPriceBase = p.ProductPriceBase,
								ProductPriceSale = p.ProductPriceSale,
								ProductPic = p.ProductPic
							}).ToList();
						manufactorsDict = new()
						{
							{ "amd", "AMD" },
							{ "nvidia", "Nvidia" },
							{ "samsung", "Samsung" },
							{ "gigabyte", "Gigabyte" },
							{ "dell", "Dell" }
						};
						foreach (KeyValuePair<string, string> ks in manufactorsDict)
							viewManufacturers = manufactorsDict.Select(s => new FilterManufacturer
							{
								FilterId = s.Key,
								FilterName = s.Value
							}).ToList();
						break;
					case "pc_chassis":
						ViewBag.PageTitle = "ATX & ITX Chassis";
						products = _DbContext.ProductDb!.Where(p =>
						p.ProductName!.ToLower().Contains("chassis") ||
						p.ProductName!.ToLower().Contains("pc chassis") ||
						p.ProductName!.ToLower().Contains("pc case") ||
						p.ProductDescription!.ToLower().Contains("chassis") ||
						p.ProductDescription!.ToLower().Contains("atx") ||
						p.ProductDescription!.ToLower().Contains("itx") ||
						p.ProductDescription!.ToLower().Contains("pc chassis") ||
						p.ProductDescription!.ToLower().Contains("pc case"))
						.OrderByDescending(p => p.ProductPriceSale)
						.Select(p => new ProductInfomation
						{
							ProductId = p.ProductId,
							ProductName = p.ProductName,
							ProductDescription = p.ProductDescription,
							ProductCategory = p.ProductCategory,
							ProductStock = p.ProductStock,
							ProductPriceBase = p.ProductPriceBase,
							ProductPriceSale = p.ProductPriceSale,
							ProductPic = p.ProductPic
						}).ToList();
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
							viewManufacturers = manufactorsDict.Select(s => new FilterManufacturer
							{
								FilterId = s.Key,
								FilterName = s.Value
							}).ToList();
						break;
					case "data_storage":
						ViewBag.PageTitle = "HDD, SSD & RAM";
						products = _DbContext.ProductDb!.Where(p =>
						p.ProductName!.ToLower().Contains("internal hard drive") ||
						p.ProductName!.ToLower().Contains("hard drive") ||
						p.ProductName!.ToLower().Contains("portal hard drive") ||
						p.ProductName!.ToLower().Contains("solid state drive") ||
						p.ProductName!.ToLower().Contains("portal hard drive"))
						.OrderByDescending(p => p.ProductPriceSale)
						.Select(p => new ProductInfomation
						{
							ProductId = p.ProductId,
							ProductName = p.ProductName,
							ProductDescription = p.ProductDescription,
							ProductCategory = p.ProductCategory,
							ProductStock = p.ProductStock,
							ProductPriceBase = p.ProductPriceBase,
							ProductPriceSale = p.ProductPriceSale,
							ProductPic = p.ProductPic
						}).ToList();
						manufactorsDict = new()
						{
							{ "samsung", "Samsung" },
							{ "seagate", "Seagate" },
							{ "mushkin", "Mushkin" }
						};
						foreach (KeyValuePair<string, string> ks in manufactorsDict)
							viewManufacturers = manufactorsDict.Select(s => new FilterManufacturer
							{
								FilterId = s.Key,
								FilterName = s.Value
							}).ToList();
						break;
					case "performace_laptops":
						ViewBag.PageTitle = "High-end Laptops & Accessories";
						products = _DbContext.ProductDb!.Where(p =>
						p.ProductName!.ToLower().Contains("gaming laptop") ||
						p.ProductName!.ToLower().Contains("notebook"))
						.OrderByDescending(p => p.ProductPriceSale)
						.Select(p => new ProductInfomation
						{
							ProductId = p.ProductId,
							ProductName = p.ProductName,
							ProductDescription = p.ProductDescription,
							ProductCategory = p.ProductCategory,
							ProductStock = p.ProductStock,
							ProductPriceBase = p.ProductPriceBase,
							ProductPriceSale = p.ProductPriceSale,
							ProductPic = p.ProductPic
						}).ToList();
						manufactorsDict = new()
						{
							{ "msi", "MSI" },
							{ "asus", "ASUS" },
							{ "hp", "HP" },
							{ "dell", "Dell" },
							{ "gigabyte", "Gigabyte" },
						};
						foreach (KeyValuePair<string, string> ks in manufactorsDict)
							viewManufacturers = manufactorsDict.Select(s => new FilterManufacturer
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
			// Cast ProductDb to Paged AND prepare ViewModel
			if (page < 1)
				page = 1;
			int items = 16;
			if (products.Count > 32)
				items = products.Count / 2;
			var viewviewproducts = products
				.GroupBy(p => p.ProductCategory!.ToLower())
				.Select(g => g.Take(10))
				.SelectMany(g => g).ToPagedList(page, items);
			var viewProducts = products.Select(s => new ProductInfomation
			{
				ProductId = s.ProductId,
				ProductName = s.ProductName,
				ProductStock = s.ProductStock,
				ProductDescription = s.ProductDescription,
				ProductCategory	= s.ProductCategory,
				ProductPriceBase = s.ProductPriceBase,
				ProductPriceSale = s.ProductPriceSale,
				ProductImageUrl = s.ProductImageUrl
				//ProductPic = s.ProductPic
			}).ToPagedList(1, 8);
			var viewPartial = new PartialView
			{
				ShoppingCart = ShoppingCart,
				User = viewUserAccount
			};
			var view = new ShopViewModel
			{
				Products = viewProducts,
				FilterSortby = viewSortBy,
				FilterManufacturer = viewManufacturers,
				ShoppingCart = cart,
				User = viewUserAccount,
				PartialView = viewPartial
			};
			return View(view);
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
			WebServices services = new(_DbContext, _httpContextAccessor);
			var user = services.LoadDbUser();
			var cart = services.LoadCart(user!.UserId);
			if (user == null)
			{
				ViewBag.isCookie = true;
			}
			else
			{
				if (user.Email!.Contains("@wazaware.co.za"))
				{
					ViewBag.isCookie = true;
				}
				else
				{
					ViewBag.isCookie = false;
				}
			}
			var product = _DbContext.ProductDb!.Where(p => p.ProductId!.Equals(id)).FirstOrDefault();
			var viewProduct = new ProductInfomation
			{
				ProductId = product!.ProductId,
				ProductName = product.ProductName,
				ProductStock = product.ProductStock,
				ProductDescription = product.ProductDescription,
				ProductCategory = product.ProductCategory,
				ProductPriceBase = product.ProductPriceBase,
				ProductPriceSale = product.ProductPriceSale,
				ProductImageUrl = product.ProductImageUrl
			};
			var viewUserAccount = new UserView
			{
				FirstName = user!.FirstName!,
				LastName = user.LastName!,
				Email = user.Email!,
				Phone = user.Phone!
			};
			var viewPartial = new PartialView
			{
				ShoppingCart = cart,
				User = viewUserAccount
			};
			var viewModel = new ShopViewModel
			{
				SingleProduct = viewProduct,
				ShoppingCart = cart,
				User = viewUserAccount,
				PartialView = viewPartial
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
			WebServices services = new(_DbContext, _httpContextAccessor);
			var user = services.LoadDbUser();
			var product = _DbContext.ProductDb!.Where(p => p.ProductId.Equals(productId)).First();
			var existingEntry = _DbContext.ShoppingCartDb!.FirstOrDefault(c => c.UserId == user!.UserId && c.ProductId == productId);
			if (existingEntry != null)
			{
				existingEntry.ProductCount += 1;
				existingEntry.ProductTotal += product.ProductPriceSale;
				existingEntry.CartEntryDate = DateTime.Now;
				_DbContext.Update(existingEntry);
			}
			else
			{
				var newEntry = new ShoppingCart
				{
					UserId = user!.UserId,
					ProductId = productId,
					ProductCount = 1,
					ProductTotal = product.ProductPriceSale,
					CartEntryDate = DateTime.Now
				};
				_DbContext.Add(newEntry);
			}
			await _DbContext.SaveChangesAsync();
		}
		/// <heading></heading>
		/// <summary>
		/// ...
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		public async Task RemoveFromCart(int productId)
		{
			WebServices services = new(_DbContext, _httpContextAccessor);
			var user = services.LoadDbUser();
			var ShoppingCart = _DbContext.ShoppingCartDb!
				.Where(s => s.UserId == user!.UserId && s.ProductId == productId).First();
			_DbContext.ShoppingCartDb!.Attach(ShoppingCart);
			_DbContext.Remove(ShoppingCart);
			await _DbContext.SaveChangesAsync();
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
		public IPagedList<ProductInfomation> Searchviewproducts(string search)
		{
			//
			var products = _DbContext.ProductDb;
			var normalSearch = products!.Where(p => p.ProductName!.Contains(search));
			var caseSensitiveSearch = products!.Where(p => p.ProductName!.ToLower().Contains(search.ToLower()));
			var deeperSearch = products!.Where(p => p.ProductDescription!.ToLower().Contains(search.ToLower()));
			//
			var viewproductsJoined = normalSearch.Union(caseSensitiveSearch).Union(deeperSearch).ToList();
			//
			var viewProducts = viewproductsJoined.Select(p => new ProductInfomation
			{
				ProductId = p.ProductId,
				ProductName = p.ProductName,
				ProductDescription = p.ProductDescription,
				ProductCategory = p.ProductCategory,
				ProductStock = p.ProductStock,
				ProductPriceBase = p.ProductPriceBase,
				ProductPriceSale = p.ProductPriceSale,
				ProductPic = p.ProductPic
			}).ToPagedList();
			//
			return viewProducts;
		}
	}
}