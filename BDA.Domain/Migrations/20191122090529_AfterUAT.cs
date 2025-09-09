using Microsoft.EntityFrameworkCore.Migrations;

namespace BDA.Migrations
{
    public partial class AfterUAT : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "WangHangus",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ApproverId",
                table: "BankDraft",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProjectNo",
                table: "BankDraft",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BankDraft_ApproverId",
                table: "BankDraft",
                column: "ApproverId");

            migrationBuilder.AddForeignKey(
                name: "FK_BankDraft_AspNetUsers_ApproverId",
                table: "BankDraft",
                column: "ApproverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankDraft_AspNetUsers_ApproverId",
                table: "BankDraft");

            migrationBuilder.DropIndex(
                name: "IX_BankDraft_ApproverId",
                table: "BankDraft");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "WangHangus");

            migrationBuilder.DropColumn(
                name: "ProjectNo",
                table: "BankDraft");

            migrationBuilder.AlterColumn<string>(
                name: "ApproverId",
                table: "BankDraft",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
