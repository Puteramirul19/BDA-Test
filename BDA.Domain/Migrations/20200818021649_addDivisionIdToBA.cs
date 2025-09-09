using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BDA.Migrations
{
    public partial class addDivisionIdToBA : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BusinessArea_Division_DivisionId",
                table: "BusinessArea");

            migrationBuilder.AlterColumn<Guid>(
                name: "DivisionId",
                table: "BusinessArea",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BusinessArea_Division_DivisionId",
                table: "BusinessArea",
                column: "DivisionId",
                principalTable: "Division",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BusinessArea_Division_DivisionId",
                table: "BusinessArea");

            migrationBuilder.AlterColumn<Guid>(
                name: "DivisionId",
                table: "BusinessArea",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddForeignKey(
                name: "FK_BusinessArea_Division_DivisionId",
                table: "BusinessArea",
                column: "DivisionId",
                principalTable: "Division",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
