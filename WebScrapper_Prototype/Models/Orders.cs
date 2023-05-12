using System.ComponentModel.DataAnnotations;

namespace WazaWare.co.za.Models
{
	public class Orders
	{
		[Key]
		public int OrderId { get; set; }
		[Required]
		public int UserId { get; set; }
		[Required]
		public int? PaymentId { get; set; }
		[Required]
		public int ProductId { get; set; }
		public int ProductCount { get; set; }
		[Required]
		public int? ShippingPrice { get; set; }
		[Required]
		public string? OrderTotal { get; set; }
		[Required]
		public Boolean isOrderPayed { get; set; }
		[Required]
		public DateTime? OrderCreatedOn { get; set; }

	}
}
