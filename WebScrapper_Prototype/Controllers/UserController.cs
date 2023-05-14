using Microsoft.AspNetCore.Mvc;
using wazaware.co.za.Services;
using WazaWare.co.za.DAL;
using WazaWare.co.za.Models;
using static WazaWare.co.za.Models.UserManagerViewModels;

namespace WazaWare.co.za.Controllers
{
	public class UserController : Controller
	{
		private readonly WazaWare_db_context _context;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public UserController(WazaWare_db_context context, IHttpContextAccessor httpContextAccessor)
		{
			_context = context;
			_httpContextAccessor = httpContextAccessor;
		}
		[HttpGet]
		public IActionResult Index()
		{
			WebServices services = new(_context, _httpContextAccessor);
			var user = services.LoadDbUser();
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
					return RedirectToAction(nameof(UserAccount));
				}
			}
			return RedirectToAction(nameof(Login));
		}
		[HttpGet]
		public IActionResult Login()
		{
			ViewBag.IsCookie = true;
			WebServices services = new(_context, _httpContextAccessor);
			var user = services.LoadDbUser();
			var cartModel = services.LoadCart(userModel!.UserId);
			var viewModel = new ShopViewModel
			{
				Cart = cartModel
			};
			return View(viewModel);
		}
		[HttpPost]
		public IActionResult Login(UserLoginViewModel loginModel)
		{
			ViewBag.IsCookie = true;
			WebServices services = new(_context, _httpContextAccessor);
			var user = services.LoadDbUser();
			var cartModel = services.LoadCart(userModel!.UserId);
			if (TryValidateModel(loginModel))
			{
				if (_context.UserAccountDb!.Any(u => u.Email == loginModel.Email))
				{
					var user = _context.UserAccountDb!.Where(u => u.Email == loginModel.Email).First();
					if (loginModel.Password != null)
					{
						if (loginModel.Password == user.Password)
						{
							UpdateUserCookie(user);
							return RedirectToAction(nameof(Index));
						}
						else
						{
							ViewBag.InvalidCredentials = "Invalid Credentials... Please Try Again!";
						}
					}
				}
				else
				{
					ViewBag.InvalidCredentials = "Invalid Email... Please Register!";
				}
			}
			var viewModel = new ShopViewModel
			{
				Cart = cartModel
			};
			return View(viewModel);
		}
		[HttpGet]
		public IActionResult Register()
		{
			ViewBag.IsCookie = true;
			WebServices services = new(_context, _httpContextAccessor);
			var user = services.LoadDbUser();
			var cartModel = services.LoadCart(userModel!.UserId);
			var viewModel = new ShopViewModel
			{
				Cart = cartModel
			};
			return View(viewModel);
		}
		[HttpPost]
		public async Task<IActionResult> Register(UserRegisterViewModel registerModel)
		{
			ViewBag.IsCookie = true;
			WebServices services = new(_context, _httpContextAccessor);
			var user = services.LoadDbUser();
			var cartModel = services.LoadCart(userModel!.UserId);

			if (_context.UserAccountDb.Any(u => u.Email!.Equals(registerModel.Email)))
			{
				ViewBag.Message = "'" + registerModel.Email + "'" + " is Already Taken!";
			}
			else
			{
				var user = new UserModel
				{
					FirstName = registerModel.FirstName,
					LastName = registerModel.LastName,
					Email = registerModel.Email,
					Phone = registerModel.Phone,
					Password = registerModel.Password,
					Joined = DateTime.Now
				};
				_context.Attach(user);
				_context.UserAccountDb.Add(user);
				await _context.SaveChangesAsync();
				UpdateUserCookie(user);
				return RedirectToAction(nameof(Index));
			}
			var viewModel = new ShopViewModel
			{
				Cart = cartModel
			};
			return View(viewModel);
		}
		[HttpGet]
		public IActionResult UpdateUser()
		{
			ViewBag.IsCookie = false;
			WebServices services = new(_context, _httpContextAccessor);
			var user = services.LoadDbUser();
			var cartModel = services.LoadCart(userModel!.UserId);
			var UserView = new UserModelView
			{
				FirstName = userModel.FirstName!,
				LastName = userModel.LastName!,
				Email = userModel.Email!,
				Phone = userModel.Phone!
			};
			var viewModel = new ShopViewModel
			{
				Cart = cartModel,
				UserView = UserView
			};
			return View(viewModel);
		}
		[HttpPost]
		public IActionResult UpdateUser(UserRegisterViewModel registerModel)
		{
			ViewBag.IsCookie = false;
			WebServices services = new(_context, _httpContextAccessor);
			var user = services.LoadDbUser();
			var updatedUser = new UserModel();
			bool changed = false;
			if (userModel!.FirstName != registerModel.FirstName)
			{
				changed = true;
				userModel.FirstName = registerModel.FirstName;
			}
			if (userModel.LastName != registerModel.LastName)
			{
				changed = true;
				userModel.LastName = registerModel.LastName;
			}
			if (userModel.Email != registerModel.Email)
			{
				changed = true;
				userModel.Email = registerModel.Email;
			}
			if (userModel.Phone != registerModel.Phone)
			{
				changed = true;
				userModel.Phone = registerModel.Phone;
			}
			if (userModel.Password != registerModel.Password)
			{
				changed = true;
				userModel.Password = registerModel.Password;
			}
			if (changed)
			{
				_context.UserAccountDb!.Attach(userModel);
				_context.UserAccountDb.Update(userModel);
				_context.SaveChanges();
				UpdateUserCookie(userModel);
			}
			return RedirectToAction(nameof(Index));
		}
		public IActionResult UserAccount()
		{
			ViewBag.IsCookie = false;
			WebServices services = new(_context, _httpContextAccessor);
			var user = services.LoadDbUser();
			var cartModel = services.LoadCart(userModel!.UserId);
			var UserView = new UserModelView
			{
				FirstName = userModel.FirstName!,
				LastName = userModel.LastName!,
				Email = userModel.Email!,
				Phone = userModel.Phone!
			};
			var viewModel = new ShopViewModel
			{
				Cart = cartModel,
				UserView = UserView
			};
			return View(viewModel);
		}
		[HttpGet]
		public IActionResult UserOrders()
		{
			ViewBag.IsCookie = false;
			WebServices services = new(_context, _httpContextAccessor);
			var user = services.LoadDbUser();
			var cartModel = services.LoadCart(userModel!.UserId);
			var userOrders = services.LoadOrders(userModel!.UserId);
			var UserView = new UserModelView
			{
				FirstName = userModel.FirstName!,
				LastName = userModel.LastName!,
				Email = userModel.Email!,
				Phone = userModel.Phone!
			};		

			var viewModel = new ShopViewModel
			{
				Cart = cartModel,
				UserView = UserView,
				OrderProducts = userOrders			

			};
			return View(viewModel);
		}
		public IActionResult UserShipping(UserShippingViewModel userModel, List<ProductsInCartModel> cartModel)
		{
			return RedirectToAction(nameof(Index));
		}
		public IActionResult Logout(int delete)
		{
			WebServices services = new(_context, _httpContextAccessor);
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
				if (delete > 0)
				{
					var user = services.LoadUser(intialRequest);
					_context.UserAccountDb!.Remove(user);
					_context.SaveChanges();
				}
				HttpContext.Response.Cookies.Delete(cookieName);
			}
			return RedirectToAction(nameof(Index));
		}
		public void UpdateUserCookie(UserModel userModel)
		{
			const string cookieName = "wazaware.co.za-auto-sign-in";
			var requestCookies = HttpContext.Request.Cookies;
			_ = requestCookies[cookieName];
			var cookieOptions = new CookieOptions
			{
				Expires = DateTimeOffset.Now.AddDays(7),
				IsEssential = true
			};
			if (!requestCookies.ContainsKey(cookieName))
			{
				HttpContext.Response.Cookies.Append(cookieName, userModel.Email!, cookieOptions);
			}
			else
			{
				HttpContext.Response.Cookies.Delete(cookieName);
				HttpContext.Response.Cookies.Append(cookieName, userModel.Email!, cookieOptions);
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
				if (_context.UserAccountDb!.Any(u => u.Email == intialRequest))
				{
					model = _context.UserAccountDb.Where(x => x.Email == intialRequest).FirstOrDefault();
				}
				else
				{
					var email = services.CreateCookieReferance().Result;
					HttpContext.Response.Cookies
						.Append(cookieName, email, cookieOptions);
					model = _context.UserAccountDb
						.Where(x => x.Email == email).FirstOrDefault();
				}
			}
			else
			{
				var email = services.CreateCookieReferance().Result;
				HttpContext.Response.Cookies
					.Append(cookieName, email, cookieOptions);
				model = _context.UserAccountDb!
					.Where(x => x.Email == email).FirstOrDefault();
			}
			return model;
		}
	}
}
