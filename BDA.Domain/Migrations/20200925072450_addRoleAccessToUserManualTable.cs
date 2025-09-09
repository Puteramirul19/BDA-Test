using Microsoft.EntityFrameworkCore.Migrations;

namespace BDA.Migrations
{
    public partial class addRoleAccessToUserManualTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Sequence",
                table: "UserManual",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RoleAccess",
                table: "UserManual",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoleAccess",
                table: "UserManual");

            migrationBuilder.AlterColumn<string>(
                name: "Sequence",
                table: "UserManual",
                nullable: true,
                oldClrType: typeof(int));
        }
    }
}
