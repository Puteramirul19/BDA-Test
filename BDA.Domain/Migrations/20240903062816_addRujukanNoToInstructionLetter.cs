using Microsoft.EntityFrameworkCore.Migrations;

namespace BDA.Migrations
{
    public partial class addRujukanNoToInstructionLetter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RujukanNo",
                table: "InstructionLetter",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RujukanNo",
                table: "InstructionLetter");
        }
    }
}
