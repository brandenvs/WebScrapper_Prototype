using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using WazaWare.co.za.Models;

namespace wazaware.co.za.Models
{	
	public class PaymentModel
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public int UserId { get; set; }
		[Required] 
		public string PaymentMethod { get; set; }

		[Required]
		[MaxLength(50)]
		public string FirstName { get; set; }

		[Required]
		[MaxLength(50)]
		public string LastName { get; set; }
		[Required]
		[Phone]
		public string Phone { get; set; }

		[Required]
		[EmailAddress]
		public string Email { get; set; }

		public int UnitNo { get; set; }

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
		public int PostalCode { get; set; }

	}
	public class UserShippingViewModelPayment
	{
		[Required]
		public string FirstName { get; set; }
		[Required]
		public string MiddleName { get; set; }
		[Required]
		public string LastName { get; set; }
		[Required]
		public string Phone { get; set; }
		[Required]
		public string Email { get; set; }
		public int UnitNo { get; set; }
		[Required]
		public string StreetAddress { get; set; }
		[Required]
		public string Suburb { get; set; }
		[Required]
		public string City { get; set; }
		[Required]
		public string Province { get; set; }
		[Required]
		public int PostalCode { get; set; }
		public string Notes { get; set; }

	}
	public class OrderSummary
	{
		public int OrderId { get; set; }
		public string PaymentMethod { get; set; }
		public string OrderTotal { get; set; }

	}
	public class PaymentView
	{
		[Required]
		public string PaymentMethod { get; set; }

		[Required]
		[MaxLength(50)]
		public string FirstName { get; set; }

		[Required]
		[MaxLength(50)]
		public string LastName { get; set; }
		[Required]
		[Phone]
		public string Phone { get; set; }

		[Required]
		[EmailAddress]
		public string Email { get; set; }

		[MaxLength(50)]
		public int UnitNo { get; set; }

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
		public int PostalCode { get; set; }
	}

}
