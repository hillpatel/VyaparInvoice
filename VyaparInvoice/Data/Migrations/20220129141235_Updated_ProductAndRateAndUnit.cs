using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VyaparInvoice.Data.Migrations
{
    public partial class Updated_ProductAndRateAndUnit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "Unit");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                table: "Unit");

            migrationBuilder.DropColumn(
                name: "LastModifierUserId",
                table: "Unit");

            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "Rate");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                table: "Rate");

            migrationBuilder.DropColumn(
                name: "LastModifierUserId",
                table: "Rate");

            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "LastModifierUserId",
                table: "Product");

            migrationBuilder.AlterColumn<string>(
                name: "CreatorUserId",
                table: "Unit",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatorUserId",
                table: "Rate",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatorUserId",
                table: "Product",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "CreatorUserId",
                table: "Unit",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                table: "Unit",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                table: "Unit",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LastModifierUserId",
                table: "Unit",
                type: "bigint",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "CreatorUserId",
                table: "Rate",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                table: "Rate",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                table: "Rate",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LastModifierUserId",
                table: "Rate",
                type: "bigint",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "CreatorUserId",
                table: "Product",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                table: "Product",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                table: "Product",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LastModifierUserId",
                table: "Product",
                type: "bigint",
                nullable: true);
        }
    }
}
