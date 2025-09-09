using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BDA.Migrations
{
    public partial class AddPartialCompletedOnDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PartialCompletedOn",
                table: "Recovery",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PartialCompletedOn",
                table: "Recovery");
        }
    }
}
