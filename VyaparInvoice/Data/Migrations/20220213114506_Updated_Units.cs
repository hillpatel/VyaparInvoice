using Microsoft.EntityFrameworkCore.Migrations;

namespace VyaparInvoice.Data.Migrations
{
    public partial class Updated_Units : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Sequence",
                table: "Unit",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sequence",
                table: "Unit");
        }
    }
}
