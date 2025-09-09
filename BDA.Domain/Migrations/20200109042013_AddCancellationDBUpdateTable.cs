using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BDA.Migrations
{
    public partial class AddCancellationDBUpdateTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ComfirmedOn",
                table: "Cancellation",
                newName: "ConfirmedOn");

            migrationBuilder.AddColumn<Guid>(
                name: "BankDraftId",
                table: "Cancellation",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Cancellation_BankDraftId",
                table: "Cancellation",
                column: "BankDraftId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cancellation_BankDraft_BankDraftId",
                table: "Cancellation",
                column: "BankDraftId",
                principalTable: "BankDraft",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cancellation_BankDraft_BankDraftId",
                table: "Cancellation");

            migrationBuilder.DropIndex(
                name: "IX_Cancellation_BankDraftId",
                table: "Cancellation");

            migrationBuilder.DropColumn(
                name: "BankDraftId",
                table: "Cancellation");

            migrationBuilder.RenameColumn(
                name: "ConfirmedOn",
                table: "Cancellation",
                newName: "ComfirmedOn");
        }
    }
}
