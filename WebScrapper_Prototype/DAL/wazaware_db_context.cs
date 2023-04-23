using Microsoft.EntityFrameworkCore;
using wazaware.co.za.Models;

namespace wazaware.co.za.DAL
{
	public class wazaware_db_context : DbContext
	{
		public wazaware_db_context(DbContextOptions<wazaware_db_context> options)
			: base(options) { }
		public DbSet<Orders> Orders { get; set; }
		public DbSet<ProductImage> ProductImages { get; set; }
		public DbSet<Product> Products { get; set; }
		public DbSet<UserModel> Users { get; set; }
		public DbSet<UserShipping> UserShippings { get; set; }
		public DbSet<UserShoppingCart> UsersShoppingCarts { get; set; }
		public DbSet<OrderProducts> OrderProducts { get; set; }
	}
}
