using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using wazaware.co.za.Models.DatabaseModels;
using wazaware.co.za.Models.ViewModels;
using wazaware.co.za.Services;
using WazaWare.co.za.DAL;
using static wazaware.co.za.Models.ViewModels.ProductView;
using static wazaware.co.za.Models.ViewModels.User;

namespace WazaWare.co.za.Controllers
{
	public class CheckoutController : Controller
	{
		private readonly ILogger<ShopController> _logger;
		private readonly WazaWare_db_context _DbContext;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IRazorViewEngine _viewEngine;
		readonly ITempDataProvider _tempData;

		public CheckoutController(ILogger<ShopController> logger, WazaWare_db_context context, IHttpContextAccessor httpContextAccessor, IRazorViewEngine viewEngine, ITempDataProvider tempData)
		{
			_logger = logger;
			_DbContext = context;
			_httpContextAccessor = httpContextAccessor;
			_viewEngine = viewEngine;
			_tempData = tempData;
		}
		[HttpGet]
		public IActionResult Index()
		{						
			// 
			WebServices services = new(_DbContext, _httpContextAccessor);
			// 
			var user = services.LoadDbUser();
			// 
			var cart = services.LoadCart(user!.UserId);
			// 
			bool isCookie = true;
			if (user != null)
			{
				if (user.Email!.Contains("@wazaware.co.za"))
					isCookie = false;
			}
			ViewBag.IsCookie = isCookie;
			if (isCookie == false)			
			{				
				if (cart.Count > 0)
					return RedirectToAction(nameof(CheckoutUser));
				else
					return RedirectToAction("Home", "Shop", new { message = "Your Shopping Cart is Empty!" });
			}
			else
			{
				ViewBag.IsCookie = isCookie;
				return RedirectToAction("Home", "Shop", new { message = "Something went wrong" });
			}
		}
		[HttpGet]
		public IActionResult CheckoutUser(string message)
		{
			WebServices services = new(_DbContext, _httpContextAccessor);
			var user = services.LoadDbUser();
			var cart = services.LoadCart(user!.UserId);
			var orders = services.LoadOrders(user!.UserId);
			var viewUserAccount = new UserView();
			bool isCookie = true;
			bool hasOrders = false;
			if (user != null)
			{
				viewUserAccount = new UserView
				{
					FirstName = user!.FirstName!,
					LastName = user.LastName!,
					Email = user.Email!,
					Phone = user.Phone!
				};
				if (user.Email!.Contains("@wazaware.co.za"))
					isCookie = false;
				if (orders != null)
					hasOrders = true;
			}	
			ViewBag.IsCookie = isCookie;
			if (isCookie == false && hasOrders == true)
			{
				var shippingAddress = _DbContext.ShippingAddressDb!.Where(s => s.UserId == user!.UserId).FirstOrDefault();
				var viewShippingAddress = new ShippingAddressView
				{
					FirstName = shippingAddress!.FirstName,
					LastName = shippingAddress.LastName,
					Email = shippingAddress.Email,
					Phone = shippingAddress.Phone,
					UnitNo = shippingAddress.UnitNo,
					StreetAddress = shippingAddress.StreetAddress,
					Suburb = shippingAddress.Suburb,
					City = shippingAddress.City,
					Province = shippingAddress.Province,
					PostalCode = shippingAddress.PostalCode,
				};
				var view = new OrderViewModel
				{
					ShoppingCart = cart,
					User = viewUserAccount,
					ShippingAddress = viewShippingAddress
				};
				return View(view);
			}
			else
			{
				var view = new OrderViewModel
				{
					ShoppingCart = cart,
					User = viewUserAccount
				};
				ViewBag.Message = message;
				return View(view);
			}
		}
		[HttpPost]
		public async Task<IActionResult> CheckoutUser(ShippingAddressView model)
		{
			WebServices services = new(_DbContext, _httpContextAccessor);
			var user = services.LoadDbUser();
			var orders = services.LoadOrders(user!.UserId);
			bool isCookie = true;
			if (user != null)
			{
				if (user.Email!.Contains("@wazaware.co.za"))
					isCookie = false;
			}
			ViewBag.IsCookie = isCookie;
			if (isCookie)
			{
				Dictionary<string, string> data = new()
				{
					{ "FirstName", model.FirstName! },
					{ "LastName", model.LastName! }
				};
				Dictionary<string, string> updatedData = services.Capitilize(data);
				// Migrate User:
				var migrateUser = _DbContext.UserAccountDb!.Find(user!.UserId);
				if (migrateUser != null)
				{
					migrateUser.FirstName = updatedData.Where(s => s.Key.Equals("FirstName")).Select(S => S.Value).First();
					migrateUser.LastName = updatedData.Where(s => s.Key.Equals("LastName")).Select(S => S.Value).First();
					migrateUser.Email = model.Email;
					migrateUser.Phone = model.Phone;

					_DbContext.UserAccountDb!.Attach(migrateUser);
					_DbContext.UserAccountDb!.Update(migrateUser);
					await _DbContext.SaveChangesAsync();
					var shippingAddress = new ShippingAddress
					{
						UserId = user.UserId,
						FirstName = model.FirstName,
						LastName = model.LastName,
						Phone = model.Phone,
						Email = model.Email,
						UnitNo = model.UnitNo,
						StreetAddress = model.StreetAddress,
						Suburb = model.Suburb,
						City = model.City,
						Province = model.Province,
						PostalCode = model.PostalCode
					};
					_DbContext.ShippingAddressDb!.Attach(shippingAddress);
					_DbContext.ShippingAddressDb!.Add(shippingAddress);
					const string cookieName = "wazaware.co.za-auto-sign-in";
					var cookieOptions = new CookieOptions
					{
						Expires = DateTimeOffset.Now.AddDays(7),
						IsEssential = true
					};
					HttpContext.Response.Cookies.Append(cookieName, model!.Email!, cookieOptions);
				}
			}
			else
			{
				var shipping = _DbContext.ShippingAddressDb!.Where(s => s.UserId.Equals(user!.UserId)).FirstOrDefault();
				if (!shipping!.Equals(model))
				{
					Dictionary<string, string> data = new()
					{
						{ "FirstName", model.FirstName! },
						{ "LastName", model.LastName! }
					};
					Dictionary<string, string> updatedData = services.Capitilize(data);
					var updateUserShipping = new ShippingAddress
					{
						UserId = user!.UserId,
						FirstName = updatedData.Where(s => s.Key.Equals("FirstName")).Select(S => S.Value).First(),
						LastName = updatedData.Where(s => s.Key.Equals("LastName")).Select(S => S.Value).First(),
						Email = model.Email,
						Phone = model.Phone,
						UnitNo = model.UnitNo,
						StreetAddress = model.StreetAddress,
						Suburb = model.Suburb,
						City = model.City,
						Province = model.Province,
						PostalCode = model.PostalCode
					};
					_DbContext.ShippingAddressDb!.Update(updateUserShipping);
					await _DbContext.SaveChangesAsync();
				}
			}
			return RedirectToAction(nameof(PlaceOrder));
		}
		[HttpGet]
		public IActionResult PlaceOrder()
		{
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
			var view = new OrderViewModel
			{
				ShoppingCart = cart,
				User = viewUserAccount,
			};
			return View(view);
		}
		[HttpGet]
		public IActionResult Payment(string message)
		{
			// 
			WebServices services = new(_DbContext, _httpContextAccessor);
			// 
			var user = services.LoadDbUser();
			// 
			var cart = services.LoadCart(user!.UserId);
			// 
			bool isCookie = true;
			if (user != null)
			{
				if (user.Email!.Contains("@wazaware.co.za"))
					isCookie = false;
			}
			ViewBag.IsCookie = isCookie;
			if (user != null && isCookie == false)
			{
				var shippingAddress = _DbContext.ShippingAddressDb!.Where(s => s.UserId == user.UserId).First();
				var viewShippingAddress = new ShippingAddressView
				{
					FirstName = shippingAddress.FirstName,
					LastName = shippingAddress.LastName,
					Phone = shippingAddress.Phone,
					Email = shippingAddress.Email,
					UnitNo = shippingAddress.UnitNo,
					StreetAddress = shippingAddress.StreetAddress,
					Suburb = shippingAddress.Suburb,
					City = shippingAddress.City,
					Province = shippingAddress.Province,
					PostalCode = shippingAddress.PostalCode,
				};
				var viewUserAccount = new UserView
				{
					FirstName = user!.FirstName!,
					LastName = user.LastName!,
					Email = user.Email!,
					Phone = user.Phone!
				};
				var view = new OrderViewModel
				{
					ShoppingCart = cart,
					User = viewUserAccount,
					ShippingAddress = viewShippingAddress
				};
				return View(view);
			}
			return RedirectToAction(nameof(CheckoutUser), new { message = "Sorry Something Went Wrong!" });
		}
		[HttpPost]
		public async Task<IActionResult> Payment(BillingAddressView model)
		{
			// 
			WebServices services = new(_DbContext, _httpContextAccessor);
			// 
			var user = services.LoadDbUser();
			// 
			var cart = services.LoadCart(user!.UserId);
			// 
			bool isCookie = true;
			if (user != null)
			{
				if (user.Email!.Contains("@wazaware.co.za"))
					isCookie = false;
			}
			if (user != null && isCookie == false)
			{
				var createOrder = new Order
				{
					UserId = user.UserId,
					ShippingPrice = 450,
					OrderTotal = cart.Select(s => s.ProductTotal).Sum(),
					IsOrderPayed = false,
					OrderCreatedOn = DateTime.Now
				};
				_DbContext.OrderDb!.Attach(createOrder);
				_DbContext.OrderDb.Add(createOrder);
				_DbContext.SaveChanges();
				var orderedProducts = cart.Select(s => new OrderedProducts
				{
					OrderId = createOrder.OrderId,
					ProductId = s.ProductId,
					ProductCount = s.ProductCount,
					ProductTotal = s.ProductTotal
				}).ToList();
				_DbContext.OrderedProductsDb!.AttachRange(orderedProducts);
				await _DbContext.OrderedProductsDb!.AddRangeAsync(orderedProducts);
				var billingAddress = new BillingAddress
				{
					OrderId = createOrder.OrderId,
					PaymentMethod = "EFT",
					FirstName = model.FirstName,
					LastName = model.LastName,
					Phone = model.Phone,
					Email = model.Email,
					UnitNo = model.UnitNo,
					StreetAddress = model.StreetAddress,
					Suburb = model.Suburb,
					City = model.City,
					Province = model.Province,
					PostalCode = model.PostalCode
				};
				_DbContext.BillingAddressDb!.Attach(billingAddress);
				_DbContext.BillingAddressDb.Add(billingAddress);
				await _DbContext.SaveChangesAsync();

				var removeCart = _DbContext.ShoppingCartDb!.Where(s => s.UserId == user.UserId).ToList();
				_DbContext.ShoppingCartDb!.AttachRange(removeCart);
				_DbContext.ShoppingCartDb!.RemoveRange(removeCart);
				await _DbContext.SaveChangesAsync();
				var cartRefreshed = services.LoadCart(user!.UserId);
				var viewUserAccount = new UserView
				{
					FirstName = user!.FirstName!,
					LastName = user.LastName!,
					Email = user.Email!,
					Phone = user.Phone!
				};
				var viewOrder = new OrderView
				{
					OrderId = createOrder.OrderId,
					ShippingPrice = createOrder.ShippingPrice,
					OrderTotal = createOrder.OrderTotal
				};
				var products = _DbContext.ProductDb!.Where(s => s.ProductId.Equals(orderedProducts.Select(s => s.ProductId)))
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
				var view = new OrderViewModel
				{
					Products = products,
					User = viewUserAccount,
					ShoppingCart = cartRefreshed
				};
				await SendEmail(view);
				return RedirectToAction(nameof(CheckoutComplete), new { orderId = createOrder.OrderId });
			}
			return RedirectToAction(nameof(CheckoutUser));
		}
		[HttpGet]
		public IActionResult CheckoutComplete(int orderId)
		{
			// 
			WebServices services = new(_DbContext, _httpContextAccessor);
			// 
			var user = services.LoadDbUser();
			// 
			var cart = services.LoadCart(user!.UserId);
			// 
			bool isCookie = true;
			if (user != null)
			{
				if (user.Email!.Contains("@wazaware.co.za"))
					isCookie = false;
			}
			ViewBag.IsCookie = isCookie;
			if (user != null && isCookie == false)
			{
				var order = _DbContext.OrderDb!.Find(orderId);
				if (order != null)
				{
					var viewOrder = new OrderView
					{
						OrderId = order.OrderId,
						ShippingPrice = order.ShippingPrice,
						OrderTotal = order.OrderTotal
					};
					var orderedProducts = _DbContext.OrderedProductsDb!.Where(s => s.OrderId == orderId)
						.Select(s => new OrderedProductsView
						{
							OrderId = s.OrderId,
							ProductId = s.ProductId,
							ProductCount = s.ProductCount,
							ProductTotal = s.ProductTotal
						}).ToList();

					var viewUserAccount = new UserView
					{
						FirstName = user!.FirstName!,
						LastName = user.LastName!,
						Email = user.Email!,
						Phone = user.Phone!
					};
					var viewShippingAddress = _DbContext.ShippingAddressDb!.Where(s => s.UserId == user.UserId)
						.Select(s => new ShippingAddressView
						{
							FirstName = s.FirstName,
							LastName = s.LastName,
							Phone = s.Phone,
							Email = s.Email,
							UnitNo = s.UnitNo,
							StreetAddress = s.StreetAddress,
							Suburb = s.Suburb,
							City = s.City,
							Province = s.Province,
							PostalCode = s.PostalCode,
						}).First();
					var view = new OrderViewModel
					{
						User = viewUserAccount,
						ShoppingCart = cart,
						OrderedProducts = orderedProducts,
						ShippingAddress = viewShippingAddress

					};
					return View(view);
				}
			}
			return RedirectToAction(nameof(CheckoutUser), new { message = "Sorry Something Went Wrong!" });
		}
		[HttpPost]
		public async Task SendEmail(OrderViewModel viewModel)
		{
			EmailerService emailerService = new(_httpContextAccessor, _viewEngine, _tempData);
			await emailerService.SendEmail(viewModel);
		}
	}

}
