using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace wazaware.co.za.Migrations
{
    public partial class majorDatabaseUpdateOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.CreateTable(
				name: "Orders",
				columns: table => new
				{
					OrderId = table.Column<int>(nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					UserId = table.Column<int>(nullable: false),
					ShippingPrice = table.Column<int>(nullable: true),
					OrderTotal = table.Column<decimal>(nullable: true),
					IsOrderPayed = table.Column<bool>(nullable: false),
					OrderCreatedOn = table.Column<DateTime>(nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Orders", x => x.OrderId);
				});

			migrationBuilder.CreateTable(
				name: "OrderedProducts",
				columns: table => new
				{
					Id = table.Column<int>(nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					OrderId = table.Column<int>(nullable: false),
					ProductId = table.Column<int>(nullable: false),
					ProductCount = table.Column<int>(nullable: false),
					ProductTotal = table.Column<decimal>(nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_OrderedProducts", x => x.Id);
					table.ForeignKey(
						name: "FK_OrderedProducts_Orders_OrderId",
						column: x => x.OrderId,
						principalTable: "Orders",
						principalColumn: "OrderId",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "BillingAddresses",
				columns: table => new
				{
					BillingId = table.Column<int>(nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					OrderId = table.Column<int>(nullable: false),
					PaymentMethod = table.Column<string>(nullable: false),
					FirstName = table.Column<string>(maxLength: 50, nullable: false),
					LastName = table.Column<string>(maxLength: 50, nullable: false),
					Phone = table.Column<string>(nullable: false),
					Email = table.Column<string>(nullable: false),
					UnitNo = table.Column<int>(nullable: false),
					StreetAddress = table.Column<string>(maxLength: 100, nullable: false),
					Suburb = table.Column<string>(maxLength: 50, nullable: false),
					City = table.Column<string>(maxLength: 50, nullable: false),
					Province = table.Column<string>(maxLength: 50, nullable: false),
					PostalCode = table.Column<int>(nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_BillingAddresses", x => x.BillingId);
					table.ForeignKey(
						name: "FK_BillingAddresses_Orders_OrderId",
						column: x => x.OrderId,
						principalTable: "Orders",
						principalColumn: "OrderId",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateIndex(
				name: "IX_OrderedProducts_OrderId",
				table: "OrderedProducts",
				column: "OrderId");

			migrationBuilder.CreateIndex(
				name: "IX_BillingAddresses_OrderId",
				table: "BillingAddresses",
				column: "OrderId");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
