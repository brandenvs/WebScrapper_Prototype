using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WazaWare.co.za.DAL;
using WazaWare.co.za.Models;

namespace WazaWare.co.za.Controllers
{
	public class UserController : Controller
	{
		private readonly WazaWare_db_context _context;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private string userEmail = string.Empty;
		private static string userFirstName;

		public UserController(WazaWare_db_context context, IHttpContextAccessor httpContextAccessor)
		{
			_context = context;
			_httpContextAccessor = httpContextAccessor;
		}
		[HttpGet]
		public async Task<IActionResult> Index(string email, int result, string test)
		{
			getUserCookie();
			if(result > 0)
				ViewBag.Result = result;
			if (!userEmail!.Contains("cookie") && email == null)
			{
				ViewBag.UserLayer = 0;
				ViewBag.IsCookie = false;
				var user = _context.Users.FirstOrDefault(u => u.Email!.Equals(userEmail));
				var userShipping = _context.UserShippings.FirstOrDefault(u => u.UserId!.Equals(userEmail));
				userFirstName = user!.FirstName!;
				ViewBag.FirstName = userFirstName;

				if (userShipping != null)
				{
					var view = new UserViewModel
					{
						FirstName = user.FirstName,
						LastName = user.LastName,
						Email = user.Email,
						PhoneNumber = user.Phone,
						Unit = userShipping.UnitNo,
						Street = userShipping.StreetAddress,
						Area = userShipping.Suburb,
						City = userShipping.City,
						Province = userShipping.Province,
						PostalCode = userShipping.PostalCode
					};
					return View(view);
				}
				else
				{
					var view = new UserViewModel
					{
						FirstName = user!.FirstName,
						LastName = user.LastName,
						Email = user.Email,
						PhoneNumber = user.Phone,
						Province = "No Information",
						City = "No Information",
						PostalCode = "No Information",
						Unit = "No Information",
						Street = "No Information",
						Area = "No Information"

					};
					return View(view);
				}
			}
			else if (userEmail!.Contains("cookie") && email == null)
			{
				ViewBag.UserLayer = 1;
				ViewBag.IsCookie = true;
				ViewBag.FirstName = "Please Login or Register";
				return View();
			}				
			else if(email != null)
			{
				ViewBag.UserLayer = 2;
				ViewBag.IsCookie = true;
				ViewBag.Result = email;
				return View();
			}
			else
				return View();
		}
		//[HttpPost]
		//public async Task<ActionResult> Register(UserModel model)
		//{
		//	UserDetailsService service = new(_context, _httpContextAccessor, _app, _userManager, _signInManager);
		//	int result = 0;
		//	const string cookieName = "WazaWarecookie6";
		//	var requestCookies = HttpContext.Request.Cookies;
		//	var cookieOptions = new CookieOptions
		//	{
		//		Expires = DateTimeOffset.Now.AddDays(7),
		//		IsEssential = true
		//	};
		//	var firstRequest = requestCookies[cookieName];
		//	if (ModelState.IsValid)
		//	{
		//		result = await service.Register(model, userEmail);
		//		if (result.Equals(100))
		//			return RedirectToAction("Index", new { email = model.Email });

		//		await CheckUserCookie();
		//	}
		//	else
		//		Console.WriteLine("ERROR!");
		//	return RedirectToAction(nameof(Index));
		//}
		//[HttpPost]
		//public async Task<ActionResult> UserUpdateContact(UserModel model)
		//{
		//	userEmail = await CheckUserCookie();
		//	var dbModel = _context.Users.FirstOrDefault(s => s.Email!.Equals(userEmail));
		//	if (dbModel != null)
		//	{
		//		// check if the model contains changes
		//		if (model.FirstName != dbModel.FirstName || model.LastName != dbModel.LastName
		//			|| model.Phone != dbModel.Phone)
		//		{
		//			// update only the changed columns
		//			dbModel.FirstName = model.FirstName ?? dbModel.FirstName;
		//			dbModel.LastName = model.LastName ?? dbModel.LastName;
		//			dbModel.Phone = model.Phone ?? dbModel.Phone;

		//			// save changes to database
		//			_context.Update(dbModel);
		//			await _context.SaveChangesAsync();
		//		}
		//	}
		//	return RedirectToAction(nameof(Index));
		//}
		//[HttpPost]
		//public async Task<ActionResult> UserUpdateShipping(UserShipping model)
		//{
		//	userEmail = await CheckUserCookie();
		//	var dbModel = _context.UserShippings.FirstOrDefault(s => s.UserId!.Equals(userEmail));
		//	if (dbModel != null)
		//	{
		//		// check if the model contains changes
		//		if (model.Province != dbModel.Province || model.City != dbModel.City
		//			|| model.PostalCode != dbModel.PostalCode || model.Unit != dbModel.Unit
		//			|| model.Street != dbModel.Street || model.Area != dbModel.Area)
		//		{
		//			// update only the changed columns
		//			dbModel.Province = model.Province ?? dbModel.Province;
		//			dbModel.City = model.City ?? dbModel.City;
		//			dbModel.PostalCode = model.PostalCode ?? dbModel.PostalCode;
		//			dbModel.Unit = model.Unit ?? dbModel.Unit;
		//			dbModel.Street = model.Street ?? dbModel.Street;
		//			dbModel.Area = model.Area ?? dbModel.Area;

		//			// save changes to database
		//			_context.Update(dbModel);
		//			await _context.SaveChangesAsync();
		//		}
		//	}
		//	return RedirectToAction(nameof(Index));
		//}		
		//public async Task<IActionResult> Login(LoginViewModel model)
		//{
		//	UserDetailsService service = new(_context, _httpContextAccessor, _app, _userManager, _signInManager);
		//	int result = 0;
		//	if (!String.IsNullOrEmpty(model.Password))
		//		result = await service.Login(model);
		//	switch (result)
		//	{					
		//		case 200:
		//			const string cookieName = "WazaWarecookie6";
		//			var requestCookies = HttpContext.Request.Cookies;
		//			var firstRequest = requestCookies[cookieName];
		//			var cookieOptions = new CookieOptions
		//			{
		//				Expires = DateTimeOffset.Now.AddDays(7),
		//				IsEssential = true
		//			};
		//			HttpContext.Response.Cookies.Append(cookieName, model.Email, cookieOptions);
		//			ViewBag.Result = "Successfully Logged In!";
		//			return RedirectToAction(nameof(Index), new { result});
		//		case 500:
		//			return RedirectToAction(nameof(Index), new { result });
		//		case 404:					
		//			return RedirectToAction(nameof(Index), new { result });
		//		default: return RedirectToAction(nameof(Index));
		//	}			
		//}
		//[HttpGet]
		//public async Task<IActionResult> Logout()
		//{
		//	await _signInManager.SignOutAsync();
		//	ViewBag.IsCookie = true;
		//	return RedirectToAction(nameof(Index));
		//}
		/// <summary>
		/// Responsible for Cookies
		/// </summary>
		public void getUserCookie()
		{
			const string cookieName = "WazaWarecookie6";
			var requestCookies = HttpContext.Request.Cookies;
			var intialRequest = requestCookies[cookieName];
			if (intialRequest != null)
			{
				foreach (var cookie in intialRequest)
				{
					Console.WriteLine("COOKIE!!!!!!!!!!!!!!!\n\n" + cookie + "\n\n");
					userEmail = intialRequest;
				}
			}
		}
	}
}
