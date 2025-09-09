using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BDA.Migrations
{
    public partial class Memo_BankDraft : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationType",
                table: "Memo",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Approver",
                table: "Memo",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApproverAddress",
                table: "Memo",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Line1",
                table: "Memo",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Line2",
                table: "Memo",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Line3",
                table: "Memo",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Line4",
                table: "Memo",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Line5",
                table: "Memo",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LineETC",
                table: "Memo",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequestorAddress",
                table: "Memo",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Signiture",
                table: "Memo",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationType",
                table: "InstructionLetter",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "BankDraftAmount",
                table: "BankDraft",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<DateTime>(
                name: "BankDraftDate",
                table: "BankDraft",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplicationType",
                table: "Memo");

            migrationBuilder.DropColumn(
                name: "Approver",
                table: "Memo");

            migrationBuilder.DropColumn(
                name: "ApproverAddress",
                table: "Memo");

            migrationBuilder.DropColumn(
                name: "Line1",
                table: "Memo");

            migrationBuilder.DropColumn(
                name: "Line2",
                table: "Memo");

            migrationBuilder.DropColumn(
                name: "Line3",
                table: "Memo");

            migrationBuilder.DropColumn(
                name: "Line4",
                table: "Memo");

            migrationBuilder.DropColumn(
                name: "Line5",
                table: "Memo");

            migrationBuilder.DropColumn(
                name: "LineETC",
                table: "Memo");

            migrationBuilder.DropColumn(
                name: "RequestorAddress",
                table: "Memo");

            migrationBuilder.DropColumn(
                name: "Signiture",
                table: "Memo");

            migrationBuilder.DropColumn(
                name: "ApplicationType",
                table: "InstructionLetter");

            migrationBuilder.DropColumn(
                name: "BankDraftAmount",
                table: "BankDraft");

            migrationBuilder.DropColumn(
                name: "BankDraftDate",
                table: "BankDraft");
        }
    }
}
