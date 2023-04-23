using System.ComponentModel.DataAnnotations;

namespace wazaware.co.za.Models
{
	public class ProductImageURLs
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public string? ProductId { get; set; }
		[Required]
		public string? VendorSiteOrigin { get; set; }
		[Required]
		public string? VendorSiteProduct { get; set; }
		[Required]
		public string? ImageURL { get; set; }
	}
}
