using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using WazaWare.co.za.Models;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using Microsoft.EntityFrameworkCore;
using WazaWare.co.za.DAL;
using WazaWare.co.za.Services;
using wazaware.co.za.Services;
using static WazaWare.co.za.Models.UserManagerViewModels;
using Microsoft.AspNetCore.Http;
using System.Web.Helpers;
using wazaware.co.za.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace WazaWare.co.za.Controllers
{
	public class CheckoutController : Controller
	{
		private readonly ILogger<ShopController> _logger;
		private readonly WazaWare_db_context _context;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private static UserModel? _user;
		private static Boolean _isCookieUser;

		public CheckoutController(ILogger<ShopController> logger, WazaWare_db_context context, IHttpContextAccessor httpContextAccessor)
		{
			_logger = logger;
			_context = context;
			_httpContextAccessor = httpContextAccessor;
		}
		[HttpGet]
		public async Task<IActionResult> Index()
		{
			
			var userModel = CookieTime();
			if (userModel!.Email!.Contains("@wazaware.co.za"))
			{
				if(_context.UsersShoppingCarts.Any(s => s.UserId == userModel.UserId))
				{
					return RedirectToAction(nameof(CheckoutNewUser));
				}
				else
				{
					return RedirectToAction("Index", "Shop", new { message = "Your Shopping Cart is Empty!" });
				}
			}
			else
			{
				return RedirectToAction(nameof(CheckoutCurrentUser));
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
			var viewModel = new ViewModels
			{
				Cart = cartModel
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
		public async Task<IActionResult> CheckoutCurrentUser()
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
		public async Task<IActionResult> PlaceOrder()
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
		public IActionResult Payment(string message)
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
					var order = _context.Orders.Where(s => s.UserId == userModel.UserId).First();
					var orderSummary = new OrderSummary
					{
						OrderId = order.OrderId,
						PaymentMethod = paymentModel!.PaymentMethod,
						OrderTotal = cartModel!.Select(s => s.CartSaleTotalFormatted!).First()
					};
					var userView = new UserModelView
					{
						FirstName = userModel.FirstName,
						LastName = userModel.LastName,
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
					return View(view);
				}
				else
				{
					var cartModel = services.LoadCart(userModel!.UserId);
					var shippingModel = _context.UserShippings.Where(s => s.UserId == userModel.UserId).First();
					var userView = new UserModelView
					{
						FirstName = userModel.FirstName,
						LastName = userModel.LastName,
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
			{
				return RedirectToAction(nameof(CheckoutNewUser), new { message = "Sorry Something Went Wrong!" });
			}
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
				var userCart = _context.UsersShoppingCarts.FirstOrDefault(u => u.UserId == userModel.UserId);
	
				var newOrder = new Orders
				{
					UserId = userModel.UserId,
					ProductId = userCart.ProductId,
					PaymentId = model.Id,
					ProductCount = userCart.ProductCount,
					ShippingPrice = 300,
					OrderTotal = cartModel.Select(s => s.CartSaleTotalFormatted).First(),
					isOrderPayed = false,
					OrderCreatedOn = DateTime.Now
				};
				_context.Orders.Attach(newOrder);
				_context.Orders.Add(newOrder);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Payment), new { message = "200" });
			}
			else
			{
				return RedirectToAction(nameof(CheckoutNewUser), new { message = "Sorry Something Went Wrong!" });
			}
		}
		[HttpPost]
		public async Task SendEmail(string userEmail, string fullName, decimal orderSubTotal, decimal shippingTotal, decimal fee, decimal orderGrandTotal, int orderId)
		{
			try
			{
				SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com", 587);
				SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
				MailMessage email = new MailMessage();
				// START
				email.From = new MailAddress("brandenconnected@gmail.com");
				email.To.Add(userEmail);
				email.CC.Add("brandenconnected@gmail.com");
				email.Subject = "SwiftLink Order Confirmation";
				email.Body = "" +
					"<!DOCTYPE html>" +
					"<html> " +
					"<head> " +
					"<meta charset=\"utf-8\"/> " +
					"<style>/* Styling for the header */ .header{display: flex; justify-content: space-between; align-items: center; padding: 20px;}.header img{height: 50px;}.header h1{margin: 0; font-size: 24px; color: #333;}/* Styling for the message */ .message{background-color: #f5f5f5; padding: 20px;}.message h2{margin-top: 0; font-size: 20px; color: #333;}.message p{margin: 0; font-size: 16px; line-height: 1.5; color: #666;}/* Styling for the banner */ .banner{text-align: center; padding: 20px;}.banner img{max-width: 100%;}/* Styling for the signature */ .signature{text-align: center; padding: 20px;}.signature p{margin: 0; font-size: 16px; line-height: 1.5; color: #666;}</style> " +
					"</head> " +
					"<body> " +
					"<div class=\"header\"> " +
					"<img src=\"/Media/LogoCropped.png\" alt=\"Logo\"/> " +
					"<h1>SwiftLink Order Confirmation</h1> </div><div class=\"message\"> " +
					"<h2>Thank You for Your Order!</h2> " +
					"<p> Dear <strong>" + fullName + "</strong>, " +
					"<br/> Thank you for placing your order with SwiftLink. We are now processing your order and it can take between 1 - 2 working days. " +
					"You will receive a shipping tracking number once your order is processed and on its way." +
					"</p><p>You can review your order by clicking on the following link: <a href='https://longdogechallenge.com/'>Order Details</a></p>" +
					"</div>" +
					"<div class=\"banner\"> " +
					"<img src=\"https://example.com/banner.png\" alt=\"Banner\"/> " +
					"</div>" +
					"<div class=\"signature\"> " +
					"<p>Kind regards,</p><p>The SwiftLink Team</p>" +
					"</div>" +
					"</body></html>";
				email.IsBodyHtml = true;
				//END
				SmtpServer.Timeout = 5000;
				SmtpServer.EnableSsl = true;
				SmtpServer.UseDefaultCredentials = false;
				SmtpServer.Credentials = new NetworkCredential("brandenconnected@gmail.com", "mueadqbombixceuk");
				await SmtpServer.SendMailAsync(email);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}

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
