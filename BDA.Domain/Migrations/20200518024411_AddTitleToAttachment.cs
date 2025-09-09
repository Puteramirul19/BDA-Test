using Microsoft.EntityFrameworkCore.Migrations;

namespace BDA.Migrations
{
    public partial class AddTitleToAttachment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Attachment",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BusinessArea_StateId",
                table: "BusinessArea",
                column: "StateId");

            migrationBuilder.AddForeignKey(
                name: "FK_BusinessArea_State_StateId",
                table: "BusinessArea",
                column: "StateId",
                principalTable: "State",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BusinessArea_State_StateId",
                table: "BusinessArea");

            migrationBuilder.DropIndex(
                name: "IX_BusinessArea_StateId",
                table: "BusinessArea");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Attachment");
        }
    }
}
