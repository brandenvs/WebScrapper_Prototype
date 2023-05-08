using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using wazaware.co.za.Models;
using WazaWare.co.za.Services;
using X.PagedList;
using static NuGet.Packaging.PackagingConstants;

namespace WazaWare.co.za.Models
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

		public string? ProductPriceBaseFormatted => ProductPriceBase?.ToString("C", CultureInfo.CreateSpecificCulture("en-ZA"));
		public string? ProductPriceSaleFormatted => ProductPriceSale?.ToString("C", CultureInfo.CreateSpecificCulture("en-ZA"));
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

		public string? ProductPriceBaseFormatted => ProductPriceBase?.ToString("C", CultureInfo.CreateSpecificCulture("en-ZA"));
		public string? ProductPriceSaleFormatted => ProductPriceSale?.ToString("C", CultureInfo.CreateSpecificCulture("en-ZA"));
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

		public string? ProductPriceBaseFormatted => ProductPriceBase?.ToString("C", CultureInfo.CreateSpecificCulture("en-ZA"));
		public string? ProductPriceSaleFormatted => ProductPriceSale?.ToString("C", CultureInfo.CreateSpecificCulture("en-ZA"));
	}
	public class ProductsInCartModel
	{
		[Key]
		public int ProductId { get; set; }
		public string? ProductName { get; set; }
		public decimal? ProductPriceBase { get; set; }
		public decimal? ProductPriceSale { get; set; }
		public string? ProductImageUrl { get; set; }
		public int ProductCount { get; set; }
		public decimal? ProductTotal { get; set; }
		public decimal? ProductBaseSaleDiff { get; set; }
		public decimal? CartBaseTotal { get; set; }
		public decimal? CartSaleTotal { get; set; }
		public decimal? CartBaseSaleDiff { get; set; }
		public decimal? ShippingCost { get; set; }
		public decimal? TryGetFreeShipping { get; set; }
		public IFormFile ProductPic { get; set; }

		public string? TryGetFreeShippingFormatted => TryGetFreeShipping?.ToString("C", CultureInfo.CreateSpecificCulture("en-ZA"));
		public string? ProductBaseSaleDiffFormatted => ProductBaseSaleDiff?.ToString("C", CultureInfo.CreateSpecificCulture("en-ZA"));
		public string? ShippingCostFormatted => ShippingCost?.ToString("C", CultureInfo.CreateSpecificCulture("en-ZA"));
		public string? CartBaseTotalFormatted => CartBaseTotal?.ToString("C", CultureInfo.CreateSpecificCulture("en-ZA"));
		public string? CartSaleTotalFormatted => CartSaleTotal?.ToString("C", CultureInfo.CreateSpecificCulture("en-ZA"));
		public string? CartBaseSaleDiffFormatted => CartBaseSaleDiff?.ToString("C", CultureInfo.CreateSpecificCulture("en-ZA"));
		public string? ProductTotalFormatted => ProductTotal?.ToString("C", CultureInfo.CreateSpecificCulture("en-ZA"));
		public string? ProductPriceBaseFormatted => ProductPriceBase?.ToString("C", CultureInfo.CreateSpecificCulture("en-ZA"));
		public string? ProductPriceSaleFormatted => ProductPriceSale?.ToString("C", CultureInfo.CreateSpecificCulture("en-ZA"));
	}
	public class Filter_Sortby
	{
		public string FilterId { get; set; }
		public string FilterName { get; set; }
	}
	public class Filter_Manufacturer
	{
		public string FilterId { get; set; }
		public string FilterName { get; set; }
	}
	public class Filter_Category
	{
		public string FilterId { get; set; }
		public string FilterName { get; set; }
	}
	public class Filter_Active
	{
		public string FilterId { get; set; }
		public string FilterName { get; set; }
	}

	public class UserManagerViewModels
	{
		//
		public class UserModelView
		{
			[Required]
			public string FirstName { get; set; }
			public string LastName { get; set; }
			[Required]
			public string Email { get; set; }
			[Required]
			public string Phone { get; set; }
		}
		//
		public class UserLoginViewModel
		{
			[Required(ErrorMessage = "Email Address is required.")]
			[EmailAddress]
			public string? Email { get; set; }
			[Required]
			public string? Password { get; set; }
		}
		//
		public class UserRegisterViewModel
		{
			[Required]
			public string FirstName { get; set; }
			public string LastName { get; set; }
			[Required]
			public string Email { get; set; }
			[Required]
			public string Phone { get; set; }
			[Required]
			[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
			[DataType(DataType.Password)]
			[Display(Name = "Password")]
			public string? Password { get; set; }
			[Required]
			[DataType(DataType.Password)]
			[Display(Name = "Confirm password")]
			[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
			public string? ConfirmPassword { get; set; }

		}
		//
		public class UserShippingViewModel
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

			public string? Password { get; set; }

			public string? ConfirmPassword { get; set; }

		}
		//public UserLoginViewModel userLoginViewModel { get; set; }
		//public UserViewModel userViewModel { get; set; }
		//public UserRegisterViewModel userRegisterModel { get; set; }
		//public UserShippingViewModel userShippingInfo { get; set; }
	}
	public class ViewModels
	{
		public PaymentModel Payment { get; set; }
		public OrderSummary Summary { get; set; }
		public UserManagerViewModels.UserModelView userView { get; set; }
		public UserManagerViewModels.UserLoginViewModel userLoginView { get; set; }
		public UserManagerViewModels.UserRegisterViewModel userRegisterView { get; set; }
		public UserManagerViewModels.UserShippingViewModel userShippingView { get; set; }
		public IPagedList<Product> Products { get; set; }
		public IList<Filter_Sortby> FilterSortBy { get; set; }
		public IList<Filter_Manufacturer> FilterManufacturer { get; set; }
		public IList<Filter_Category> FilterCategory { get; set; }
		public Product Product { get; set; }
		public IPagedList<LatestArrivalModel> LatestArrivals { get; set; }
		public IPagedList<LimitedStockModel> LimitedStock { get; set; }
		public IPagedList<TrendingProductsModel> TrendingProducts { get; set; }
		public IList<ProductsInCartModel> Cart { get; set; }





	}
}
