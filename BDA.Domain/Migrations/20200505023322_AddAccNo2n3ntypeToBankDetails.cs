using Microsoft.EntityFrameworkCore.Migrations;

namespace BDA.Migrations
{
    public partial class AddAccNo2n3ntypeToBankDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AccountNo",
                table: "BankDetails",
                newName: "Type");

            migrationBuilder.AddColumn<string>(
                name: "AccountNo1",
                table: "BankDetails",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountNo2",
                table: "BankDetails",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountNo3",
                table: "BankDetails",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountNo1",
                table: "BankDetails");

            migrationBuilder.DropColumn(
                name: "AccountNo2",
                table: "BankDetails");

            migrationBuilder.DropColumn(
                name: "AccountNo3",
                table: "BankDetails");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "BankDetails",
                newName: "AccountNo");
        }
    }
}
