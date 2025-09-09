using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BDA.Migrations
{
    public partial class AddWangCagaranHangusTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WangCagaranHangus",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    isActive = table.Column<bool>(nullable: false),
                    BankDraftId = table.Column<Guid>(nullable: false),
                    ErmsDocNo = table.Column<string>(nullable: true),
                    Pemula = table.Column<string>(nullable: true),
                    Tarikh = table.Column<DateTime>(nullable: true),
                    Alamat1 = table.Column<string>(nullable: true),
                    Alamat2 = table.Column<string>(nullable: true),
                    Bandar = table.Column<string>(nullable: true),
                    Poskod = table.Column<string>(nullable: true),
                    Negeri = table.Column<string>(nullable: true),
                    KeteranganKerja = table.Column<string>(nullable: true),
                    JKRInvolved = table.Column<bool>(nullable: true),
                    JKRType = table.Column<string>(nullable: true),
                    Jumlah = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RMCagaran = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RMHangus = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CajKod = table.Column<string>(nullable: true),
                    NamaPemegangCagaran = table.Column<string>(nullable: true),
                    WBSProjekNo = table.Column<string>(nullable: true),
                    GL = table.Column<string>(nullable: true),
                    CoCode = table.Column<string>(nullable: true),
                    CostCenter = table.Column<string>(nullable: true),
                    Assignment = table.Column<string>(nullable: true),
                    BusinessArea = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    PostingDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WangCagaranHangus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WangCagaranHangus_BankDraft_BankDraftId",
                        column: x => x.BankDraftId,
                        principalTable: "BankDraft",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WangCagaranHangus_BankDraftId",
                table: "WangCagaranHangus",
                column: "BankDraftId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WangCagaranHangus");
        }
    }
}
