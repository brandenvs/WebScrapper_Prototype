using wazaware.co.za.Models.DatabaseModels;

namespace wazaware.co.za.Models.ViewModels
{
	public class PartialView
	{
		public IList<ShoppingCartView>? ShoppingCart { get; set; }
		public UserView? User { get; set; }
		public string Search { get; set; }
	}
}
