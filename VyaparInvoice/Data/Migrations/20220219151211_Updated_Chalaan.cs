using Microsoft.EntityFrameworkCore.Migrations;

namespace VyaparInvoice.Data.Migrations
{
    public partial class Updated_Chalaan : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClientGSTNumber",
                table: "Chalaan",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClientGSTNumber",
                table: "Chalaan");
        }
    }
}
