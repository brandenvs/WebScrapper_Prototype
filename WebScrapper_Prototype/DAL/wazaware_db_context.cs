using Microsoft.EntityFrameworkCore;
using wazaware.co.za.Models.DatabaseModels;

namespace WazaWare.co.za.DAL
{
	public class WazaWare_db_context : DbContext
	{
		public WazaWare_db_context(DbContextOptions<WazaWare_db_context> options)
			: base(options) { }
		public DbSet<UserAccount>? UserAccountDb { get; set; }
		public DbSet<ShoppingCart>? ShoppingCartDb { get; set; }
		public DbSet<ShippingAddress>? ShippingAddressDb { get; set; }
		public DbSet<Order>? OrderDb { get; set; }
		public DbSet<OrderedProducts>? OrderedProductsDb { get; set; }
		public DbSet<BillingAddress>? BillingAddressDb { get; set; }
		public DbSet<Product>? ProductDb { get; set; }	
	}
}
