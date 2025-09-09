using Microsoft.EntityFrameworkCore.Migrations;

namespace BDA.Migrations
{
    public partial class Attachment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FileType",
                table: "Attachment",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "FileSubType",
                table: "Attachment",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<string>(
                name: "FileExtension",
                table: "Attachment",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileExtension",
                table: "Attachment");

            migrationBuilder.AlterColumn<int>(
                name: "FileType",
                table: "Attachment",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "FileSubType",
                table: "Attachment",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
