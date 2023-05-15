using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace wazaware.co.za.Models.ViewModels
{
    public class UserView
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
    }
    public class ShoppingCartView
    {
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
        public IFormFile? ProductPic { get; set; }
        public string? TryGetFreeShippingFormatted =>
            TryGetFreeShipping?.ToString("C", CultureInfo.CreateSpecificCulture("en-ZA"));
        public string? ProductBaseSaleDiffFormatted =>
            ProductBaseSaleDiff?.ToString("C", CultureInfo.CreateSpecificCulture("en-ZA"));
        public string? ShippingCostFormatted =>
            ShippingCost?.ToString("C", CultureInfo.CreateSpecificCulture("en-ZA"));
        public string? CartBaseTotalFormatted =>
            CartBaseTotal?.ToString("C", CultureInfo.CreateSpecificCulture("en-ZA"));
        public string? CartSaleTotalFormatted =>
            CartSaleTotal?.ToString("C", CultureInfo.CreateSpecificCulture("en-ZA"));
        public string? CartBaseSaleDiffFormatted =>
            CartBaseSaleDiff?.ToString("C", CultureInfo.CreateSpecificCulture("en-ZA"));
        public string? ProductTotalFormatted =>
            ProductTotal?.ToString("C", CultureInfo.CreateSpecificCulture("en-ZA"));
        public string? ProductPriceBaseFormatted =>
            ProductPriceBase?.ToString("C", CultureInfo.CreateSpecificCulture("en-ZA"));
        public string? ProductPriceSaleFormatted =>
            ProductPriceSale?.ToString("C", CultureInfo.CreateSpecificCulture("en-ZA"));
    }
    public class LoginUserView
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        [PasswordPropertyText]
        public string? Password { get; set; }
    }
    public class RegisterUserView
    {
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? LastName { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Phone { get; set; }
        [Required]
        public string? Password { get; set; }
        [Required]
        public string? ConfirmPassword { get; set; }
    }
    public class UserViewModel
    {
        public UserView? User { get; set; }		
		public LoginUserView? Login { get; set; }
		public RegisterUserView? Register { get; set; }
		public IList<ShoppingCartView>? ShoppingCart { get; set; }
        public IList<OrderView>? Orders { get; set; }
        public IList<OrderCartView>? OrderedProducts { get; set; }
		public PartialView? PartialView { get; set; }
	}
}
