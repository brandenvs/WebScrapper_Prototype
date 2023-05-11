using System.IO;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using WazaWare.co.za.Models;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using WazaWare.co.za.DAL;
using wazaware.co.za.Services;
using static WazaWare.co.za.Models.UserManagerViewModels;
using wazaware.co.za.Models;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace WazaWare.co.za.Controllers
{
	public class CheckoutController : Controller
	{
		private readonly ILogger<ShopController> _logger;
		private readonly WazaWare_db_context _context;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IRazorViewEngine _viewEngine;
		ITempDataProvider _tempData;
		private static UserModel? _user;
		private static Boolean _isCookieUser;

		public CheckoutController(ILogger<ShopController> logger, WazaWare_db_context context, IHttpContextAccessor httpContextAccessor, IRazorViewEngine viewEngine, ITempDataProvider tempData)
		{
			_logger = logger;
			_context = context;
			_httpContextAccessor = httpContextAccessor;
			_viewEngine = viewEngine;
			_tempData = tempData;
		}
		[HttpGet]
		public IActionResult Index()
		{			
			var userModel = CookieTime();
			if (userModel!.Email!.Contains("@wazaware.co.za"))
			{
				if(_context.UsersShoppingCarts.Any(s => s.UserId == userModel.UserId))
					return RedirectToAction(nameof(CheckoutNewUser));
				else
					return RedirectToAction("Index", "Shop", new { message = "Your Shopping Cart is Empty!" });
			}
			else
			{
				if (_context.UsersShoppingCarts.Any(s => s.UserId == userModel.UserId))
					return RedirectToAction(nameof(CheckoutNewUser));
				else
					return RedirectToAction("Index", "Shop", new { message = "Your Shopping Cart is Empty!" });
			}
		}
		[HttpGet]
		public IActionResult CheckoutNewUser(string message)
		{
			WebServices services = new(_context, _httpContextAccessor);
			var userModel = CookieTime();
			var cartModel = services.LoadCart(userModel!.UserId);
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
			var userView = new UserModelView
			{
				FirstName = userModel.FirstName,
				LastName = userModel.LastName,
				Email = userModel.Email,
				Phone = userModel.Phone
			};
			var viewModel = new ViewModels
			{
				Cart = cartModel,
				userView = userView
			};
			ViewBag.Message = message;
			return View(viewModel);
		}
		[HttpPost]
		public async Task<IActionResult> CheckoutNewUser(UserShippingViewModel model)
		{
			var userModel = CookieTime();
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
			string notes = "No Instructions Given";
			if(model.Notes != null)
				notes = model.Notes;
			string password = "AccountWasNotMade!";
			if (model.Password != null)
				password = model.Password;
			var newUserShipping = new UserShipping
			{
				UserId = userModel!.UserId,
				FirstName = model.FirstName,
				LastName = model.LastName,
				Email = model.Email,
				Phone = model.Phone,
				UnitNo = model.UnitNo,
				StreetAddress = model.StreetAddress,
				Suburb = model.Suburb,
				City = model.City,
				Province = model.Province,
				PostalCode = model.PostalCode,
				Notes = notes
			};
			_context.UserShippings.Attach(newUserShipping);
			_context.UserShippings.Add(newUserShipping);
			var userToUpdate = _context.Users.FirstOrDefault(u => u.UserId == userModel.UserId);

			if (userToUpdate != null)
			{
				userToUpdate.FirstName = model.FirstName;
				userToUpdate.LastName = model.LastName;
				userToUpdate.Email = model.Email;
				userToUpdate.Phone = model.Phone;
				userToUpdate.Password = password;

				_context.Users.Update(userToUpdate);				
			}
			await _context.SaveChangesAsync();

			const string cookieName = "wazaware.co.za-auto-sign-in";
			var cookieOptions = new CookieOptions
			{
				Expires = DateTimeOffset.Now.AddDays(7),
				IsEssential = true
			};
			HttpContext.Response.Cookies.Append(cookieName, userToUpdate!.Email!, cookieOptions);

			return RedirectToAction(nameof(PlaceOrder));
		}
		[HttpGet]
		public IActionResult CheckoutCurrentUser()
		{
			WebServices services = new(_context, _httpContextAccessor);
			var userModel = CookieTime();
			var cartModel = services.LoadCart(userModel!.UserId);
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
			var userView = new UserModelView
			{
				FirstName = userModel.FirstName,
				LastName = userModel.LastName,
				Email = userModel.Email,
				Phone = userModel.Phone
			};
			var viewModel = new ViewModels
			{
				Cart = cartModel,
				userView = userView
			};
			return View(viewModel);
		}
		[HttpGet]
		public IActionResult PlaceOrder()
		{
			WebServices services = new(_context, _httpContextAccessor);
			var userModel = CookieTime();
			var cartModel = services.LoadCart(userModel!.UserId);
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
			var userView = new UserModelView
			{
				FirstName = userModel.FirstName,
				LastName = userModel.LastName,
				Email = userModel.Email,
				Phone = userModel.Phone
			};
			var viewModel = new ViewModels
			{
				Cart = cartModel,
				userView = userView
			};
			return View(viewModel);
		}
		[HttpGet]
		public async Task<IActionResult> Payment(string message)
		{
			WebServices services = new(_context, _httpContextAccessor);
			var userModel = CookieTime();
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
			if (userModel != null && !userModel.Email!.Contains("@wazaware.co.za"))
			{
				if(message != null) 
				{
					var paymentModel = _context.PaymentModels.Where(s => s.UserId == userModel.UserId).First();
					var shippingModel = _context.UserShippings.Where(s => s.UserId == userModel.UserId).First();
					var cartModel = services.LoadCart(userModel!.UserId);
					// RESET CART:
					var removeCart = _context.UsersShoppingCarts.Where(s => s.UserId != userModel.UserId).ToList();
					_context.UsersShoppingCarts.AddRange(removeCart);
					_context.UsersShoppingCarts.RemoveRange(removeCart);
					await _context.SaveChangesAsync();
					var order = _context.Orders.Where(s => s.UserId == userModel.UserId).First();
					var orderSummary = new OrderSummary
					{
						OrderId = order.OrderId,
						PaymentMethod = paymentModel!.PaymentMethod,
						OrderTotal = cartModel!.Select(s => s.CartSaleTotalFormatted!).First()
					};
					var userView = new UserModelView
					{
						FirstName = userModel!.FirstName!,
						LastName = userModel!.LastName!,
						Email = userModel.Email,
						Phone = userModel.Phone
					};
					string notes = "No Instructions Provided";
					if (shippingModel.Notes != null)
						notes = shippingModel.Notes;
					var shippingView = new UserShippingViewModel
					{
						FirstName = shippingModel.FirstName,
						LastName = shippingModel.LastName,
						Email = shippingModel.Email,
						Phone = shippingModel.Phone,
						UnitNo = shippingModel.UnitNo,
						StreetAddress = shippingModel.StreetAddress,
						Suburb = shippingModel.Suburb,
						City = shippingModel.City,
						Province = shippingModel.Province,
						PostalCode = shippingModel.PostalCode,
						Notes = notes
					};
					var view = new ViewModels
					{
						userShippingView = shippingView,
						Payment = paymentModel,
						Cart = cartModel,
						Summary = orderSummary,
						userView = userView
					};
					ViewBag.Message = "hasBilling";
                    await SendEmail(view);
                    return View(view);
				}
				else
				{
					var cartModel = services.LoadCart(userModel!.UserId);
					var shippingModel = _context.UserShippings.Where(s => s.UserId == userModel.UserId).First();
					var userView = new UserModelView
					{
						FirstName = userModel!.FirstName!,
						LastName = userModel!.LastName!,
						Email = userModel.Email,
						Phone = userModel.Phone
					};
					string notes = "No Instructions Provided";
					if (shippingModel.Notes != null)
						notes = shippingModel.Notes;
					var shippingView = new UserShippingViewModel
					{
						FirstName = shippingModel.FirstName,
						LastName = shippingModel.LastName,
						Email = shippingModel.Email,
						Phone = shippingModel.Phone,
						UnitNo = shippingModel.UnitNo,
						StreetAddress = shippingModel.StreetAddress,
						Suburb = shippingModel.Suburb,
						City = shippingModel.City,
						Province = shippingModel.Province,
						PostalCode = shippingModel.PostalCode,
						Notes = notes
					};
					var view = new ViewModels
					{
						Cart = cartModel,
						userView = userView,
						userShippingView = shippingView
					};
					ViewBag.Message = "noBilling";
					return View(view);
				}
			}
			else
				return RedirectToAction(nameof(CheckoutNewUser), new { message = "Sorry Something Went Wrong!" });
		}
		[HttpPost]
		public async Task<IActionResult> Payment(PaymentModel model)
		{
			WebServices services = new(_context, _httpContextAccessor);
			var userModel = CookieTime();
			var cartModel = services.LoadCart(userModel!.UserId);
			if (userModel != null && !userModel.Email!.Contains("@wazaware.co.za"))
			{
				model.PaymentMethod = "EFT";
				model.UserId = userModel.UserId;				
				_context.PaymentModels.Attach(model);
				_context.PaymentModels.Add(model);
	
				var newOrder = new Orders
				{
					UserId = userModel.UserId,
					PaymentId = model.Id,
					ShippingPrice = 300,
					OrderTotal = cartModel.Select(s => s.CartSaleTotalFormatted).First(),
					isOrderPayed = false,
					OrderCreatedOn = DateTime.Now
				};
				_context.Orders.Attach(newOrder);
				_context.Orders.Add(newOrder);
				var newOrderProducts = cartModel.Select(s => new OrderProducts
				{
					OrderId = newOrder.UserId,
					ProductId = s.ProductId,
					ProductCount = s.ProductCount
				}).ToList();
				_context.AttachRange(newOrderProducts);
				_context.OrderProducts.AddRange(newOrderProducts);

								
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Payment), new { message = "200" });
			}
			else
			{
				return RedirectToAction(nameof(CheckoutNewUser), new { message = "Sorry Something Went Wrong!" });
			}
		}
		[HttpPost]
		public async Task SendEmail(ViewModels viewModel)
		{	
			EmailerService emailerService = new EmailerService(_httpContextAccessor, _viewEngine, _tempData);
			await emailerService.SendEmail(viewModel);
		}
		public UserModel? CookieTime()
		{
			WebServices services = new(_context, _httpContextAccessor);
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
				model = _context.Users
					.Where(x => x.Email == email).FirstOrDefault();
			}
			return model;
		}
	}

}
