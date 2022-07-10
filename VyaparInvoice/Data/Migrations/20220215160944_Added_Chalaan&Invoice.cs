using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VyaparInvoice.Data.Migrations
{
    public partial class Added_ChalaanInvoice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Chalaan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChalaanNumber = table.Column<int>(type: "int", nullable: false),
                    ClientName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientPhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PayableAmount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemDetails = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chalaan", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Invoice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvoiceNumber = table.Column<int>(type: "int", nullable: false),
                    ClientName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientPhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientGSTNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaxableAmount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CGST = table.Column<int>(type: "int", nullable: false),
                    SGST = table.Column<int>(type: "int", nullable: false),
                    PayableAmount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemDetails = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoice", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Chalaan");

            migrationBuilder.DropTable(
                name: "Invoice");
        }
    }
}
