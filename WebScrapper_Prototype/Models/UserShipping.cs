using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WazaWare.co.za.Models
{
	public class UserShipping
	{
		[Key]
		public int UserShippingId { get; set; }

		[Required]
		public int UserId { get; set; }

		[Required]
		[MaxLength(50)]
		public string FirstName { get; set; }

		[MaxLength(50)]
		public string MiddleName { get; set; }

		[Required]
		[MaxLength(50)]
		public string LastName { get; set; }

		[MaxLength(50)]
		public string UnitNo { get; set; }

		[Required]
		[MaxLength(100)]
		public string StreetAddress { get; set; }

		[Required]
		[MaxLength(50)]
		public string Suburb { get; set; }

		[Required]
		[MaxLength(50)]
		public string City { get; set; }

		[Required]
		[MaxLength(50)]
		public string Province { get; set; }
		[Required]
		[MaxLength(50)]
		public string PostalCode { get; set; }

		[Required]
		[Phone]
		public string Phone { get; set; }

		[Required]
		[EmailAddress]
		public string Email { get; set; }
	}
}
