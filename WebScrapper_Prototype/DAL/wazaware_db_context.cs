using Microsoft.EntityFrameworkCore;
using wazaware.co.za.Models.DatabaseModels;

namespace wazaware.co.za.DAL
{
	public class wazaware_db_context : DbContext
	{
		public wazaware_db_context(DbContextOptions<wazaware_db_context> options)
			: base(options) { }
		public DbSet<UserAccount>? UserAccountDb { get; set; }
		public DbSet<ShoppingCart>? ShoppingCartDb { get; set; }
		public DbSet<ShippingAddress>? ShippingAddressDb { get; set; }
		public DbSet<Order>? OrderDb { get; set; }
		public DbSet<OrderedProducts>? OrderedProductsDb { get; set; }
		public DbSet<BillingAddress>? BillingAddressDb { get; set; }
		public DbSet<Product>? ProductDb { get; set; }
		public DbSet<ProductImage>? ProductImageDb { get; set; }
	}
}