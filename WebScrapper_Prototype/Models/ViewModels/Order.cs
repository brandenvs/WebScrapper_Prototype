using System.Globalization;

namespace wazaware.co.za.Models.ViewModels
{
	public class OrderView
	{
		public int OrderId { get; set; }
		public int? ShippingPrice { get; set; }
		public decimal? OrderTotal { get; set; }
		public Boolean IsOrderPayed { get; set; }
		public DateTime? OrderCreatedOn { get; set; }
	}
	public class OrderedProductsView
	{
		public int Id { get; set; }
		public int OrderId { get; set; }
		public int ProductId { get; set; }
		public int ProductCount { get; set; }
		public decimal? ProductTotal { get; set; }
	}
	public class ShippingAddressView
	{
		public string? FirstName { get; set; }
		public string? LastName { get; set; }
		public string? Phone { get; set; }
		public string? Email { get; set; }
		public int UnitNo { get; set; }
		public string? StreetAddress { get; set; }
		public string? Suburb { get; set; }
		public string? City { get; set; }
		public string? Province { get; set; }
		public int PostalCode { get; set; }
		public string? Notes { get; set; }

	}
	public class BillingAddressView
	{
		public string? PaymentMethod { get; set; }
		public string? FirstName { get; set; }
		public string? LastName { get; set; }
		public string? Phone { get; set; }
		public string? Email { get; set; }
		public int UnitNo { get; set; }
		public string? StreetAddress { get; set; }
		public string? Suburb { get; set; }
		public string? City { get; set; }
		public string? Province { get; set; }
		public int PostalCode { get; set; }
	}
	public class OrderCartView
	{
		public int OrderId { get; set; }
		public int ProductId { get; set; }
		public string? ProductName { get; set; }
		public decimal? ProductPriceBase { get; set; }
		public decimal? ProductPriceSale { get; set; }
		public string? ProductImageUrl { get; set; }
		public int ProductCount { get; set; }
		public decimal? ProductTotal { get; set; }
		public decimal? ProductBaseSaleDiff { get; set; }
		public decimal? OrderSaleTotal { get; set; }
		public decimal? ShippingCost { get; set; }
		public IFormFile? ProductPic { get; set; }

		public string? ProductBaseSaleDiffFormatted =>
			ProductBaseSaleDiff?.ToString("C", CultureInfo.CreateSpecificCulture("en-ZA"));
		public string? ShippingCostFormatted =>
			ShippingCost?.ToString("C", CultureInfo.CreateSpecificCulture("en-ZA"));
		public string? CartSaleTotalFormatted =>
			OrderSaleTotal?.ToString("C", CultureInfo.CreateSpecificCulture("en-ZA"));
		public string? ProductTotalFormatted =>
			ProductTotal?.ToString("C", CultureInfo.CreateSpecificCulture("en-ZA"));
		public string? ProductPriceBaseFormatted =>
			ProductPriceBase?.ToString("C", CultureInfo.CreateSpecificCulture("en-ZA"));
		public string? ProductPriceSaleFormatted =>
			ProductPriceSale?.ToString("C", CultureInfo.CreateSpecificCulture("en-ZA"));
	}
	public class OrderViewModel
	{
		public IList<ProductInfomation>? Products { get; set; }
		public IList<OrderView>? Orders { get; set; }
		public IList<OrderedProductsView>? OrderedProducts { get; set; }
		public OrderView? Order { get; set; }
		public ShippingAddressView? ShippingAddress { get; set; }
		public BillingAddressView? BillingAddress { get; set; }
		public UserView? User { get; set; }
		public IList<ShoppingCartView>? ShoppingCart { get; set; }
		public IList<OrderCartView>? OrderCartView { get; set; }
		public PartialView? PartialView { get; set; }

	}

}