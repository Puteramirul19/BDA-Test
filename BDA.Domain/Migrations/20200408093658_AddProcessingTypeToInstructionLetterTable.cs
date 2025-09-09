using Microsoft.EntityFrameworkCore.Migrations;

namespace BDA.Migrations
{
    public partial class AddProcessingTypeToInstructionLetterTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "InstructionLetter");

            migrationBuilder.DropColumn(
                name: "CostObject",
                table: "InstructionLetter");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "InstructionLetter");

            migrationBuilder.DropColumn(
                name: "TaxAmount",
                table: "InstructionLetter");

            migrationBuilder.RenameColumn(
                name: "TaxCode",
                table: "InstructionLetter",
                newName: "ProcessingType");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProcessingType",
                table: "InstructionLetter",
                newName: "TaxCode");

            migrationBuilder.AddColumn<string>(
                name: "Amount",
                table: "InstructionLetter",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CostObject",
                table: "InstructionLetter",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "InstructionLetter",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaxAmount",
                table: "InstructionLetter",
                nullable: true);
        }
    }
}
