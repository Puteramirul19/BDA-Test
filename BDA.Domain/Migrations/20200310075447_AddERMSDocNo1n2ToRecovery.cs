using Microsoft.EntityFrameworkCore.Migrations;

namespace BDA.Migrations
{
    public partial class AddERMSDocNo1n2ToRecovery : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ErmsDocNo1",
                table: "Recovery",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ErmsDocNo2",
                table: "Recovery",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ErmsDocNo1",
                table: "Recovery");

            migrationBuilder.DropColumn(
                name: "ErmsDocNo2",
                table: "Recovery");
        }
    }
}
