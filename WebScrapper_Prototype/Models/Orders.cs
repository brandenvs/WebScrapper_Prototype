using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace wazaware.co.za.Models
{
	public class Orders
	{
		[Key]
		public int OrderId { get; set; }
		[Required]
		public int UserId { get; set; }
		[Required]
		public int? ProductId { get; set; }
		[Required]
		public decimal OrderTotalShipping { get; set; }
		[Required]
		public decimal OrderTotalHandlingFee { get; set; }
		[Required]
		public decimal OrderGrandTotal { get; set; }
		[Required]
		public DateTime? OrderCreatedOn { get; set; }
		
	}
}
