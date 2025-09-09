using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BDA.Migrations
{
    public partial class AddDivisionToBusinessAreaTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DivisionId",
                table: "BusinessArea",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BusinessArea_DivisionId",
                table: "BusinessArea",
                column: "DivisionId");

            migrationBuilder.AddForeignKey(
                name: "FK_BusinessArea_Division_DivisionId",
                table: "BusinessArea",
                column: "DivisionId",
                principalTable: "Division",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BusinessArea_Division_DivisionId",
                table: "BusinessArea");

            migrationBuilder.DropIndex(
                name: "IX_BusinessArea_DivisionId",
                table: "BusinessArea");

            migrationBuilder.DropColumn(
                name: "DivisionId",
                table: "BusinessArea");
        }
    }
}
