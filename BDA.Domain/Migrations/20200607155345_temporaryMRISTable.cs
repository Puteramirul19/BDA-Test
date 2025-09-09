using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BDA.Migrations
{
    public partial class temporaryMRISTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MR_BCRM",
                columns: table => new
                {
                    ZID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ZMRID = table.Column<string>(nullable: true),
                    ZEMPID = table.Column<string>(nullable: true),
                    ZDEVICEID = table.Column<string>(nullable: true),
                    ZMDATETIME = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ZAREAD = table.Column<int>(type: "int", nullable: true),
                    ZEREAD = table.Column<int>(type: "int", nullable: true),
                    ZLPCREAD = table.Column<int>(type: "int", nullable: true),
                    ZSREAD = table.Column<int>(type: "int", nullable: true),
                    ZSMRU = table.Column<string>(nullable: true),
                    ZSTATUS = table.Column<string>(nullable: true),
                    ZSDATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ZSUSER = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MR_BCRM", x => x.ZID);
                });

            migrationBuilder.CreateTable(
                name: "MR_BCRM_Delete",
                columns: table => new
                {
                    ZID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ZMRID = table.Column<string>(nullable: true),
                    ZEMPID = table.Column<string>(nullable: true),
                    ZDEVICEID = table.Column<string>(nullable: true),
                    ZMDATETIME = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ZAREAD = table.Column<int>(type: "int", nullable: true),
                    ZEREAD = table.Column<int>(type: "int", nullable: true),
                    ZLPCREAD = table.Column<int>(type: "int", nullable: true),
                    ZSREAD = table.Column<int>(type: "int", nullable: true),
                    ZSMRU = table.Column<string>(nullable: true),
                    ZSTATUS = table.Column<string>(nullable: true),
                    ZSDATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ZSUSER = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MR_BCRM_Delete", x => x.ZID);
                });

            migrationBuilder.CreateTable(
                name: "MR_BCRM_Edit",
                columns: table => new
                {
                    ZID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ZMRID = table.Column<string>(nullable: true),
                    ZEMPID = table.Column<string>(nullable: true),
                    ZDEVICEID = table.Column<string>(nullable: true),
                    ZMDATETIME = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ZAREAD = table.Column<int>(type: "int", nullable: true),
                    ZEREAD = table.Column<int>(type: "int", nullable: true),
                    ZLPCREAD = table.Column<int>(type: "int", nullable: true),
                    ZSREAD = table.Column<int>(type: "int", nullable: true),
                    ZSMRU = table.Column<string>(nullable: true),
                    ZSTATUS = table.Column<string>(nullable: true),
                    ZSDATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ZSUSER = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MR_BCRM_Edit", x => x.ZID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MR_BCRM");

            migrationBuilder.DropTable(
                name: "MR_BCRM_Delete");

            migrationBuilder.DropTable(
                name: "MR_BCRM_Edit");
        }
    }
}
