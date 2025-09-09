using Microsoft.EntityFrameworkCore.Migrations;

namespace BDA.Migrations
{
    public partial class addRujukanNotoMemo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RujukanNo",
                table: "Memo",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RujukanNo",
                table: "Memo");
        }
    }
}
