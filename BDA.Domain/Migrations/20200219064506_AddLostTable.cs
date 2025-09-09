using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BDA.Migrations
{
    public partial class AddLostTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Lost",
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
                    BDRequesterName = table.Column<string>(nullable: true),
                    ProjectNo = table.Column<string>(nullable: true),
                    ERMSDocNo = table.Column<string>(nullable: true),
                    CoCode = table.Column<string>(nullable: true),
                    BA = table.Column<string>(nullable: true),
                    NameOnBD = table.Column<string>(nullable: true),
                    BDAmount = table.Column<decimal>(nullable: true),
                    RequesterId = table.Column<string>(nullable: true),
                    Justification = table.Column<string>(nullable: true),
                    DraftedOn = table.Column<DateTime>(nullable: true),
                    SubmittedOn = table.Column<DateTime>(nullable: true),
                    WithdrewOn = table.Column<DateTime>(nullable: true),
                    CreatedById = table.Column<string>(nullable: true),
                    CreatedByName = table.Column<string>(nullable: true),
                    ApproverComment = table.Column<string>(nullable: true),
                    ApproverId = table.Column<string>(nullable: true),
                    ApprovedOn = table.Column<DateTime>(nullable: true),
                    TGBSAcceptedComment = table.Column<string>(nullable: true),
                    TGBSAcceptanceId = table.Column<string>(nullable: true),
                    TGBSAcceptedOn = table.Column<DateTime>(nullable: true),
                    InstructionLetterRefNo = table.Column<string>(nullable: true),
                    TGBSProcesserId = table.Column<string>(nullable: true),
                    TGBSProcessedOn = table.Column<DateTime>(nullable: true),
                    ReceivedDate = table.Column<DateTime>(nullable: true),
                    TGBSReceiverId = table.Column<string>(nullable: true),
                    ReconReceiverId = table.Column<string>(nullable: true),
                    ReceivedOn = table.Column<DateTime>(nullable: true),
                    TGBSConfirmationComment = table.Column<string>(nullable: true),
                    CompletedOn = table.Column<DateTime>(nullable: true),
                    TGBSValidatorId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lost", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lost_AspNetUsers_ApproverId",
                        column: x => x.ApproverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Lost_BankDraft_BankDraftId",
                        column: x => x.BankDraftId,
                        principalTable: "BankDraft",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Lost_AspNetUsers_ReconReceiverId",
                        column: x => x.ReconReceiverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Lost_AspNetUsers_RequesterId",
                        column: x => x.RequesterId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Lost_AspNetUsers_TGBSAcceptanceId",
                        column: x => x.TGBSAcceptanceId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Lost_AspNetUsers_TGBSProcesserId",
                        column: x => x.TGBSProcesserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Lost_ApproverId",
                table: "Lost",
                column: "ApproverId");

            migrationBuilder.CreateIndex(
                name: "IX_Lost_BankDraftId",
                table: "Lost",
                column: "BankDraftId");

            migrationBuilder.CreateIndex(
                name: "IX_Lost_ReconReceiverId",
                table: "Lost",
                column: "ReconReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Lost_RequesterId",
                table: "Lost",
                column: "RequesterId");

            migrationBuilder.CreateIndex(
                name: "IX_Lost_TGBSAcceptanceId",
                table: "Lost",
                column: "TGBSAcceptanceId");

            migrationBuilder.CreateIndex(
                name: "IX_Lost_TGBSProcesserId",
                table: "Lost",
                column: "TGBSProcesserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Lost");
        }
    }
}
