using Microsoft.EntityFrameworkCore.Migrations;

namespace BDA.Migrations
{
    public partial class AddArchivedStructure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Assignment",
                table: "WangCagaran",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BusinessArea",
                table: "WangCagaran",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CoCode",
                table: "WangCagaran",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CostCenter",
                table: "WangCagaran",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GL",
                table: "WangCagaran",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PK",
                table: "WangCagaran",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostingDate",
                table: "WangCagaran",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "WangCagaran",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Assignment",
                table: "WangCagaran");

            migrationBuilder.DropColumn(
                name: "BusinessArea",
                table: "WangCagaran");

            migrationBuilder.DropColumn(
                name: "CoCode",
                table: "WangCagaran");

            migrationBuilder.DropColumn(
                name: "CostCenter",
                table: "WangCagaran");

            migrationBuilder.DropColumn(
                name: "GL",
                table: "WangCagaran");

            migrationBuilder.DropColumn(
                name: "PK",
                table: "WangCagaran");

            migrationBuilder.DropColumn(
                name: "PostingDate",
                table: "WangCagaran");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "WangCagaran");
        }
    }
}
