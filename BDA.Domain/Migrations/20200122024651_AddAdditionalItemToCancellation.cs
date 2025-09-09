using Microsoft.EntityFrameworkCore.Migrations;

namespace BDA.Migrations
{
    public partial class AddAdditionalItemToCancellation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BA",
                table: "Cancellation",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CoCode",
                table: "Cancellation",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ERMSDocNo",
                table: "Cancellation",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameOnBD",
                table: "Cancellation",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BA",
                table: "Cancellation");

            migrationBuilder.DropColumn(
                name: "CoCode",
                table: "Cancellation");

            migrationBuilder.DropColumn(
                name: "ERMSDocNo",
                table: "Cancellation");

            migrationBuilder.DropColumn(
                name: "NameOnBD",
                table: "Cancellation");
        }
    }
}
