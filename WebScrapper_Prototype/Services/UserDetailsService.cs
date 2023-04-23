using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using wazaware.co.za.DAL;
using wazaware.co.za.Models;

namespace wazaware.co.za.Services
{
	public class UserDetailsService
	{
		private readonly wazaware_db_context? _context;
		private readonly IHttpContextAccessor? _httpContextAccessor;
		private string emailPersis = string.Empty;

		public UserDetailsService(wazaware_db_context context, IHttpContextAccessor httpContextAccessor)
		{
			_context = context;
			_httpContextAccessor = httpContextAccessor;
		}
		public async Task<int> Login(LoginViewModel model)
		{
			//var user = await _signInManager!.UserManager.FindByEmailAsync(model.Email);
			//if (user != null)
			//{
			//	var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
			//	if (result.Succeeded)
			//	{
			//		var claims = new List<Claim>
			//		{
			//			new Claim("arr", "pwd"),
			//		};
			//		var roles = await _signInManager.UserManager.GetRolesAsync(user);
			//		if (roles.Any())
			//		{
			//			//Gives Cookie [USER] Role
			//			var roleClaim = string.Join(",", roles);
			//			claims.Add(new Claim("Roles", roleClaim));
			//		}
			//		else
			//		{
			//			await _signInManager.UserManager.AddToRoleAsync(user, "User");
			//			var roleClaim = string.Join(",", roles);
			//			claims.Add(new Claim("Roles", roleClaim));
			//		}
			//		await _signInManager.SignInWithClaimsAsync(user, true, claims);
			//		return 200;
			//	}
			//	else
			//		return 500;
			//}
			//else
			//	return 404;
			return 0;
		}
		public async Task<int> Register(UserModel model, string token)
		{
			//var user = CreateUser();
			//user.FirstName = model.FirstName;
			//user.LastName = model.LastName;
			//user.Email = model.Email;
			//user.PhoneNumber = model.Phone;
			//model.Joined = DateTime.Now;
			//user.Joined = DateTime.Now;
			//user.UserName = model.Email;
			//IdentityUser identity = user;

			//var existingUser = _app!.Users.FirstOrDefault(u => u.Email == model.Email);
			//if (existingUser != null && !existingUser.Email.Contains("cookie"))
			//	return 100;
			//else
			//{
			//	var resultCreate = await _userManager!.CreateAsync(user, model.Password);
			//	Console.WriteLine(resultCreate.Errors);
			//	if (resultCreate.Succeeded)
			//	{
			//		Console.WriteLine("USER CREATED!!");
			//		_context!.Attach(model);
			//		_context.Entry(model).State = EntityState.Added;
			//		await _context.SaveChangesAsync();

			//		if (token != null)
			//		{
			//			var basketProducts = _context!.UsersShoppingCarts.Where(s => s.UserId.Equals(token));/// Start-->
			//			if (basketProducts != null)
			//			{
			//				foreach (var product in basketProducts)
			//					product.UserId = model.Email!;
			//				/// Save Changes
			//				_context.SaveChanges();

			//			}/// -->END

			//			 /// Remove: oldUser
			//			var oldUser = await _userManager!.FindByEmailAsync(token);/// Start-->
			//			if (oldUser != null)
			//			{
			//				_app.Attach(oldUser);
			//				_app.Remove(oldUser);
			//				/// Save Changes
			//				_app.SaveChanges();
			//				/// -->END
			//			}
			//			/// SignIn -> NewUser
			//			user = await _signInManager!.UserManager.FindByEmailAsync(model.Email);
			//		}
			//	}
			//}
			//var result = await _signInManager!.CheckPasswordSignInAsync(user, model.Password, false);
			//if (result.Succeeded)
			//{
			//	var claims = new List<Claim>
			//		{
			//			new Claim("arr", "pwd"),
			//		};
			//	var roles = await _signInManager.UserManager.GetRolesAsync(user);
			//	if (roles.Any())
			//	{
			//		//Gives Cookie [USER] Role
			//		var roleClaim = string.Join(",", roles);
			//		claims.Add(new Claim("Roles", roleClaim));
			//	}
			//	else
			//	{
			//		await _signInManager.UserManager.AddToRoleAsync(user, "User");
			//		var roleClaim = string.Join(",", roles);
			//		claims.Add(new Claim("Roles", roleClaim));
			//	}
			//	await _signInManager.SignInWithClaimsAsync(user, true, claims);
			//	return 200;
			//}
			//else
			//	return 300;
			return 0;
		}
		public async Task<Dictionary<int, string>> EntireUser(UserViewModel model, string token)
		{
			//var result = new Dictionary<int, string>();
			//var existingUser = _app!.Users.FirstOrDefault(u => u.Email == model.Email);
			//var userShipping = _context!.UserShippings.FirstOrDefault(u => u.UserId == model.Email);

			//if (existingUser != null)
			//	result.Add(500, "A User Already is Exists with the Email Address: " + model.Email);
			//else
			//{
			//	var user = new UserModel
			//	{
			//		FirstName = model.FirstName,
			//		LastName = model.LastName,
			//		Email = model.Email,
			//		Phone = model.PhoneNumber,
			//		Password = model.Password,
			//	};
			//	int resultLocal = await Register(user, token);
			//	switch (resultLocal)
			//	{
			//		case 500:
			//			result.Add(500, "A User Already is Exists with the Email Address: " + model.Email);
			//			break;
			//		case 200:
			//			userShipping = new UserShipping
			//			{
			//				UserId = model.Email,
			//				Province = model.Province,
			//				City = model.City,
			//				PostalCode = model.PostalCode,
			//				Unit = model.Unit,
			//				Street = model.Street,
			//				Area = model.Area,
			//			};
			//			await ShippingUser(userShipping, userShipping.UserId!);
			//			await SignIn(userShipping.UserId!);
			//			result.Add(200, "User Created with Email Address: " + model.Email);
			//			break;
			//		case 300:
			//			result.Add(300, "300: Something Went Wrong!");
			//			break;
			//		default:
			//			result.Add(0, "Page not found!");
			//			break;
			//	}
			//}
			var result = new Dictionary<int, string>();
			return result;
		}
		//public async Task ShippingUser(UserShipping model, string token)
		//{
		//	var existingUser = _app!.Users.FirstOrDefault(u => u.Email == token);
		//	var userShipping = _context!.UserShippings.FirstOrDefault(u => u.UserId == token);

