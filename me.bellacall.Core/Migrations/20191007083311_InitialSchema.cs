using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace me.bellacall.Core.Migrations
{
    public partial class InitialSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PermissibleLevel = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Providers",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 256, nullable: false),
                    Code = table.Column<string>(maxLength: 32, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Providers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Regions",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 256, nullable: false),
                    Code = table.Column<string>(maxLength: 32, nullable: true),
                    TimeZone = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Regions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tariffs",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    Description = table.Column<string>(nullable: true),
                    AllowEnjoy = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tariffs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<long>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRolePermissions",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<long>(nullable: false),
                    TableName = table.Column<string>(maxLength: 64, nullable: false),
                    Operation = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRolePermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRolePermissions_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    Company_Id = table.Column<long>(nullable: false),
                    Level = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Companies_Company_Id",
                        column: x => x.Company_Id,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompanyIncomes",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Company_Id = table.Column<long>(nullable: false),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    Note = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyIncomes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyIncomes_Companies_Company_Id",
                        column: x => x.Company_Id,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DEFs",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(maxLength: 16, nullable: false),
                    Provider_Id = table.Column<long>(nullable: false),
                    Region_Id = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DEFs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DEFs_Providers_Provider_Id",
                        column: x => x.Provider_Id,
                        principalTable: "Providers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DEFs_Regions_Region_Id",
                        column: x => x.Region_Id,
                        principalTable: "Regions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MNPs",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Number = table.Column<string>(maxLength: 16, nullable: false),
                    Provider_Id = table.Column<long>(nullable: false),
                    Region_Id = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MNPs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MNPs_Providers_Provider_Id",
                        column: x => x.Provider_Id,
                        principalTable: "Providers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MNPs_Regions_Region_Id",
                        column: x => x.Region_Id,
                        principalTable: "Regions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Campaigns",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Company_Id = table.Column<long>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: false),
                    Tariff_Id = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Campaigns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Campaigns_Companies_Company_Id",
                        column: x => x.Company_Id,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Campaigns_Tariffs_Tariff_Id",
                        column: x => x.Tariff_Id,
                        principalTable: "Tariffs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TariffConditions",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Tariff_Id = table.Column<long>(nullable: false),
                    DateStart = table.Column<DateTime>(nullable: false),
                    DateStop = table.Column<DateTime>(nullable: false),
                    Rule = table.Column<int>(nullable: false),
                    Unit = table.Column<int>(nullable: false),
                    Price = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TariffConditions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TariffConditions_Tariffs_Tariff_Id",
                        column: x => x.Tariff_Id,
                        principalTable: "Tariffs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<long>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
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
                    UserId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<long>(nullable: false),
                    LoginProvider = table.Column<string>(maxLength: 128, nullable: false),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogs",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<long>(nullable: false),
                    Campaign_Id = table.Column<long>(nullable: true),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                    TableName = table.Column<string>(maxLength: 50, nullable: false),
                    Operation = table.Column<int>(nullable: false),
                    Data_Id = table.Column<long>(nullable: true),
                    Data_Json = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserLogs_Campaigns_Campaign_Id",
                        column: x => x.Campaign_Id,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AspNetUserLogs_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<long>(nullable: false),
                    RoleId = table.Column<long>(nullable: false),
                    Campaign_Id = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_Campaigns_Campaign_Id",
                        column: x => x.Campaign_Id,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ContactGroups",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Campaign_Id = table.Column<long>(nullable: false),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    Description = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContactGroups_Campaigns_Campaign_Id",
                        column: x => x.Campaign_Id,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ContactProperties",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    Caption = table.Column<string>(maxLength: 128, nullable: false),
                    Type = table.Column<int>(nullable: false),
                    Campaign_Id = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactProperties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContactProperties_Campaigns_Campaign_Id",
                        column: x => x.Campaign_Id,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Gateways",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Campaign_Id = table.Column<long>(nullable: false),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    Description = table.Column<string>(maxLength: 256, nullable: true),
                    RegistrationType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gateways", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Gateways_Campaigns_Campaign_Id",
                        column: x => x.Campaign_Id,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Jobs",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Campaign_Id = table.Column<long>(nullable: false),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    Region_Id = table.Column<long>(nullable: false),
                    AllowJob = table.Column<bool>(nullable: false),
                    AllowInbox = table.Column<bool>(nullable: false),
                    TimeStart = table.Column<TimeSpan>(nullable: false),
                    TimeStop = table.Column<TimeSpan>(nullable: false),
                    TimeZoneType = table.Column<int>(nullable: false),
                    TimeZoneCustom = table.Column<string>(maxLength: 128, nullable: true),
                    DialDuration = table.Column<int>(nullable: false),
                    DialDensity = table.Column<int>(nullable: false),
                    DialEfforts = table.Column<int>(nullable: false),
                    DialInterval = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Jobs_Campaigns_Campaign_Id",
                        column: x => x.Campaign_Id,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Jobs_Regions_Region_Id",
                        column: x => x.Region_Id,
                        principalTable: "Regions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LeadProperties",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    Caption = table.Column<string>(maxLength: 128, nullable: false),
                    Type = table.Column<int>(nullable: false),
                    Campaign_Id = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeadProperties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeadProperties_Campaigns_Campaign_Id",
                        column: x => x.Campaign_Id,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Leads",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Campaign_Id = table.Column<long>(nullable: false),
                    Properties = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Leads_Campaigns_Campaign_Id",
                        column: x => x.Campaign_Id,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Scripts",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Campaign_Id = table.Column<long>(nullable: false),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scripts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Scripts_Campaigns_Campaign_Id",
                        column: x => x.Campaign_Id,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompanyExpenses",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Company_Id = table.Column<long>(nullable: false),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                    TariffCondition_Id = table.Column<long>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    Note = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyExpenses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyExpenses_Companies_Company_Id",
                        column: x => x.Company_Id,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompanyExpenses_TariffConditions_TariffCondition_Id",
                        column: x => x.TariffCondition_Id,
                        principalTable: "TariffConditions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Contacts",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ContactGroup_Id = table.Column<long>(nullable: false),
                    Phone = table.Column<string>(maxLength: 32, nullable: false),
                    Email = table.Column<string>(maxLength: 128, nullable: true),
                    BirthDate = table.Column<DateTime>(nullable: true),
                    Gender = table.Column<int>(nullable: true),
                    LastName = table.Column<string>(maxLength: 32, nullable: true),
                    FirstName = table.Column<string>(maxLength: 32, nullable: true),
                    Patronymic = table.Column<string>(maxLength: 32, nullable: true),
                    Properties = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contacts_ContactGroups_ContactGroup_Id",
                        column: x => x.ContactGroup_Id,
                        principalTable: "ContactGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GatewayStreams",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Gateway_Id = table.Column<long>(nullable: false),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    UserName = table.Column<string>(maxLength: 32, nullable: false),
                    Password = table.Column<string>(maxLength: 32, nullable: false),
                    TrunkType = table.Column<int>(nullable: false),
                    TrunkCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GatewayStreams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GatewayStreams_Gateways_Gateway_Id",
                        column: x => x.Gateway_Id,
                        principalTable: "Gateways",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InboxScripts",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Job_Id = table.Column<long>(nullable: false),
                    Script_Id = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InboxScripts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InboxScripts_Jobs_Job_Id",
                        column: x => x.Job_Id,
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InboxScripts_Scripts_Script_Id",
                        column: x => x.Script_Id,
                        principalTable: "Scripts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JobScripts",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Job_Id = table.Column<long>(nullable: false),
                    Script_Id = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobScripts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobScripts_Jobs_Job_Id",
                        column: x => x.Job_Id,
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobScripts_Scripts_Script_Id",
                        column: x => x.Script_Id,
                        principalTable: "Scripts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ScriptElements",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Script_Id = table.Column<long>(nullable: false),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Function = table.Column<string>(maxLength: 128, nullable: false),
                    XPos = table.Column<int>(nullable: false),
                    YPos = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScriptElements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScriptElements_Scripts_Script_Id",
                        column: x => x.Script_Id,
                        principalTable: "Scripts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ScriptProperties",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Script_Id = table.Column<long>(nullable: false),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    DataType = table.Column<int>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScriptProperties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScriptProperties_Scripts_Script_Id",
                        column: x => x.Script_Id,
                        principalTable: "Scripts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ScriptVariables",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Script_Id = table.Column<long>(nullable: false),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    DataType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScriptVariables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScriptVariables_Scripts_Script_Id",
                        column: x => x.Script_Id,
                        principalTable: "Scripts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JobContacts",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Job_Id = table.Column<long>(nullable: false),
                    Contact_Id = table.Column<long>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    LastAttemptDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobContacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobContacts_Contacts_Contact_Id",
                        column: x => x.Contact_Id,
                        principalTable: "Contacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobContacts_Jobs_Job_Id",
                        column: x => x.Job_Id,
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LeadContacts",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Lead_Id = table.Column<long>(nullable: false),
                    Contact_Id = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeadContacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeadContacts_Contacts_Contact_Id",
                        column: x => x.Contact_Id,
                        principalTable: "Contacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LeadContacts_Leads_Lead_Id",
                        column: x => x.Lead_Id,
                        principalTable: "Leads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ScriptConditions",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ScriptElement_Id = table.Column<long>(nullable: false),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    CaseExpression = table.Column<string>(nullable: true),
                    CaseElement_Id = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScriptConditions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScriptConditions_ScriptElements_CaseElement_Id",
                        column: x => x.CaseElement_Id,
                        principalTable: "ScriptElements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ScriptConditions_ScriptElements_ScriptElement_Id",
                        column: x => x.ScriptElement_Id,
                        principalTable: "ScriptElements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ScriptInputParameters",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ScriptElement_Id = table.Column<long>(nullable: false),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    DataType = table.Column<int>(nullable: false),
                    Expression = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScriptInputParameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScriptInputParameters_ScriptElements_ScriptElement_Id",
                        column: x => x.ScriptElement_Id,
                        principalTable: "ScriptElements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ScriptOutputParameters",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ScriptElement_Id = table.Column<long>(nullable: false),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    DataType = table.Column<int>(nullable: false),
                    Expression = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScriptOutputParameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScriptOutputParameters_ScriptElements_ScriptElement_Id",
                        column: x => x.ScriptElement_Id,
                        principalTable: "ScriptElements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRolePermissions_RoleId",
                table: "AspNetRolePermissions",
                column: "RoleId");

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
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogs_Campaign_Id",
                table: "AspNetUserLogs",
                column: "Campaign_Id");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogs_TimeStamp",
                table: "AspNetUserLogs",
                column: "TimeStamp");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogs_UserId",
                table: "AspNetUserLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_Campaign_Id",
                table: "AspNetUserRoles",
                column: "Campaign_Id");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_UserId",
                table: "AspNetUserRoles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Company_Id",
                table: "AspNetUsers",
                column: "Company_Id");

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
                name: "IX_Campaigns_Company_Id",
                table: "Campaigns",
                column: "Company_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_Tariff_Id",
                table: "Campaigns",
                column: "Tariff_Id");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyExpenses_Company_Id",
                table: "CompanyExpenses",
                column: "Company_Id");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyExpenses_TariffCondition_Id",
                table: "CompanyExpenses",
                column: "TariffCondition_Id");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyIncomes_Company_Id",
                table: "CompanyIncomes",
                column: "Company_Id");

            migrationBuilder.CreateIndex(
                name: "IX_ContactGroups_Campaign_Id",
                table: "ContactGroups",
                column: "Campaign_Id");

            migrationBuilder.CreateIndex(
                name: "IX_ContactProperties_Campaign_Id",
                table: "ContactProperties",
                column: "Campaign_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_ContactGroup_Id",
                table: "Contacts",
                column: "ContactGroup_Id");

            migrationBuilder.CreateIndex(
                name: "IX_DEFs_Provider_Id",
                table: "DEFs",
                column: "Provider_Id");

            migrationBuilder.CreateIndex(
                name: "IX_DEFs_Region_Id",
                table: "DEFs",
                column: "Region_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Gateways_Campaign_Id",
                table: "Gateways",
                column: "Campaign_Id");

            migrationBuilder.CreateIndex(
                name: "IX_GatewayStreams_Gateway_Id",
                table: "GatewayStreams",
                column: "Gateway_Id");

            migrationBuilder.CreateIndex(
                name: "IX_InboxScripts_Job_Id",
                table: "InboxScripts",
                column: "Job_Id");

            migrationBuilder.CreateIndex(
                name: "IX_InboxScripts_Script_Id",
                table: "InboxScripts",
                column: "Script_Id");

            migrationBuilder.CreateIndex(
                name: "IX_JobContacts_Contact_Id",
                table: "JobContacts",
                column: "Contact_Id");

            migrationBuilder.CreateIndex(
                name: "IX_JobContacts_Job_Id",
                table: "JobContacts",
                column: "Job_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_Campaign_Id",
                table: "Jobs",
                column: "Campaign_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_Region_Id",
                table: "Jobs",
                column: "Region_Id");

            migrationBuilder.CreateIndex(
                name: "IX_JobScripts_Job_Id",
                table: "JobScripts",
                column: "Job_Id");

            migrationBuilder.CreateIndex(
                name: "IX_JobScripts_Script_Id",
                table: "JobScripts",
                column: "Script_Id");

            migrationBuilder.CreateIndex(
                name: "IX_LeadContacts_Contact_Id",
                table: "LeadContacts",
                column: "Contact_Id");

            migrationBuilder.CreateIndex(
                name: "IX_LeadContacts_Lead_Id",
                table: "LeadContacts",
                column: "Lead_Id");

            migrationBuilder.CreateIndex(
                name: "IX_LeadProperties_Campaign_Id",
                table: "LeadProperties",
                column: "Campaign_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Leads_Campaign_Id",
                table: "Leads",
                column: "Campaign_Id");

            migrationBuilder.CreateIndex(
                name: "IX_MNPs_Provider_Id",
                table: "MNPs",
                column: "Provider_Id");

            migrationBuilder.CreateIndex(
                name: "IX_MNPs_Region_Id",
                table: "MNPs",
                column: "Region_Id");

            migrationBuilder.CreateIndex(
                name: "IX_ScriptConditions_CaseElement_Id",
                table: "ScriptConditions",
                column: "CaseElement_Id");

            migrationBuilder.CreateIndex(
                name: "IX_ScriptConditions_ScriptElement_Id",
                table: "ScriptConditions",
                column: "ScriptElement_Id");

            migrationBuilder.CreateIndex(
                name: "IX_ScriptElements_Script_Id",
                table: "ScriptElements",
                column: "Script_Id");

            migrationBuilder.CreateIndex(
                name: "IX_ScriptInputParameters_ScriptElement_Id",
                table: "ScriptInputParameters",
                column: "ScriptElement_Id");

            migrationBuilder.CreateIndex(
                name: "IX_ScriptOutputParameters_ScriptElement_Id",
                table: "ScriptOutputParameters",
                column: "ScriptElement_Id");

            migrationBuilder.CreateIndex(
                name: "IX_ScriptProperties_Script_Id",
                table: "ScriptProperties",
                column: "Script_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Scripts_Campaign_Id",
                table: "Scripts",
                column: "Campaign_Id");

            migrationBuilder.CreateIndex(
                name: "IX_ScriptVariables_Script_Id",
                table: "ScriptVariables",
                column: "Script_Id");

            migrationBuilder.CreateIndex(
                name: "IX_TariffConditions_Tariff_Id",
                table: "TariffConditions",
                column: "Tariff_Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetRolePermissions");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserLogs");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "CompanyExpenses");

            migrationBuilder.DropTable(
                name: "CompanyIncomes");

            migrationBuilder.DropTable(
                name: "ContactProperties");

            migrationBuilder.DropTable(
                name: "DEFs");

            migrationBuilder.DropTable(
                name: "GatewayStreams");

            migrationBuilder.DropTable(
                name: "InboxScripts");

            migrationBuilder.DropTable(
                name: "JobContacts");

            migrationBuilder.DropTable(
                name: "JobScripts");

            migrationBuilder.DropTable(
                name: "LeadContacts");

            migrationBuilder.DropTable(
                name: "LeadProperties");

            migrationBuilder.DropTable(
                name: "MNPs");

            migrationBuilder.DropTable(
                name: "ScriptConditions");

            migrationBuilder.DropTable(
                name: "ScriptInputParameters");

            migrationBuilder.DropTable(
                name: "ScriptOutputParameters");

            migrationBuilder.DropTable(
                name: "ScriptProperties");

            migrationBuilder.DropTable(
                name: "ScriptVariables");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "TariffConditions");

            migrationBuilder.DropTable(
                name: "Gateways");

            migrationBuilder.DropTable(
                name: "Jobs");

            migrationBuilder.DropTable(
                name: "Contacts");

            migrationBuilder.DropTable(
                name: "Leads");

            migrationBuilder.DropTable(
                name: "Providers");

            migrationBuilder.DropTable(
                name: "ScriptElements");

            migrationBuilder.DropTable(
                name: "Regions");

            migrationBuilder.DropTable(
                name: "ContactGroups");

            migrationBuilder.DropTable(
                name: "Scripts");

            migrationBuilder.DropTable(
                name: "Campaigns");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropTable(
                name: "Tariffs");
        }
    }
}
