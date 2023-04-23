using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace wazaware.co.za.Models
{
	[NotMapped]
	public class UserViewModel
	{
		[Required]
		public string? FirstName { get; set; }
		[Required]
		public string? LastName { get; set; }
		[Required]
		[EmailAddress]
		public string? Email { get; set; }
		[Required]
		[Phone]
		public string? PhoneNumber { get; set; }
		[Required]
		[NotMapped]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string? Password { get; set; }
		[Required]
		[DataType(DataType.Password)]
		[NotMapped]
		[Display(Name = "Confirm password")]
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		public string? ConfirmPassword { get; set; }
		[Required]
		public string? Province { get; set; }
		[Required]
		public string? City { get; set; }
		[Required]
		public string? PostalCode { get; set; }
		[Required]
		public string? Unit { get; set; }
		[Required]
		public string? Street { get; set; }
		[Required]
		public string? Area { get; set; }
	}
}