		//	if (existingUser != null)
		//	{
		//		if (userShipping == null)
		//		{
		//			userShipping = new UserShipping
		//			{
		//				UserId = token,
		//				Province = model.Province,
		//				City = model.City,
		//				PostalCode = model.PostalCode,
		//				Unit = model.Unit,
		//				Street = model.Street,
		//				Area = model.Area,
		//			};
		//			_context.Attach(userShipping);
		//			_context.Entry(userShipping).State = EntityState.Added;
		//			await _context.SaveChangesAsync();
		//		}
		//	}
		//}

		public async Task<string> SignIn(string token)
		{
			///// Handle Null Exceptions
			//if (_signInManager == null || _userManager == null || _signInManager.UserManager == null)
			//{
			//	return "--------------------------------------------------ERROR: httpContextAccessor is NULL!!";
			//}
			///// Handle Null Exceptions
			//var userA = await _signInManager.UserManager.FindByEmailAsync(token);
			//if (userA != null)
			//{
			//	var result = await _signInManager.CheckPasswordSignInAsync(userA, "3%@D8Iy2?Kt7*ceK", false);
			//	if (result.Succeeded)
			//	{
			//		var claims = new List<Claim>
			//			{
			//				new Claim("arr", "pwd"),
			//			};
			//		var roles = await _signInManager.UserManager.GetRolesAsync(userA);
			//		if (roles.Any())
			//		{
			//			//Gives Cookie [USER] Role
			//			var roleClaim = string.Join(",", roles);
			//			claims.Add(new Claim("Roles", roleClaim));
			//		}
			//		else
			//		{
			//			await _signInManager.UserManager.AddToRoleAsync(userA, "User");
			//			await _userManager.UpdateAsync(userA);
			//			var roleClaim = string.Join(",", roles);
			//			claims.Add(new Claim("Roles", roleClaim));
			//		}
			//		await _signInManager.SignInWithClaimsAsync(userA, true, claims);
			//	}
			//}
			//else
			//	await Cookie();
			//return "Successfully SignedIn!";
			return string.Empty;
		}
	}
}
