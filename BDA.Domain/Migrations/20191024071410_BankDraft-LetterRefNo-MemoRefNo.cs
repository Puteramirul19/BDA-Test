using Microsoft.EntityFrameworkCore.Migrations;

namespace BDA.Migrations
{
    public partial class BankDraftLetterRefNoMemoRefNo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "BankDraftAttachment");

            migrationBuilder.AddColumn<string>(
                name: "CoverMemoRefNo",
                table: "BankDraft",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InstructionLetterRefNo",
                table: "BankDraft",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoverMemoRefNo",
                table: "BankDraft");

            migrationBuilder.DropColumn(
                name: "InstructionLetterRefNo",
                table: "BankDraft");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "BankDraftAttachment",
                nullable: false,
                defaultValue: 0);
        }
    }
}
