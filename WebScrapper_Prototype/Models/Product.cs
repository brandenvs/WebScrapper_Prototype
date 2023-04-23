using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using X.PagedList;

namespace wazaware.co.za.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
		[Required]
		public string? ProductName { get; set; }
		[Required]
		public string? ProductStock { get; set; }
		[Required]
		public string? ProductDescription { get; set; }
		[Required]
		public string? ProductCategory { get; set; }
		[Required]
		public decimal? ProductPriceBase { get; set; }
		[Required]
		public decimal? ProductPriceSale { get; set; }
		[Required]
        public string? ProductVendorName { get; set; }
        [Required]
        public string? ProductVendorUrl { get; set; }
        [Required]
        public string? ProductVisibility { get; set; }
        [Required]
        public string? ProductDataBatchNo { get; set; }
		[Required]
		public string? ProductImageUrl { get; set; }
		[NotMapped]
        public IFormFile ProductPic { get; set; }
    }
	// Define LatestArrivalModel
	public class LatestArrivalModel
	{
		[Key]
		public int ProductId { get; set; }
		[Required]
		public string? ProductName { get; set; }
		[Required]
		public string? ProductStock { get; set; }
		[Required]
		public string? ProductDescription { get; set; }
		[Required]
		public string? ProductCategory { get; set; }
		[Required]
		public decimal? ProductPriceBase { get; set; }
		[Required]
		public decimal? ProductPriceSale { get; set; }
		[NotMapped]
		public IFormFile ProductPic { get; set; }
	}
	// Define LimitedStockModel
	public class LimitedStockModel
	{
		[Key]
		public int ProductId { get; set; }
		[Required]
		public string? ProductName { get; set; }
		[Required]
		public string? ProductStock { get; set; }
		[Required]
		public string? ProductDescription { get; set; }
		[Required]
		public string? ProductCategory { get; set; }
		[Required]
		public decimal? ProductPriceBase { get; set; }
		[Required]
		public decimal? ProductPriceSale { get; set; }
		[NotMapped]
		public IFormFile ProductPic { get; set; }
	}
	// Define TrendingProductsModel
	public class TrendingProductsModel
	{
		[Key]
		public int ProductId { get; set; }
		[Required]
		public string? ProductName { get; set; }
		[Required]
		public string? ProductStock { get; set; }
		[Required]
		public string? ProductDescription { get; set; }
		[Required]
		public string? ProductCategory { get; set; }
		[Required]
		public decimal? ProductPriceBase { get; set; }
		[Required]
		public decimal? ProductPriceSale { get; set; }
		[NotMapped]
		public IFormFile ProductPic { get; set; }
	}
	public class ProductsViewModel
	{
		public IPagedList<Product> Products { get; set; }
		public Product Product { get; set; }
		public IPagedList<LatestArrivalModel> LatestArrivals { get; set; }
		public IPagedList<LimitedStockModel> LimitedStock { get; set; }
		public IPagedList<TrendingProductsModel> TrendingProducts { get; set; }
		public IList<Product> Cart { get; set; }
	}
}
