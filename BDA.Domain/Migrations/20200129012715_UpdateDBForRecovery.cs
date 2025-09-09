using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BDA.Migrations
{
    public partial class UpdateDBForRecovery : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FinalPartialSubmissionOn",
                table: "Recovery",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PartialReceivedDate",
                table: "Recovery",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PartialReceivedOn",
                table: "Recovery",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PartialSubmitterId",
                table: "Recovery",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TGBSPartialReceiverId",
                table: "Recovery",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TGBSPartialValidatorId",
                table: "Recovery",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FinalPartialSubmissionOn",
                table: "Recovery");

            migrationBuilder.DropColumn(
                name: "PartialReceivedDate",
                table: "Recovery");

            migrationBuilder.DropColumn(
                name: "PartialReceivedOn",
                table: "Recovery");

            migrationBuilder.DropColumn(
                name: "PartialSubmitterId",
                table: "Recovery");

            migrationBuilder.DropColumn(
                name: "TGBSPartialReceiverId",
                table: "Recovery");

            migrationBuilder.DropColumn(
                name: "TGBSPartialValidatorId",
                table: "Recovery");
        }
    }
}
