using Microsoft.EntityFrameworkCore.Migrations;

namespace BDA.Migrations
{
    public partial class addInvoiceNumberWangHangus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InvoiceNumber",
                table: "WangHangus",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InvoiceNumber",
                table: "WangHangus");
        }
    }
}
