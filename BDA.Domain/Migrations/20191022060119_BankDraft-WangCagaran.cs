using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BDA.Migrations
{
    public partial class BankDraftWangCagaran : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankDraft_AspNetUsers_ApproverId",
                table: "BankDraft");

            migrationBuilder.DropIndex(
                name: "IX_BankDraft_ApproverId",
                table: "BankDraft");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "WangCagaran",
                newName: "isActive");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "BankDraft",
                newName: "isActive");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Tarikh",
                table: "WangCagaran",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<decimal>(
                name: "Jumlah",
                table: "WangCagaran",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "JKRType",
                table: "WangCagaran",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<bool>(
                name: "JKRInvolved",
                table: "WangCagaran",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<DateTime>(
                name: "VerifiedOn",
                table: "BankDraft",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ValueDate",
                table: "BankDraft",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTime>(
                name: "TGBSProcessedOn",
                table: "BankDraft",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTime>(
                name: "TGBSIssuedOn",
                table: "BankDraft",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTime>(
                name: "TGBSAcceptedOn",
                table: "BankDraft",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTime>(
                name: "SubmittedOn",
                table: "BankDraft",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReceiveBankDraftDate",
                table: "BankDraft",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTime>(
                name: "DraftedOn",
                table: "BankDraft",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CompletedOn",
                table: "BankDraft",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<string>(
                name: "ApproverId",
                table: "BankDraft",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ApprovedOn",
                table: "BankDraft",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "BankDraft",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "BankDraft");

            migrationBuilder.RenameColumn(
                name: "isActive",
                table: "WangCagaran",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "isActive",
                table: "BankDraft",
                newName: "IsActive");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Tarikh",
                table: "WangCagaran",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Jumlah",
                table: "WangCagaran",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "JKRType",
                table: "WangCagaran",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "JKRInvolved",
                table: "WangCagaran",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "VerifiedOn",
                table: "BankDraft",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ValueDate",
                table: "BankDraft",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "TGBSProcessedOn",
                table: "BankDraft",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "TGBSIssuedOn",
                table: "BankDraft",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "TGBSAcceptedOn",
                table: "BankDraft",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "SubmittedOn",
                table: "BankDraft",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReceiveBankDraftDate",
                table: "BankDraft",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DraftedOn",
                table: "BankDraft",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CompletedOn",
                table: "BankDraft",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ApproverId",
                table: "BankDraft",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ApprovedOn",
                table: "BankDraft",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BankDraft_ApproverId",
                table: "BankDraft",
                column: "ApproverId");

            migrationBuilder.AddForeignKey(
                name: "FK_BankDraft_AspNetUsers_ApproverId",
                table: "BankDraft",
                column: "ApproverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
