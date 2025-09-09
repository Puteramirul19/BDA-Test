using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BDA.Migrations
{
    public partial class AddErmsLogTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ErmsLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    BankDraftId = table.Column<Guid>(nullable: false),
                    RequestId = table.Column<string>(nullable: true),
                    TransactionId = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    Source = table.Column<string>(nullable: true),
                    Destination = table.Column<string>(nullable: true),
                    Action = table.Column<string>(nullable: true),
                    Message = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ErmsLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ErmsLog_BankDraft_BankDraftId",
                        column: x => x.BankDraftId,
                        principalTable: "BankDraft",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ErmsLog_BankDraftId",
                table: "ErmsLog",
                column: "BankDraftId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ErmsLog");
        }
    }
}
