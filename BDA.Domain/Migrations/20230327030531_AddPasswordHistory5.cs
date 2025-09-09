using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BDA.Migrations
{
    public partial class AddPasswordHistory5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Password5",
                table: "PasswordHistory",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Password5CreateDate",
                table: "PasswordHistory",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password5",
                table: "PasswordHistory");

            migrationBuilder.DropColumn(
                name: "Password5CreateDate",
                table: "PasswordHistory");
        }
    }
}
