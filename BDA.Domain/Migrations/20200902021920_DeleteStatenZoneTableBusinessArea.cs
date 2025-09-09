using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BDA.Migrations
{
    public partial class DeleteStatenZoneTableBusinessArea : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BusinessArea_State_StateId",
                table: "BusinessArea");

            migrationBuilder.DropIndex(
                name: "IX_BusinessArea_StateId",
                table: "BusinessArea");

            migrationBuilder.DropColumn(
                name: "StateId",
                table: "BusinessArea");

            migrationBuilder.DropColumn(
                name: "Zone",
                table: "BusinessArea");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "StateId",
                table: "BusinessArea",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Zone",
                table: "BusinessArea",
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
    }
}
