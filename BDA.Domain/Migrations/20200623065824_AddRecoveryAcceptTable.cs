using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BDA.Migrations
{
    public partial class AddRecoveryAcceptTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AcceptedOn",
                table: "Recovery",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PartialAcceptedOn",
                table: "Recovery",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TGBSAcceptanceId",
                table: "Recovery",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TGBSPartialAcceptanceId",
                table: "Recovery",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcceptedOn",
                table: "Recovery");

            migrationBuilder.DropColumn(
                name: "PartialAcceptedOn",
                table: "Recovery");

            migrationBuilder.DropColumn(
                name: "TGBSAcceptanceId",
                table: "Recovery");

            migrationBuilder.DropColumn(
                name: "TGBSPartialAcceptanceId",
                table: "Recovery");
        }
    }
}
