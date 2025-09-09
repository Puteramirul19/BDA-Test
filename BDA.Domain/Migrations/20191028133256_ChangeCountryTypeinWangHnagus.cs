using Microsoft.EntityFrameworkCore.Migrations;

namespace BDA.Migrations
{
    public partial class ChangeCountryTypeinWangHnagus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Country",
                table: "WangHangus",
                nullable: true,
                oldClrType: typeof(bool));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "Country",
                table: "WangHangus",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
