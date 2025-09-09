using Microsoft.EntityFrameworkCore.Migrations;

namespace BDA.Migrations
{
    public partial class ModificationToBDAction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RequestorComment",
                table: "BankDraft",
                newName: "TGBSAcceptedComment");

            migrationBuilder.AlterColumn<string>(
                name: "ActionType",
                table: "BankDraftAction",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "ActionRole",
                table: "BankDraftAction",
                nullable: true,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TGBSAcceptedComment",
                table: "BankDraft",
                newName: "RequestorComment");

            migrationBuilder.AlterColumn<int>(
                name: "ActionType",
                table: "BankDraftAction",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ActionRole",
                table: "BankDraftAction",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
