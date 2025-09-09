using Microsoft.EntityFrameworkCore.Migrations;

namespace BDA.Migrations
{
    public partial class bankDraftAmountToDecimal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "BankDraftAmount",
                table: "BankDraft",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(double));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "BankDraftAmount",
                table: "BankDraft",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);
        }
    }
}
