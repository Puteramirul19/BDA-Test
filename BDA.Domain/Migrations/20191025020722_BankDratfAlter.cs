using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BDA.Migrations
{
    public partial class BankDratfAlter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BankDraftAttachment");

            migrationBuilder.DropColumn(
                name: "InstructionLetterEmail",
                table: "BankDraft");

            migrationBuilder.DropColumn(
                name: "IssuedBDReceiverContactNo",
                table: "BankDraft");

            migrationBuilder.DropColumn(
                name: "SendingMethod",
                table: "BankDraft");

            migrationBuilder.DropColumn(
                name: "ValueDate",
                table: "BankDraft");

            migrationBuilder.AddColumn<string>(
                name: "BankAccount",
                table: "WangHangus",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankCountry",
                table: "WangHangus",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BusinessArea",
                table: "WangHangus",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CoCode",
                table: "WangHangus",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "WangHangus",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "WangHangus",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PONumber",
                table: "WangHangus",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VendorName",
                table: "WangHangus",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VendorNo",
                table: "WangHangus",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VendorType",
                table: "WangHangus",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Country",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Country", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Country");

            migrationBuilder.DropColumn(
                name: "BankAccount",
                table: "WangHangus");

            migrationBuilder.DropColumn(
                name: "BankCountry",
                table: "WangHangus");

            migrationBuilder.DropColumn(
                name: "BusinessArea",
                table: "WangHangus");

            migrationBuilder.DropColumn(
                name: "CoCode",
                table: "WangHangus");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "WangHangus");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "WangHangus");

            migrationBuilder.DropColumn(
                name: "PONumber",
                table: "WangHangus");

            migrationBuilder.DropColumn(
                name: "VendorName",
                table: "WangHangus");

            migrationBuilder.DropColumn(
                name: "VendorNo",
                table: "WangHangus");

            migrationBuilder.DropColumn(
                name: "VendorType",
                table: "WangHangus");

            migrationBuilder.AddColumn<string>(
                name: "InstructionLetterEmail",
                table: "BankDraft",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IssuedBDReceiverContactNo",
                table: "BankDraft",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SendingMethod",
                table: "BankDraft",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ValueDate",
                table: "BankDraft",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BankDraftAttachment",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AttachmentId = table.Column<Guid>(nullable: true),
                    BankDraftId = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankDraftAttachment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BankDraftAttachment_Attachment_AttachmentId",
                        column: x => x.AttachmentId,
                        principalTable: "Attachment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BankDraftAttachment_BankDraft_BankDraftId",
                        column: x => x.BankDraftId,
                        principalTable: "BankDraft",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BankDraftAttachment_AttachmentId",
                table: "BankDraftAttachment",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_BankDraftAttachment_BankDraftId",
                table: "BankDraftAttachment",
                column: "BankDraftId");
        }
    }
}
