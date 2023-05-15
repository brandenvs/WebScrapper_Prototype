using System.Globalization;
using X.PagedList;

namespace wazaware.co.za.Models.ViewModels
{
    public class LatestArrival
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductStock { get; set; }
        public string? ProductDescription { get; set; }
        public string? ProductCategory { get; set; }
        public decimal? ProductPriceBase { get; set; }
        public decimal? ProductPriceSale { get; set; }
        public IFormFile? ProductPic { get; set; }
        public string? ProductPriceBaseFormatted =>
            ProductPriceBase?.ToString("C", CultureInfo.CreateSpecificCulture("en-ZA"));
        public string? ProductPriceSaleFormatted =>
            ProductPriceSale?.ToString("C", CultureInfo.CreateSpecificCulture("en-ZA"));
    }
    // Define LimitedStockModel
    public class LimitedStock
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductStock { get; set; }
        public string? ProductDescription { get; set; }
        public string? ProductCategory { get; set; }
        public decimal? ProductPriceBase { get; set; }
        public decimal? ProductPriceSale { get; set; }
        public IFormFile? ProductPic { get; set; }
        public string? ProductPriceBaseFormatted =>
            ProductPriceBase?.ToString("C", CultureInfo.CreateSpecificCulture("en-ZA"));
        public string? ProductPriceSaleFormatted =>
            ProductPriceSale?.ToString("C", CultureInfo.CreateSpecificCulture("en-ZA"));
    }
    // Define TrendingProductsModel
    public class TrendingProduct
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductStock { get; set; }
        public string? ProductDescription { get; set; }
        public string? ProductCategory { get; set; }
        public decimal? ProductPriceBase { get; set; }
        public decimal? ProductPriceSale { get; set; }
        public IFormFile? ProductPic { get; set; }
        public string? ProductPriceBaseFormatted =>
            ProductPriceBase?.ToString("C", CultureInfo.CreateSpecificCulture("en-ZA"));
        public string? ProductPriceSaleFormatted =>
            ProductPriceSale?.ToString("C", CultureInfo.CreateSpecificCulture("en-ZA"));
    }
    public class FilterSortby
    {
        public string? FilterId { get; set; }
        public string? FilterName { get; set; }
    }
    public class FilterManufacturer
    {
        public string? FilterId { get; set; }
        public string? FilterName { get; set; }
    }
    public class ShopViewModel
    {
        public IPagedList<ProductInfomation>? Products { get; set; }
        public ProductInfomation? SingleProduct { get; set; }
        public IList<FilterSortby>? FilterSortby { get; set; }
        public IList<FilterManufacturer>? FilterManufacturer { get; set; }
        public IPagedList<LatestArrival>? LatestArrivals { get; set; }
        public IPagedList<LimitedStock>? LimitedStocks { get; set; }
        public IPagedList<TrendingProduct>? TrendingProducts { get; set; }
        public IList<ShoppingCartView>? ShoppingCart { get; set; }
        public UserView? User { get; set; }
		public PartialView? PartialView { get; set; }
	}
}