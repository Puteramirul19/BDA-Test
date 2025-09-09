using Microsoft.EntityFrameworkCore.Migrations;

namespace BDA.Migrations
{
    public partial class ChangeBdIdToParentIdForBdAction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankDraftAction_BankDraft_BankDraftId",
                table: "BankDraftAction");

            migrationBuilder.DropIndex(
                name: "IX_BankDraftAction_BankDraftId",
                table: "BankDraftAction");

            migrationBuilder.RenameColumn(
                name: "BankDraftId",
                table: "BankDraftAction",
                newName: "ParentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ParentId",
                table: "BankDraftAction",
                newName: "BankDraftId");

            migrationBuilder.CreateIndex(
                name: "IX_BankDraftAction_BankDraftId",
                table: "BankDraftAction",
                column: "BankDraftId");

            migrationBuilder.AddForeignKey(
                name: "FK_BankDraftAction_BankDraft_BankDraftId",
                table: "BankDraftAction",
                column: "BankDraftId",
                principalTable: "BankDraft",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
