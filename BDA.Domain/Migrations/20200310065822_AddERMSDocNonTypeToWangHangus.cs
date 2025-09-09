using Microsoft.EntityFrameworkCore.Migrations;

namespace BDA.Migrations
{
    public partial class AddERMSDocNonTypeToWangHangus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ErmsDocNo",
                table: "WangHangus",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "WangHangus",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ErmsDocNo",
                table: "WangHangus");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "WangHangus");
        }
    }
}
