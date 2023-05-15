using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace wazaware.co.za.Migrations
{
    public partial class majorDatabaseUpdateRenaming : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.RenameTable(
	            name: "UserAccounts",
	            newName: "UserAccountDb");

			migrationBuilder.RenameTable(
				name: "ShoppingCarts",
				newName: "ShoppingCartDb");

			migrationBuilder.RenameTable(
				name: "ShippingAddresses",
				newName: "ShippingAddressDb");

			migrationBuilder.RenameTable(
				name: "Orders",
				newName: "OrderDb");

			migrationBuilder.RenameTable(
				name: "OrderedProducts",
				newName: "OrderedProducts");

			migrationBuilder.RenameTable(
				name: "BillingAddresses",
				newName: "BillingAddressDb");

			migrationBuilder.RenameTable(
				name: "Products",
				newName: "ProductDb");

			migrationBuilder.RenameTable(
				name: "ProductImages",
				newName: "ProductImageDb");

		}

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
