using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BDA.Migrations
{
    public partial class InstructionLetter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AddressLine1",
                table: "InstructionLetter",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AddressLine2",
                table: "InstructionLetter",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AddressLine3",
                table: "InstructionLetter",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankPIC",
                table: "InstructionLetter",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RinggitText",
                table: "InstructionLetter",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "InstructionLetter",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ValueDate",
                table: "InstructionLetter",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddressLine1",
                table: "InstructionLetter");

            migrationBuilder.DropColumn(
                name: "AddressLine2",
                table: "InstructionLetter");

            migrationBuilder.DropColumn(
                name: "AddressLine3",
                table: "InstructionLetter");

            migrationBuilder.DropColumn(
                name: "BankPIC",
                table: "InstructionLetter");

            migrationBuilder.DropColumn(
                name: "RinggitText",
                table: "InstructionLetter");

            migrationBuilder.DropColumn(
                name: "State",
                table: "InstructionLetter");

            migrationBuilder.DropColumn(
                name: "ValueDate",
                table: "InstructionLetter");
        }
    }
}
