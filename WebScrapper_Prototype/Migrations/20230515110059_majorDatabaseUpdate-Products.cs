using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace wazaware.co.za.Migrations
{
    public partial class majorDatabaseUpdateProducts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.CreateTable(
			  name: "Products",
			  columns: table => new
			  {
				  ProductId = table.Column<int>(nullable: false)
					  .Annotation("SqlServer:Identity", "1, 1"),
				  ProductName = table.Column<string>(nullable: false),
				  ProductStock = table.Column<string>(nullable: false),
				  ProductDescription = table.Column<string>(nullable: false),
				  ProductCategory = table.Column<string>(nullable: false),
				  ProductPriceBase = table.Column<decimal>(nullable: true),
				  ProductPriceSale = table.Column<decimal>(nullable: true),
				  ProductVendorName = table.Column<string>(nullable: false),
				  ProductVendorUrl = table.Column<string>(nullable: false),
				  ProductVisibility = table.Column<string>(nullable: false),
				  ProductDataBatchNo = table.Column<string>(nullable: false),
				  ProductImageUrl = table.Column<string>(nullable: false)
			  },
			  constraints: table =>
			  {
				  table.PrimaryKey("PK_Products", x => x.ProductId);
			  });

			migrationBuilder.CreateTable(
				name: "ProductImages",
				columns: table => new
				{
					ImageId = table.Column<int>(nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					ProductId = table.Column<int>(nullable: false),
					ImageFileName = table.Column<string>(nullable: false),
					ImageFileType = table.Column<string>(nullable: false),
					ImageFileContent = table.Column<byte[]>(nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_ProductImages", x => x.ImageId);
					table.ForeignKey(
						name: "FK_ProductImages_Products_ProductId",
						column: x => x.ProductId,
						principalTable: "Products",
						principalColumn: "ProductId",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateIndex(
				name: "IX_ProductImages_ProductId",
				table: "ProductImages",
				column: "ProductId");
		}

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
