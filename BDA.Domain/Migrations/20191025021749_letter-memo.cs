using Microsoft.EntityFrameworkCore.Migrations;

namespace BDA.Migrations
{
    public partial class lettermemo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IssuedBDReceiverContactNo",
                table: "Memo",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SendingMethod",
                table: "Memo",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InstructionLetterEmail",
                table: "InstructionLetter",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IssuedBDReceiverContactNo",
                table: "Memo");

            migrationBuilder.DropColumn(
                name: "SendingMethod",
                table: "Memo");

            migrationBuilder.DropColumn(
                name: "InstructionLetterEmail",
                table: "InstructionLetter");
        }
    }
}
