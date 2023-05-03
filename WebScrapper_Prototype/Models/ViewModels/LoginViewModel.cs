using System.ComponentModel.DataAnnotations;

namespace WazaWare.co.za.Models
{
	public class LoginViewModel
	{
		[EmailAddress]
		public string? Email { get; set; }
		public string? Password { get; set; }
	}
}
