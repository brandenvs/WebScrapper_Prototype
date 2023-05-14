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
				table: "OrderDb");
			migrationBuilder.AddColumn<int>(
				name: "UserId",
				table: "OrderDb",
				type: "int",
				nullable: false);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{

		}
	}
}
