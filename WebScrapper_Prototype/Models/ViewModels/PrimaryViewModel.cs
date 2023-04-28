using X.PagedList;

namespace wazaware.co.za.Models.ViewModels
{
	public class PrimaryModel
	{
		public class PrimaryProductInfoViewModel
		{
			public int ProductId { get; set; }
			public string? ProductName { get; set; }
			public decimal? ProductPriceBase { get; set; }
			public decimal? ProductPriceSale { get; set; }
			public string? ProductStock { get; set; }
			public IFormFile ProductPic { get; set; }
		}
		public class PrimaryUserInfoModel
		{
			public string? UserFullName { get; set; }
			public string? UserEmail { get; set; }
			public string? PhoneNumber { get; set; }
		}
		public class PrimaryUserCartModel
		{
			public int ProductId { get; set; }
			public string? ProductName { get; set; }
			public decimal? ProductPriceBase { get; set; }
			public decimal? ProductPriceSale { get; set; }
			public int? BasketProductCount{ get; set; }
			public IFormFile ProductPic { get; set; }

		}
		public class PrimaryViewModel
		{
			public IList<PrimaryUserCartModel> ShoppingCart { get; set; }
			public IPagedList<PrimaryProductInfoViewModel> ProductInfo { get; set; }

		}
	}
}
