using Microsoft.EntityFrameworkCore.Migrations;

namespace BDA.Migrations
{
    public partial class UpdateDBForRecovery3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Stage",
                table: "Recovery",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Stage",
                table: "Recovery");
        }
    }
}
