using System.Globalization;

namespace wazaware.co.za.Models.ViewModels
{
	public class ProductInfomation
	{
		public int ProductId { get; set; }
		public string? ProductName { get; set; }
		public string? ProductStock { get; set; }
		public string? ProductDescription { get; set; }
		public string? ProductCategory { get; set; }
		public decimal? ProductPriceBase { get; set; }
		public decimal? ProductPriceSale { get; set; }
		public string? ProductImageUrl { get; set; }
		public IFormFile? ProductPic { get; set; }
		public string? ProductPriceBaseFormatted =>
			ProductPriceBase?.ToString("C", CultureInfo.CreateSpecificCulture("en-ZA"));
		public string? ProductPriceSaleFormatted =>
			ProductPriceSale?.ToString("C", CultureInfo.CreateSpecificCulture("en-ZA"));
	}
}