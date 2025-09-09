using Microsoft.EntityFrameworkCore.Migrations;

namespace BDA.Migrations
{
    public partial class UpdateDBForRecovery2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FullSubmittedOn",
                table: "Recovery",
                newName: "PartialSubmittedOn");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PartialSubmittedOn",
                table: "Recovery",
                newName: "FullSubmittedOn");
        }
    }
}
