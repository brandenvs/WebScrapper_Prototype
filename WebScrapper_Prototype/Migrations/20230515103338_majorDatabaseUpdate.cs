using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace wazaware.co.za.Migrations
{
    public partial class majorDatabaseUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderProducts");

			migrationBuilder.DropTable(
	            name: "Orders");

			migrationBuilder.DropTable(
                name: "PaymentModels");

            migrationBuilder.DropTable(
                name: "ProductImages");

			migrationBuilder.DropTable(
                name: "Products");

			migrationBuilder.DropTable(
                name: "Users");

			migrationBuilder.DropTable(
                name: "UserShippings");

			migrationBuilder.DropTable(
	            name: "UsersShoppingCarts");


			migrationBuilder.CreateTable(
						  name: "UserAccounts",
						  columns: table => new
						  {
							  UserId = table.Column<int>(nullable: false)
								  .Annotation("SqlServer:Identity", "1, 1"),
							  FirstName = table.Column<string>(nullable: false),
							  LastName = table.Column<string>(nullable: false),
							  Email = table.Column<string>(nullable: false),
							  Phone = table.Column<string>(nullable: false),
							  Password = table.Column<string>(nullable: false),
							  Joined = table.Column<DateTime>(nullable: false)
						  },
						  constraints: table =>
						  {
							  table.PrimaryKey("PK_UserAccounts", x => x.UserId);
						  });

			migrationBuilder.CreateTable(
				name: "ShoppingCarts",
				columns: table => new
				{
					CartId = table.Column<int>(nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					UserId = table.Column<int>(nullable: false),
					ProductId = table.Column<int>(nullable: false),
					ProductCount = table.Column<int>(nullable: false),
					ProductTotal = table.Column<decimal>(nullable: false),
					CartEntryDate = table.Column<DateTime>(nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_ShoppingCarts", x => x.CartId);
					table.ForeignKey(
						name: "FK_ShoppingCarts_UserAccounts_UserId",
						column: x => x.UserId,
						principalTable: "UserAccounts",
						principalColumn: "UserId",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "ShippingAddresses",
				columns: table => new
				{
					ShippingId = table.Column<int>(nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					UserId = table.Column<int>(nullable: false),
					FirstName = table.Column<string>(maxLength: 50, nullable: false),
					LastName = table.Column<string>(maxLength: 50, nullable: false),
					Phone = table.Column<string>(nullable: false),
					Email = table.Column<string>(nullable: false),
					UnitNo = table.Column<int>(maxLength: 50, nullable: true),
					StreetAddress = table.Column<string>(maxLength: 100, nullable: false),
					Suburb = table.Column<string>(maxLength: 50, nullable: false),
					City = table.Column<string>(maxLength: 50, nullable: false),
					Province = table.Column<string>(maxLength: 50, nullable: false),
					PostalCode = table.Column<int>(nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_ShippingAddresses", x => x.ShippingId);
					table.ForeignKey(
						name: "FK_ShippingAddresses_UserAccounts_UserId",
						column: x => x.UserId,
						principalTable: "UserAccounts",
						principalColumn: "UserId",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateIndex(
				name: "IX_ShoppingCarts_UserId",
				table: "ShoppingCarts",
				column: "UserId");

			migrationBuilder.CreateIndex(
				name: "IX_ShippingAddresses_UserId",
				table: "ShippingAddresses",
				column: "UserId");
		}

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
