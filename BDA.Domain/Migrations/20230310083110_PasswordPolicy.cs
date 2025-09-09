using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BDA.Migrations
{
    public partial class PasswordPolicy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PasswordExpiredOn",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "ResetPassword",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordExpiredOn",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ResetPassword",
                table: "AspNetUsers");
        }
    }
}
