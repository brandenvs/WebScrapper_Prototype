using Microsoft.EntityFrameworkCore;
using WazaWare.co.za.DAL;
using WazaWare.co.za.Models;

namespace WazaWare.co.za.Services
{
	public class UserManagerService
	{
		private readonly WazaWare_db_context _context;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public UserManagerService(WazaWare_db_context context, IHttpContextAccessor httpContextAccessor)
		{
			_context = context;
			_httpContextAccessor = httpContextAccessor;
		}
		public async Task<int> CreateCookieReferance()
		{
			string generatedName = "WazaWareCookie6.542-FirstName";
			string generatedEmail = "WazaWareCookie6.542-Email" + Guid.NewGuid().ToString() + "@WazaWare.co.za";
			string generatedPhone = "1234567890";
			string generatedPwd = "3%@D8Iy2?Kt7*ceK";

			var userModel = new UserModel
			{
				FirstName = generatedName,
				LastName = generatedName,
				Email = generatedEmail,
				Phone = generatedPhone,
				Password = generatedPwd,
				Joined = DateTime.Now
			};
			_context!.Users.Add(userModel);
			_context!.SaveChanges();
			var user = _context!.Users.Where(s => s.Email!.Equals(generatedEmail)).FirstOrDefault();
			return user!.UserId;
		}
		public async Task<UserModel?> GetCurrentUserModel(int userId)
		{
			var user = await _context!.Users.Where(s => s.UserId == userId).FirstOrDefaultAsync();
			return user;
		}
	}
}
