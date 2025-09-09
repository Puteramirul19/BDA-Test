using Microsoft.EntityFrameworkCore.Migrations;

namespace BDA.Migrations
{
    public partial class AddFieldToRecoverynCancellation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReceiverId",
                table: "Recovery",
                newName: "TGBSValidatorId");

            migrationBuilder.RenameColumn(
                name: "ReceivedById",
                table: "Cancellation",
                newName: "TGBSValidatorId");

            migrationBuilder.AddColumn<string>(
                name: "TGBSReceiverId",
                table: "Recovery",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TGBSReceiverId",
                table: "Cancellation",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TGBSReceiverId",
                table: "Recovery");

            migrationBuilder.DropColumn(
                name: "TGBSReceiverId",
                table: "Cancellation");

            migrationBuilder.RenameColumn(
                name: "TGBSValidatorId",
                table: "Recovery",
                newName: "ReceiverId");

            migrationBuilder.RenameColumn(
                name: "TGBSValidatorId",
                table: "Cancellation",
                newName: "ReceivedById");
        }
    }
}
