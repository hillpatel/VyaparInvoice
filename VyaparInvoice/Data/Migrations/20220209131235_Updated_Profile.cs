using Microsoft.EntityFrameworkCore.Migrations;

namespace VyaparInvoice.Data.Migrations
{
    public partial class Updated_Profile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CentralTax",
                table: "Profile",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StateTax",
                table: "Profile",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CentralTax",
                table: "Profile");

            migrationBuilder.DropColumn(
                name: "StateTax",
                table: "Profile");
        }
    }
}
