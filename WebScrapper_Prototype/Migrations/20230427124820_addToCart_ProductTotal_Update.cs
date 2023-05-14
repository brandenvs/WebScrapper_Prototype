using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WazaWare.co.za.Migrations
{
	public partial class addToCart_ProductTotal_Update : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<decimal>(
				name: "ProductTotal",
				table: "ShoppingCartDb",
				type: "decimal(18,2)",
				nullable: true);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "ProductTotal",
				table: "ShoppingCartDb");
		}
	}
}
