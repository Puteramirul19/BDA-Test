using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BDA.Migrations
{
    public partial class AddWithdrewOnForCancellationnRecovery : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "WithdrewOn",
                table: "Recovery",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "WithdrewOn",
                table: "Cancellation",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WithdrewOn",
                table: "Recovery");

            migrationBuilder.DropColumn(
                name: "WithdrewOn",
                table: "Cancellation");
        }
    }
}
