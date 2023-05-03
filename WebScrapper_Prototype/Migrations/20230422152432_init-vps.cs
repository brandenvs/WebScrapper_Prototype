using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WazaWare.co.za.Migrations
{
    public partial class initvps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.DropColumn(
				name: "UserEmail",
				table: "Orders");
			migrationBuilder.AddColumn<int>(
				name: "UserId",
				table: "Orders",
				type: "int",
				nullable: false);
		}

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
