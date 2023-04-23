using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace wazaware.co.za.Migrations
{
    public partial class initvps1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.DropColumn(
				name: "UserId",
				table: "OrderProducts");
			migrationBuilder.DropColumn(
				name: "ProductKey",
				table: "OrderProducts");
			migrationBuilder.AddColumn<int>(
				name: "OrderId",
				table: "OrderProducts",
				type: "int",
				nullable: false);
			migrationBuilder.AddColumn<int>(
				name: "ProductId",
				table: "OrderProducts",
				type: "int",
				nullable: false);

		}

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
