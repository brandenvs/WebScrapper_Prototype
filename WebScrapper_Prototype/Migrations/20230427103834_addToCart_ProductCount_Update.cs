using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WazaWare.co.za.Migrations
{
	public partial class addToCart_ProductCount_Update : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<int>(
				name: "ProductCount",
				table: "UsersShoppingCarts",
				type: "int",
				nullable: false,
				defaultValue: 0);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "ProductCount",
				table: "UsersShoppingCarts");
		}
	}
}
