using System.ComponentModel.DataAnnotations;

namespace wazaware.co.za.Models
{
	public class UserShoppingCart
	{
		[Key]
		public int CartId { get; set; }

		[Required]
		public int UserId { get; set; }

		[Required]
		public int ProductId { get; set; }
		[Required]
		public int ProductCount { get; set; }
		public decimal? ProductTotal { get; set; }
		[Required]
		public DateTime CartEntryDate { get; set; }

	}
}
