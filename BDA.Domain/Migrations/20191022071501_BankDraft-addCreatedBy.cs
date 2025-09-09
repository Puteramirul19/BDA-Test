using Microsoft.EntityFrameworkCore.Migrations;

namespace BDA.Migrations
{
    public partial class BankDraftaddCreatedBy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "BankDraft",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByName",
                table: "BankDraft",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "BankDraft");

            migrationBuilder.DropColumn(
                name: "CreatedByName",
                table: "BankDraft");
        }
    }
}
