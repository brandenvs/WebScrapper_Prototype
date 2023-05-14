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
				table: "OrderDb");

			migrationBuilder.DropColumn(
				name: "OrderTotalHandlingFee",
				table: "OrderDb");

			migrationBuilder.DropColumn(
				name: "OrderTotalShipping",
				table: "OrderDb");

			migrationBuilder.AddColumn<string>(
				name: "OrderTotal",
				table: "OrderDb",
				type: "nvarchar(max)",
				nullable: false,
				defaultValue: "");

			migrationBuilder.AddColumn<int>(
				name: "PaymentId",
				table: "OrderDb",
				type: "int",
				nullable: false,
				defaultValue: 0);

			migrationBuilder.AddColumn<int>(
				name: "ProductCount",
				table: "OrderDb",
				type: "int",
				nullable: false,
				defaultValue: 0);

			migrationBuilder.AddColumn<int>(
				name: "ShippingPrice",
				table: "OrderDb",
				type: "int",
				nullable: false,
				defaultValue: 0);

			migrationBuilder.AddColumn<bool>(
				name: "IsOrderPayed",
				table: "OrderDb",
				type: "bit",
				nullable: false,
				defaultValue: false);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "OrderTotal",
				table: "OrderDb");

			migrationBuilder.DropColumn(
				name: "PaymentId",
				table: "OrderDb");

			migrationBuilder.DropColumn(
				name: "ProductCount",
				table: "OrderDb");

			migrationBuilder.DropColumn(
				name: "ShippingPrice",
				table: "OrderDb");

			migrationBuilder.DropColumn(
				name: "IsOrderPayed",
				table: "OrderDb");

			migrationBuilder.AddColumn<decimal>(
				name: "OrderGrandTotal",
				table: "OrderDb",
				type: "decimal(18,2)",
				nullable: false,
				defaultValue: 0m);

			migrationBuilder.AddColumn<decimal>(
				name: "OrderTotalHandlingFee",
				table: "OrderDb",
				type: "decimal(18,2)",
				nullable: false,
				defaultValue: 0m);

			migrationBuilder.AddColumn<decimal>(
				name: "OrderTotalShipping",
				table: "OrderDb",
				type: "decimal(18,2)",
				nullable: false,
				defaultValue: 0m);
		}
	}
}
