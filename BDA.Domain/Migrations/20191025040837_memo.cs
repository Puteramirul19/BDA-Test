using Microsoft.EntityFrameworkCore.Migrations;

namespace BDA.Migrations
{
    public partial class memo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PostageNo",
                table: "BankDraft",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReceiverContactNo",
                table: "BankDraft",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SendMethod",
                table: "BankDraft",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostageNo",
                table: "BankDraft");

            migrationBuilder.DropColumn(
                name: "ReceiverContactNo",
                table: "BankDraft");

            migrationBuilder.DropColumn(
                name: "SendMethod",
                table: "BankDraft");
        }
    }
}
