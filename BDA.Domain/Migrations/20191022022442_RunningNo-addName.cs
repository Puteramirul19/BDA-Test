using Microsoft.EntityFrameworkCore.Migrations;

namespace BDA.Migrations
{
    public partial class RunningNoaddName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "RunningNo",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "RunningNo");
        }
    }
}
