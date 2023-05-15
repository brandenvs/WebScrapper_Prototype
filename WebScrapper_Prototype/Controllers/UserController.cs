using Microsoft.AspNetCore.Mvc;
using wazaware.co.za.Models.DatabaseModels;
using wazaware.co.za.Models.ViewModels;
using wazaware.co.za.Services;
using wazaware.co.za.DAL;

namespace wazaware.co.za.Controllers
{
	public class UserController : Controller
	{
		private readonly wazaware_db_context _DbContext;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public UserController(wazaware_db_context context, IHttpContextAccessor httpContextAccessor)
		{
			_DbContext = context;
			_httpContextAccessor = httpContextAccessor;
		}
		public IActionResult Index()
		{
			WebServices services = new(_DbContext, _httpContextAccessor);
			var user = services.LoadDbUser();
			bool isCookie = true;
			if (user != null)
			{
				if (!user.Email!.Contains("@wazaware.co.za"))
					isCookie = false;
			}
			if (isCookie)
				return RedirectToAction(nameof(Login));
			else
				return RedirectToAction(nameof(UserAccount));			
		}
		[HttpGet]
		public IActionResult Login()
		{
			ViewBag.IsCookie = true;
			// 
			WebServices services = new(_DbContext, _httpContextAccessor);
			// 
			var user = services.LoadDbUser();
			// 
			var cart = services.LoadCart(user!.UserId);
			var viewPartial = new PartialView
			{
				ShoppingCart = cart
			};
			var view = new UserViewModel
			{
				ShoppingCart = cart,
				PartialView = viewPartial
			};
			return View(view);
		}
		[HttpPost]
		public IActionResult Login(LoginUserView model)
		{
			ViewBag.IsCookie = true;
			WebServices services = new(_DbContext, _httpContextAccessor);
			var user = services.LoadDbUser();
			var cart = services.LoadCart(user!.UserId);
			if (TryValidateModel(model))
			{
				if (_DbContext.UserAccountDb!.Any(u => u.Email == model.Email))
				{
					var findUser = _DbContext.UserAccountDb!.Where(u => u.Email == model.Email).First();
					if (model.Password != null)
					{
						if (model.Password == findUser.Password)
						{
							services.UpdateLoadedUser(findUser.Email!);			
							return RedirectToAction(nameof(Index));
						}
						else
							ViewBag.InvalidCredentials = "Invalid Credentials... Please Try Again!";
					}
				}
				else
					ViewBag.InvalidCredentials = "Invalid Email... Please Register!";
			}
			var viewPartial = new PartialView
			{
				ShoppingCart = cart
			};
			var view = new UserViewModel
			{
				ShoppingCart = cart,
				PartialView = viewPartial
			};
			return View(view);
		}
		[HttpGet]
		public IActionResult Register()
		{
			ViewBag.IsCookie = true;
			WebServices services = new(_DbContext, _httpContextAccessor);
			var user = services.LoadDbUser();
			var cart = services.LoadCart(user!.UserId);
			var viewPartial = new PartialView
			{
				ShoppingCart = cart
			};
			var view = new UserViewModel
			{
				ShoppingCart = cart,
				PartialView = viewPartial
			};
			return View(view);
		}
		[HttpPost]
		public async Task<IActionResult> Register(RegisterUserView model)
		{
			ViewBag.IsCookie = true;
			WebServices services = new(_DbContext, _httpContextAccessor);
			var user = services.LoadDbUser();
			var cart = services.LoadCart(user!.UserId);

			if (_DbContext.UserAccountDb!.Any(u => u.Email!.Equals(model.Email)))
			{
				var findUser = _DbContext.UserAccountDb!.Where(u => u.Email == model.Email).First();
				services.UpdateLoadedUser(findUser.Email!);
				ViewBag.Message = "That Email Already has an Account! " +
					"Don't worry though: We have automatically signed you into your account!";
			}
			else
			{
				var newUser = new UserAccount
				{
					FirstName = model.FirstName,
					LastName = model.LastName,
					Email = model.Email,
					Phone = model.Phone,
					Password = model.Password,
					Joined = DateTime.Now
				};
				_DbContext.Attach(newUser);
				_DbContext.UserAccountDb!.Add(newUser);
				await _DbContext.SaveChangesAsync();
				var userRefreshed = services.LoadDbUser();
				services.UpdateLoadedUser(userRefreshed!.Email);
				return RedirectToAction(nameof(Index));
			}
			var viewPartial = new PartialView
			{
				ShoppingCart = cart
			};
			var view = new UserViewModel
			{
				ShoppingCart = cart,
				PartialView = viewPartial
			};
			return View(view);
		}
		[HttpGet]
		public IActionResult UpdateUser()
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
		[HttpPost]
		public IActionResult UpdateUser(RegisterUserView model)
		{
			ViewBag.IsCookie = false;
			WebServices services = new(_DbContext, _httpContextAccessor);
			var user = services.LoadDbUser();
			var updatedUser = new UserAccount
			{
				FirstName = model.FirstName,
				LastName = model.LastName,
				Email = model.Email,
				Phone = model.Phone,
				Password = model.Password
			};
			if (!updatedUser.Equals(user))
			{
				if (model.ConfirmPassword == updatedUser.Password)
				{
					_DbContext.UserAccountDb!.Attach(updatedUser);
					_DbContext.UserAccountDb!.Update(updatedUser);
					_DbContext.SaveChanges();
				}
			}		
			return RedirectToAction(nameof(Index));
		}
		[HttpGet]
		public IActionResult UserAccount()
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
		[HttpGet]
		public IActionResult UserOrders()
		{
			ViewBag.IsCookie = false;
			WebServices services = new(_DbContext, _httpContextAccessor);
			var user = services.LoadDbUser();
			var cart = services.LoadCart(user!.UserId);
			var orders = services.LoadOrderedProductsView(user!.UserId);

			var viewUserAccount = new UserView
			{
				FirstName = user!.FirstName!,
				LastName = user.LastName!,
				Email = user.Email!,
				Phone = user.Phone!
			};
			var view = new UserViewModel
			{
				ShoppingCart = cart,
				User = viewUserAccount,
				OrderedProducts = orders
			};
			return View(view);
		}
		[HttpPost]
		public ActionResult Logout(int delete)
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
			const string cookieName = "wazaware.co.za-auto-sign-in";
			var requestCookies = HttpContext.Request.Cookies;
			var intialRequest = requestCookies[cookieName];
			var cookieOptions = new CookieOptions
			{
				Expires = DateTimeOffset.Now.AddDays(7),
				IsEssential = true
			};
			if (isCookie == false)
			{
				HttpContext.Response.Cookies.Delete(cookieName);				
			}
			else
			{
				ViewBag.IsCookie = isCookie;
				_DbContext.UserAccountDb!.Attach(user!);
				_DbContext.UserAccountDb!.Remove(user!);
				_DbContext.SaveChanges();
				HttpContext.Response.Cookies.Delete(cookieName);
			}
			HttpContext.Response.Cookies.Delete(cookieName);
			return RedirectToAction(nameof(Index));		
		}
	
	}
}
