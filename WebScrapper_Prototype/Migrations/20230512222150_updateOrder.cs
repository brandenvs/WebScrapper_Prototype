using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WazaWare.co.za.Migrations
{
    public partial class updateOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductCount",
                table: "OrderDb");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "OrderDb",
                newName: "UserShippingId");

            migrationBuilder.AddColumn<decimal>(
                name: "ProductTotal",
                table: "OrderProducts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductTotal",
                table: "OrderProducts");

            migrationBuilder.RenameColumn(
                name: "UserShippingId",
                table: "OrderDb",
                newName: "ProductId");

            migrationBuilder.AddColumn<int>(
                name: "ProductCount",
                table: "OrderDb",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
