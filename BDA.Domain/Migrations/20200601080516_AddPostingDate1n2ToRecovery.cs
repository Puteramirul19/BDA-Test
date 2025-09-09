using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BDA.Migrations
{
    public partial class AddPostingDate1n2ToRecovery : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PostingDate",
                table: "Recovery",
                newName: "PostingDate2");

            migrationBuilder.AddColumn<DateTime>(
                name: "PostingDate1",
                table: "Recovery",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostingDate1",
                table: "Recovery");

            migrationBuilder.RenameColumn(
                name: "PostingDate2",
                table: "Recovery",
                newName: "PostingDate");
        }
    }
}
