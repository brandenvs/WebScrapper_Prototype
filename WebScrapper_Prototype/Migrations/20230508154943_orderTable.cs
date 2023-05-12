using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WazaWare.co.za.Migrations
{
	public partial class orderTable : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "OrderGrandTotal",
				table: "Orders");

			migrationBuilder.DropColumn(
				name: "OrderTotalHandlingFee",
				table: "Orders");

			migrationBuilder.DropColumn(
				name: "OrderTotalShipping",
				table: "Orders");

			migrationBuilder.AddColumn<string>(
				name: "OrderTotal",
				table: "Orders",
				type: "nvarchar(max)",
				nullable: false,
				defaultValue: "");

			migrationBuilder.AddColumn<int>(
				name: "PaymentId",
				table: "Orders",
				type: "int",
				nullable: false,
				defaultValue: 0);

			migrationBuilder.AddColumn<int>(
				name: "ProductCount",
				table: "Orders",
				type: "int",
				nullable: false,
				defaultValue: 0);

			migrationBuilder.AddColumn<int>(
				name: "ShippingPrice",
				table: "Orders",
				type: "int",
				nullable: false,
				defaultValue: 0);

			migrationBuilder.AddColumn<bool>(
				name: "isOrderPayed",
				table: "Orders",
				type: "bit",
				nullable: false,
				defaultValue: false);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "OrderTotal",
				table: "Orders");

			migrationBuilder.DropColumn(
				name: "PaymentId",
				table: "Orders");

			migrationBuilder.DropColumn(
				name: "ProductCount",
				table: "Orders");

			migrationBuilder.DropColumn(
				name: "ShippingPrice",
				table: "Orders");

			migrationBuilder.DropColumn(
				name: "isOrderPayed",
				table: "Orders");

			migrationBuilder.AddColumn<decimal>(
				name: "OrderGrandTotal",
				table: "Orders",
				type: "decimal(18,2)",
				nullable: false,
				defaultValue: 0m);

			migrationBuilder.AddColumn<decimal>(
				name: "OrderTotalHandlingFee",
				table: "Orders",
				type: "decimal(18,2)",
				nullable: false,
				defaultValue: 0m);

			migrationBuilder.AddColumn<decimal>(
				name: "OrderTotalShipping",
				table: "Orders",
				type: "decimal(18,2)",
				nullable: false,
				defaultValue: 0m);
		}
	}
}
