using Microsoft.EntityFrameworkCore.Migrations;

namespace BDA.Migrations
{
    public partial class AddRecoveryTableAppUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RequesterId",
                table: "Recovery",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Recovery_RequesterId",
                table: "Recovery",
                column: "RequesterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Recovery_AspNetUsers_RequesterId",
                table: "Recovery",
                column: "RequesterId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recovery_AspNetUsers_RequesterId",
                table: "Recovery");

            migrationBuilder.DropIndex(
                name: "IX_Recovery_RequesterId",
                table: "Recovery");

            migrationBuilder.AlterColumn<string>(
                name: "RequesterId",
                table: "Recovery",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
