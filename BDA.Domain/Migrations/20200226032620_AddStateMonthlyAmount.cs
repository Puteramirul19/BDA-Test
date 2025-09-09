using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BDA.Migrations
{
    public partial class AddStateMonthlyAmount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StateMonthlyAmount",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    isActive = table.Column<bool>(nullable: false),
                    Month = table.Column<int>(nullable: false),
                    Year = table.Column<int>(nullable: false),
                    StateId = table.Column<Guid>(nullable: false),
                    BDNoIssued = table.Column<int>(nullable: false),
                    Amount = table.Column<decimal>(nullable: true),
                    BDNoRecovered = table.Column<int>(nullable: false),
                    RecoveryAmount = table.Column<decimal>(nullable: true),
                    BDNoOutstanding = table.Column<int>(nullable: false),
                    OutstandingAmount = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StateMonthlyAmount", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StateMonthlyAmount");
        }
    }
}
