using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BDA.Migrations
{
    public partial class deleteUselessTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InstructionLetterAttachment");

            migrationBuilder.DropTable(
                name: "MemoAttachment");

            migrationBuilder.DropTable(
                name: "WangCagaranAttachment");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InstructionLetterAttachment",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AttachmentId = table.Column<Guid>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    InstructionLetterId = table.Column<Guid>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    Type = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstructionLetterAttachment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InstructionLetterAttachment_Attachment_AttachmentId",
                        column: x => x.AttachmentId,
                        principalTable: "Attachment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InstructionLetterAttachment_InstructionLetter_InstructionLetterId",
                        column: x => x.InstructionLetterId,
                        principalTable: "InstructionLetter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MemoAttachment",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AttachmentId = table.Column<Guid>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    MemoId = table.Column<Guid>(nullable: false),
                    Type = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemoAttachment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MemoAttachment_Attachment_AttachmentId",
                        column: x => x.AttachmentId,
                        principalTable: "Attachment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MemoAttachment_Memo_MemoId",
                        column: x => x.MemoId,
                        principalTable: "Memo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WangCagaranAttachment",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AttachmentId = table.Column<Guid>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    Type = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    WangCagaranId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WangCagaranAttachment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WangCagaranAttachment_Attachment_AttachmentId",
                        column: x => x.AttachmentId,
                        principalTable: "Attachment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WangCagaranAttachment_WangCagaran_WangCagaranId",
                        column: x => x.WangCagaranId,
                        principalTable: "WangCagaran",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InstructionLetterAttachment_AttachmentId",
                table: "InstructionLetterAttachment",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_InstructionLetterAttachment_InstructionLetterId",
                table: "InstructionLetterAttachment",
                column: "InstructionLetterId");

            migrationBuilder.CreateIndex(
                name: "IX_MemoAttachment_AttachmentId",
                table: "MemoAttachment",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_MemoAttachment_MemoId",
                table: "MemoAttachment",
                column: "MemoId");

            migrationBuilder.CreateIndex(
                name: "IX_WangCagaranAttachment_AttachmentId",
                table: "WangCagaranAttachment",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_WangCagaranAttachment_WangCagaranId",
                table: "WangCagaranAttachment",
                column: "WangCagaranId");
        }
    }
}
