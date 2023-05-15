using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace wazaware.co.za.Models.DatabaseModels
{
	public class UserAccount
	{
		[Key]
		public int UserId { get; set; }
		[Required]
		public string? FirstName { get; set; }
		[Required]
		public string? LastName { get; set; }
		[Required]
		[EmailAddress]
		public string? Email { get; set; }
		[Required]
		[Phone]
		public string? Phone { get; set; }
		[Required]
		[DataType(DataType.Password)]
		public string? Password { get; set; }
		[Required]
		public DateTime Joined { get; set; }
	}
	public class ShoppingCart
	{
		[Key]
		public int CartId { get; set; }
		[ForeignKey(name: "UserId")]
		public int UserId { get; set; }
		[Required]
		public int ProductId { get; set; }
		[Required]
		public int ProductCount { get; set; }
		[Required]
		public decimal? ProductTotal { get; set; }
		[Required]
		public DateTime CartEntryDate { get; set; }
	}
	public class ShippingAddress
	{
		[Key]
		public int ShippingId { get; set; }
		[ForeignKey(name: "UserId")]
		public int UserId { get; set; }
		[Required]
		[MaxLength(50)]
		public string? FirstName { get; set; }

		[Required]
		[MaxLength(50)]
		public string? LastName { get; set; }
		[Required]
		[Phone]
		public string? Phone { get; set; }

		[Required]
		[EmailAddress]
		public string? Email { get; set; }

		[MaxLength(50)]
		public int UnitNo { get; set; }

		[Required]
		[MaxLength(100)]
		public string? StreetAddress { get; set; }

		[Required]
		[MaxLength(50)]
		public string? Suburb { get; set; }

		[Required]
		[MaxLength(50)]
		public string? City { get; set; }

		[Required]
		[MaxLength(50)]
		public string? Province { get; set; }
		[Required]
		[MaxLength(50)]
		public int PostalCode { get; set; }
	}
}