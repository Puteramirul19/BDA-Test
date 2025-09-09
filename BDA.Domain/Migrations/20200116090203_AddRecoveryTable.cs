using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BDA.Migrations
{
    public partial class AddRecoveryTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Recovery",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    isActive = table.Column<bool>(nullable: false),
                    BankDraftId = table.Column<Guid>(nullable: false),
                    Status = table.Column<string>(nullable: true),
                    RefNo = table.Column<string>(nullable: true),
                    BDNo = table.Column<string>(nullable: true),
                    NameOnBD = table.Column<string>(nullable: true),
                    BDAmount = table.Column<decimal>(nullable: true),
                    ProjectCompletionDate = table.Column<DateTime>(nullable: true),
                    BDRequesterName = table.Column<string>(nullable: true),
                    PBTEmailAddress = table.Column<string>(nullable: true),
                    RequesterId = table.Column<string>(nullable: true),
                    RequesterSubmissionComment = table.Column<string>(nullable: true),
                    DraftedOn = table.Column<DateTime>(nullable: true),
                    SubmittedOn = table.Column<DateTime>(nullable: true),
                    FullSubmittedOn = table.Column<DateTime>(nullable: true),
                    CreatedById = table.Column<string>(nullable: true),
                    CreatedByName = table.Column<string>(nullable: true),
                    SiteVisitDate = table.Column<DateTime>(nullable: true),
                    CPCDate = table.Column<DateTime>(nullable: true),
                    ClaimDuration = table.Column<string>(nullable: true),
                    RequesterProcessComment = table.Column<string>(nullable: true),
                    ProcesserId = table.Column<string>(nullable: true),
                    ProcessedOn = table.Column<DateTime>(nullable: true),
                    RecoveryType = table.Column<string>(nullable: true),
                    TotalRecoveryAmount = table.Column<decimal>(nullable: true),
                    FirstRecoveryAmount = table.Column<decimal>(nullable: true),
                    SecondRecoveryAmount = table.Column<decimal>(nullable: true),
                    RequesterFinalSubmissionComment = table.Column<string>(nullable: true),
                    SubmitterId = table.Column<string>(nullable: true),
                    FinalSubmissionOn = table.Column<DateTime>(nullable: true),
                    ReceivedDate = table.Column<DateTime>(nullable: true),
                    ReceiverId = table.Column<string>(nullable: true),
                    ReceivedOn = table.Column<DateTime>(nullable: true),
                    TGBSConfirmationComment = table.Column<string>(nullable: true),
                    CompletedOn = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recovery", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Recovery_BankDraft_BankDraftId",
                        column: x => x.BankDraftId,
                        principalTable: "BankDraft",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Recovery_BankDraftId",
                table: "Recovery",
                column: "BankDraftId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Recovery");
        }
    }
}
