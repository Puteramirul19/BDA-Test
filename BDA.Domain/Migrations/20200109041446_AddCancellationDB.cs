using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BDA.Migrations
{
    public partial class AddCancellationDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cancellation",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    isActive = table.Column<bool>(nullable: false),
                    Status = table.Column<string>(nullable: true),
                    RefNo = table.Column<string>(nullable: true),
                    BDNo = table.Column<string>(nullable: true),
                    BDRequesterName = table.Column<string>(nullable: true),
                    RequesterId = table.Column<string>(nullable: true),
                    ReasonCancel = table.Column<string>(nullable: true),
                    DraftedOn = table.Column<DateTime>(nullable: true),
                    SubmittedOn = table.Column<DateTime>(nullable: true),
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
                    ReceivedById = table.Column<string>(nullable: true),
                    ReconReceiverId = table.Column<string>(nullable: true),
                    ReceivedOn = table.Column<DateTime>(nullable: true),
                    TGBSConfirmationComment = table.Column<string>(nullable: true),
                    ComfirmedOn = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cancellation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cancellation_AspNetUsers_ApproverId",
                        column: x => x.ApproverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cancellation_AspNetUsers_ReconReceiverId",
                        column: x => x.ReconReceiverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cancellation_AspNetUsers_RequesterId",
                        column: x => x.RequesterId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cancellation_AspNetUsers_TGBSAcceptanceId",
                        column: x => x.TGBSAcceptanceId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cancellation_AspNetUsers_TGBSProcesserId",
                        column: x => x.TGBSProcesserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cancellation_ApproverId",
                table: "Cancellation",
                column: "ApproverId");

            migrationBuilder.CreateIndex(
                name: "IX_Cancellation_ReconReceiverId",
                table: "Cancellation",
                column: "ReconReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Cancellation_RequesterId",
                table: "Cancellation",
                column: "RequesterId");

            migrationBuilder.CreateIndex(
                name: "IX_Cancellation_TGBSAcceptanceId",
                table: "Cancellation",
                column: "TGBSAcceptanceId");

            migrationBuilder.CreateIndex(
                name: "IX_Cancellation_TGBSProcesserId",
                table: "Cancellation",
                column: "TGBSProcesserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cancellation");
        }
    }
}
