using Microsoft.EntityFrameworkCore.Migrations;

namespace BDA.Migrations
{
    public partial class AddItemToRecoveryTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BA",
                table: "Recovery",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CoCode",
                table: "Recovery",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProjNo",
                table: "Recovery",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BA",
                table: "Recovery");

            migrationBuilder.DropColumn(
                name: "CoCode",
                table: "Recovery");

            migrationBuilder.DropColumn(
                name: "ProjNo",
                table: "Recovery");
        }
    }
}
