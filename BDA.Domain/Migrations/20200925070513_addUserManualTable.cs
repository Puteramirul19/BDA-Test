using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BDA.Migrations
{
    public partial class addUserManualTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MR_BCRM");

            migrationBuilder.DropTable(
                name: "MR_BCRM_Delete");

            migrationBuilder.DropTable(
                name: "MR_BCRM_Edit");

            migrationBuilder.CreateTable(
                name: "UserManual",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    FileName = table.Column<string>(nullable: true),
                    Sequence = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserManual", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserManual");

            migrationBuilder.CreateTable(
                name: "MR_BCRM",
                columns: table => new
                {
                    ZID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ZAREAD = table.Column<int>(type: "int", nullable: true),
                    ZDEVICEID = table.Column<string>(nullable: true),
                    ZEMPID = table.Column<string>(nullable: true),
                    ZEREAD = table.Column<int>(type: "int", nullable: true),
                    ZLPCREAD = table.Column<int>(type: "int", nullable: true),
                    ZMDATETIME = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ZMRID = table.Column<string>(nullable: true),
                    ZSDATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ZSMRU = table.Column<string>(nullable: true),
                    ZSREAD = table.Column<int>(type: "int", nullable: true),
                    ZSTATUS = table.Column<string>(nullable: true),
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
                    ZAREAD = table.Column<int>(type: "int", nullable: true),
                    ZDEVICEID = table.Column<string>(nullable: true),
                    ZEMPID = table.Column<string>(nullable: true),
                    ZEREAD = table.Column<int>(type: "int", nullable: true),
                    ZLPCREAD = table.Column<int>(type: "int", nullable: true),
                    ZMDATETIME = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ZMRID = table.Column<string>(nullable: true),
                    ZSDATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ZSMRU = table.Column<string>(nullable: true),
                    ZSREAD = table.Column<int>(type: "int", nullable: true),
                    ZSTATUS = table.Column<string>(nullable: true),
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
                    ZAREAD = table.Column<int>(type: "int", nullable: true),
                    ZDEVICEID = table.Column<string>(nullable: true),
                    ZEMPID = table.Column<string>(nullable: true),
                    ZEREAD = table.Column<int>(type: "int", nullable: true),
                    ZLPCREAD = table.Column<int>(type: "int", nullable: true),
                    ZMDATETIME = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ZMRID = table.Column<string>(nullable: true),
                    ZSDATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ZSMRU = table.Column<string>(nullable: true),
                    ZSREAD = table.Column<int>(type: "int", nullable: true),
                    ZSTATUS = table.Column<string>(nullable: true),
                    ZSUSER = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MR_BCRM_Edit", x => x.ZID);
                });
        }
    }
}
