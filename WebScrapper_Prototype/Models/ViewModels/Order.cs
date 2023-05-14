using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using static wazaware.co.za.Models.ViewModels.ProductView;
using static wazaware.co.za.Models.ViewModels.User;

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
	}

}
