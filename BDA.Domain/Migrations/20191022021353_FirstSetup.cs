using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BDA.Migrations
{
    public partial class FirstSetup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    NeedDivision = table.Column<bool>(nullable: false),
                    NeedFunction = table.Column<bool>(nullable: false),
                    NeedZone = table.Column<bool>(nullable: false),
                    NeedUnit = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    AuthenticationMethod = table.Column<int>(nullable: false),
                    FullName = table.Column<string>(nullable: true),
                    Division = table.Column<string>(nullable: true),
                    Unit = table.Column<string>(nullable: true),
                    OfficeNo = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    LastLogin = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Attachment",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    FileType = table.Column<int>(nullable: false),
                    FileSubType = table.Column<int>(nullable: false),
                    ParentId = table.Column<Guid>(nullable: false),
                    FileId = table.Column<string>(nullable: true),
                    FileName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attachment", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Division",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    isActive = table.Column<bool>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Division", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmailQueues",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    Subject = table.Column<string>(nullable: true),
                    ToAddress = table.Column<string>(nullable: true),
                    ToName = table.Column<string>(nullable: true),
                    Content = table.Column<string>(nullable: true),
                    Attachments = table.Column<string>(nullable: true),
                    ScheduledOn = table.Column<DateTime>(nullable: false),
                    ProcessedOn = table.Column<DateTime>(nullable: false),
                    SentOn = table.Column<DateTime>(nullable: false),
                    ObjectId = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailQueues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InstructionLetter",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    isActive = table.Column<bool>(nullable: false),
                    BankName = table.Column<string>(nullable: true),
                    BankAccount = table.Column<string>(nullable: true),
                    ChargedBankAccount = table.Column<string>(nullable: true),
                    Remarks = table.Column<string>(nullable: true),
                    CostObject = table.Column<string>(nullable: true),
                    TaxCode = table.Column<string>(nullable: true),
                    Currency = table.Column<string>(nullable: true),
                    TaxAmount = table.Column<string>(nullable: true),
                    Amount = table.Column<string>(nullable: true),
                    ReferenceNo = table.Column<string>(nullable: true),
                    LetterDate = table.Column<DateTime>(nullable: true),
                    LetterRefNo = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstructionLetter", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Memo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    isActive = table.Column<bool>(nullable: false),
                    CoverRefNo = table.Column<string>(nullable: true),
                    ReferenceNo = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Memo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RunningNo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    isActive = table.Column<bool>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    RunNo = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RunningNo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    RoleId1 = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId1",
                        column: x => x.RoleId1,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    UserId1 = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId1",
                        column: x => x.UserId1,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false),
                    UserId1 = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId1",
                        column: x => x.UserId1,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(maxLength: 128, nullable: false),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    Value = table.Column<string>(nullable: true),
                    UserId1 = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId1",
                        column: x => x.UserId1,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BankDraft",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    Status = table.Column<string>(nullable: true),
                    RefNo = table.Column<string>(nullable: true),
                    RequesterId = table.Column<string>(nullable: true),
                    DraftedOn = table.Column<DateTime>(nullable: false),
                    SubmittedOn = table.Column<DateTime>(nullable: false),
                    VerifierComment = table.Column<string>(nullable: true),
                    VerifierId = table.Column<string>(nullable: true),
                    VerifiedOn = table.Column<DateTime>(nullable: false),
                    ApproverComment = table.Column<string>(nullable: true),
                    ApproverId = table.Column<string>(nullable: true),
                    ApprovedOn = table.Column<DateTime>(nullable: false),
                    RequestorComment = table.Column<string>(nullable: true),
                    TGBSAcceptanceId = table.Column<string>(nullable: true),
                    TGBSAcceptedOn = table.Column<DateTime>(nullable: false),
                    InstructionLetterEmail = table.Column<string>(nullable: true),
                    ValueDate = table.Column<DateTime>(nullable: false),
                    TGBSProcesserId = table.Column<string>(nullable: true),
                    TGBSProcessedOn = table.Column<DateTime>(nullable: false),
                    SendingMethod = table.Column<string>(nullable: true),
                    BankDrafNoIssued = table.Column<string>(nullable: true),
                    IssuedBDReceiverContactNo = table.Column<string>(nullable: true),
                    TGBSIssuerId = table.Column<string>(nullable: true),
                    TGBSIssuedOn = table.Column<DateTime>(nullable: false),
                    RequesterComment = table.Column<string>(nullable: true),
                    ReceiveBankDraftDate = table.Column<DateTime>(nullable: false),
                    ReceiptNo = table.Column<string>(nullable: true),
                    CompletedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankDraft", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BankDraft_AspNetUsers_ApproverId",
                        column: x => x.ApproverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BankDraft_AspNetUsers_RequesterId",
                        column: x => x.RequesterId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BankDraft_AspNetUsers_TGBSAcceptanceId",
                        column: x => x.TGBSAcceptanceId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BankDraft_AspNetUsers_TGBSIssuerId",
                        column: x => x.TGBSIssuerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BankDraft_AspNetUsers_TGBSProcesserId",
                        column: x => x.TGBSProcesserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BankDraft_AspNetUsers_VerifierId",
                        column: x => x.VerifierId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Function",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    isActive = table.Column<bool>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    DivisionId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Function", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Function_Division_DivisionId",
                        column: x => x.DivisionId,
                        principalTable: "Division",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InstructionLetterAttachment",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    InstructionLetterId = table.Column<Guid>(nullable: false),
                    Type = table.Column<string>(nullable: true),
                    AttachmentId = table.Column<Guid>(nullable: true)
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
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    MemoId = table.Column<Guid>(nullable: false),
                    Type = table.Column<string>(nullable: true),
                    AttachmentId = table.Column<Guid>(nullable: true)
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
                name: "BankDraftAction",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    BankDraftId = table.Column<Guid>(nullable: false),
                    ActionRole = table.Column<int>(nullable: false),
                    On = table.Column<DateTime>(nullable: false),
                    ById = table.Column<string>(nullable: true),
                    ActionType = table.Column<int>(nullable: false),
                    Comment = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankDraftAction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BankDraftAction_BankDraft_BankDraftId",
                        column: x => x.BankDraftId,
                        principalTable: "BankDraft",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BankDraftAction_AspNetUsers_ById",
                        column: x => x.ById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BankDraftAttachment",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    BankDraftId = table.Column<Guid>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    AttachmentId = table.Column<Guid>(nullable: true)
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

            migrationBuilder.CreateTable(
                name: "ERMSQueues",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    BankDrafIdId = table.Column<Guid>(nullable: true),
                    RefNo = table.Column<string>(nullable: true),
                    Function = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    ActionDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ERMSQueues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ERMSQueues_BankDraft_BankDrafIdId",
                        column: x => x.BankDrafIdId,
                        principalTable: "BankDraft",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WangCagaran",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    BankDraftId = table.Column<Guid>(nullable: false),
                    ErmsDocNo = table.Column<string>(nullable: true),
                    Pemula = table.Column<string>(nullable: true),
                    Tarikh = table.Column<DateTime>(nullable: false),
                    Alamat1 = table.Column<string>(nullable: true),
                    Alamat2 = table.Column<string>(nullable: true),
                    Bandar = table.Column<string>(nullable: true),
                    Poskod = table.Column<string>(nullable: true),
                    Negeri = table.Column<string>(nullable: true),
                    KeteranganKerja = table.Column<string>(nullable: true),
                    JKRInvolved = table.Column<bool>(nullable: false),
                    JKRType = table.Column<int>(nullable: false),
                    Jumlah = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CajKod = table.Column<string>(nullable: true),
                    NamaPemegangCagaran = table.Column<string>(nullable: true),
                    WBSProjekNo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WangCagaran", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WangCagaran_BankDraft_BankDraftId",
                        column: x => x.BankDraftId,
                        principalTable: "BankDraft",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WangHangus",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    BankDraftId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    ICNo = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    SSTRegNo = table.Column<string>(nullable: true),
                    BusRegNo = table.Column<string>(nullable: true),
                    Street = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    Region = table.Column<string>(nullable: true),
                    Postcode = table.Column<string>(nullable: true),
                    Country = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WangHangus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WangHangus_BankDraft_BankDraftId",
                        column: x => x.BankDraftId,
                        principalTable: "BankDraft",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Zone",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    isActive = table.Column<bool>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    FunctionId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zone", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Zone_Function_FunctionId",
                        column: x => x.FunctionId,
                        principalTable: "Function",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WangCagaranAttachment",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    WangCagaranId = table.Column<Guid>(nullable: false),
                    Type = table.Column<string>(nullable: true),
                    AttachmentId = table.Column<Guid>(nullable: true)
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

            migrationBuilder.CreateTable(
                name: "AccountingTable",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    WangHangusId = table.Column<Guid>(nullable: false),
                    DrCr = table.Column<string>(nullable: true),
                    GLAccount = table.Column<string>(nullable: true),
                    CONW = table.Column<string>(nullable: true),
                    CONWNo = table.Column<string>(nullable: true),
                    CostObject = table.Column<string>(nullable: true),
                    TaxCode = table.Column<string>(nullable: true),
                    Currency = table.Column<string>(nullable: true),
                    TaxAmount = table.Column<string>(nullable: true),
                    Amount = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountingTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountingTable_WangHangus_WangHangusId",
                        column: x => x.WangHangusId,
                        principalTable: "WangHangus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Unit",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    isActive = table.Column<bool>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    ZoneId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Unit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Unit_Zone_ZoneId",
                        column: x => x.ZoneId,
                        principalTable: "Zone",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false),
                    UserId1 = table.Column<string>(nullable: true),
                    RoleId1 = table.Column<string>(nullable: true),
                    DivisionId = table.Column<Guid>(nullable: true),
                    FunctionId = table.Column<Guid>(nullable: true),
                    ZoneId = table.Column<Guid>(nullable: true),
                    UnitId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_Division_DivisionId",
                        column: x => x.DivisionId,
                        principalTable: "Division",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_Function_FunctionId",
                        column: x => x.FunctionId,
                        principalTable: "Function",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId1",
                        column: x => x.RoleId1,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_Unit_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Unit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId1",
                        column: x => x.UserId1,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_Zone_ZoneId",
                        column: x => x.ZoneId,
                        principalTable: "Zone",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountingTable_WangHangusId",
                table: "AccountingTable",
                column: "WangHangusId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId1",
                table: "AspNetRoleClaims",
                column: "RoleId1");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId1",
                table: "AspNetUserClaims",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId1",
                table: "AspNetUserLogins",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_DivisionId",
                table: "AspNetUserRoles",
                column: "DivisionId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_FunctionId",
                table: "AspNetUserRoles",
                column: "FunctionId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId1",
                table: "AspNetUserRoles",
                column: "RoleId1");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_UnitId",
                table: "AspNetUserRoles",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_UserId1",
                table: "AspNetUserRoles",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_ZoneId",
                table: "AspNetUserRoles",
                column: "ZoneId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserTokens_UserId1",
                table: "AspNetUserTokens",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_BankDraft_ApproverId",
                table: "BankDraft",
                column: "ApproverId");

            migrationBuilder.CreateIndex(
                name: "IX_BankDraft_RequesterId",
                table: "BankDraft",
                column: "RequesterId");

            migrationBuilder.CreateIndex(
                name: "IX_BankDraft_TGBSAcceptanceId",
                table: "BankDraft",
                column: "TGBSAcceptanceId");

            migrationBuilder.CreateIndex(
                name: "IX_BankDraft_TGBSIssuerId",
                table: "BankDraft",
                column: "TGBSIssuerId");

            migrationBuilder.CreateIndex(
                name: "IX_BankDraft_TGBSProcesserId",
                table: "BankDraft",
                column: "TGBSProcesserId");

            migrationBuilder.CreateIndex(
                name: "IX_BankDraft_VerifierId",
                table: "BankDraft",
                column: "VerifierId");

            migrationBuilder.CreateIndex(
                name: "IX_BankDraftAction_BankDraftId",
                table: "BankDraftAction",
                column: "BankDraftId");

            migrationBuilder.CreateIndex(
                name: "IX_BankDraftAction_ById",
                table: "BankDraftAction",
                column: "ById");

            migrationBuilder.CreateIndex(
                name: "IX_BankDraftAttachment_AttachmentId",
                table: "BankDraftAttachment",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_BankDraftAttachment_BankDraftId",
                table: "BankDraftAttachment",
                column: "BankDraftId");

            migrationBuilder.CreateIndex(
                name: "IX_ERMSQueues_BankDrafIdId",
                table: "ERMSQueues",
                column: "BankDrafIdId");

            migrationBuilder.CreateIndex(
                name: "IX_Function_DivisionId",
                table: "Function",
                column: "DivisionId");

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
                name: "IX_Unit_ZoneId",
                table: "Unit",
                column: "ZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_WangCagaran_BankDraftId",
                table: "WangCagaran",
                column: "BankDraftId");

            migrationBuilder.CreateIndex(
                name: "IX_WangCagaranAttachment_AttachmentId",
                table: "WangCagaranAttachment",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_WangCagaranAttachment_WangCagaranId",
                table: "WangCagaranAttachment",
                column: "WangCagaranId");

            migrationBuilder.CreateIndex(
                name: "IX_WangHangus_BankDraftId",
                table: "WangHangus",
                column: "BankDraftId");

            migrationBuilder.CreateIndex(
                name: "IX_Zone_FunctionId",
                table: "Zone",
                column: "FunctionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountingTable");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "BankDraftAction");

            migrationBuilder.DropTable(
                name: "BankDraftAttachment");

            migrationBuilder.DropTable(
                name: "EmailQueues");

            migrationBuilder.DropTable(
                name: "ERMSQueues");

            migrationBuilder.DropTable(
                name: "InstructionLetterAttachment");

            migrationBuilder.DropTable(
                name: "MemoAttachment");

            migrationBuilder.DropTable(
                name: "RunningNo");

            migrationBuilder.DropTable(
                name: "WangCagaranAttachment");

            migrationBuilder.DropTable(
                name: "WangHangus");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Unit");

            migrationBuilder.DropTable(
                name: "InstructionLetter");

            migrationBuilder.DropTable(
                name: "Memo");

            migrationBuilder.DropTable(
                name: "Attachment");

            migrationBuilder.DropTable(
                name: "WangCagaran");

            migrationBuilder.DropTable(
                name: "Zone");

            migrationBuilder.DropTable(
                name: "BankDraft");

            migrationBuilder.DropTable(
                name: "Function");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Division");
        }
    }
}
