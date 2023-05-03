using Microsoft.EntityFrameworkCore;
using WazaWare.co.za.Models;

namespace WazaWare.co.za.DAL
{
	public class WazaWare_db_context : DbContext
	{
		public WazaWare_db_context(DbContextOptions<WazaWare_db_context> options)
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
