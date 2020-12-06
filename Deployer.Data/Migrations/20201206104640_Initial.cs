using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Deployer.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Applications",
                columns: table => new
                {
                    Id = table.Column<string>(type: "CHAR(36)", maxLength: 36, nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR(128)", maxLength: 128, nullable: true),
                    VersionEndpoint = table.Column<string>(type: "NVARCHAR(256)", maxLength: 256, nullable: true),
                    RepositoryUrl = table.Column<string>(type: "NVARCHAR(256)", maxLength: 256, nullable: true),
                    ChangelogPath = table.Column<string>(type: "NVARCHAR(256)", maxLength: 256, nullable: true),
                    ScriptPath = table.Column<string>(type: "NVARCHAR(128)", maxLength: 128, nullable: true),
                    Description = table.Column<string>(type: "NVARCHAR(1024)", maxLength: 1024, nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationVersions",
                columns: table => new
                {
                    Version = table.Column<string>(type: "CHAR(16)", maxLength: 16, nullable: false),
                    ApplicationId = table.Column<string>(type: "CHAR(36)", maxLength: 36, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UnListed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationVersions", x => new { x.Version, x.ApplicationId });
                    table.ForeignKey(
                        name: "FK_ApplicationVersions_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Nodes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "CHAR(36)", maxLength: 36, nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR(128)", maxLength: 128, nullable: true),
                    ApiEndpoint = table.Column<string>(type: "NVARCHAR(256)", maxLength: 256, nullable: true),
                    Description = table.Column<string>(type: "NVARCHAR(1024)", maxLength: 1024, nullable: true),
                    Registered = table.Column<bool>(type: "bit", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    ApplicationId = table.Column<string>(type: "CHAR(36)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Nodes_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<string>(type: "CHAR(36)", maxLength: 36, nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR(512)", maxLength: 512, nullable: true),
                    NodeId = table.Column<string>(type: "CHAR(36)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_Nodes_NodeId",
                        column: x => x.NodeId,
                        principalTable: "Nodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationVersionProject",
                columns: table => new
                {
                    ProjectsId = table.Column<string>(type: "CHAR(36)", nullable: false),
                    ApplicationVersionsVersion = table.Column<string>(type: "CHAR(16)", nullable: false),
                    ApplicationVersionsApplicationId = table.Column<string>(type: "CHAR(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationVersionProject", x => new { x.ProjectsId, x.ApplicationVersionsVersion, x.ApplicationVersionsApplicationId });
                    table.ForeignKey(
                        name: "FK_ApplicationVersionProject_ApplicationVersions_ApplicationVersionsVersion_ApplicationVersionsApplicationId",
                        columns: x => new { x.ApplicationVersionsVersion, x.ApplicationVersionsApplicationId },
                        principalTable: "ApplicationVersions",
                        principalColumns: new[] { "Version", "ApplicationId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationVersionProject_Projects_ProjectsId",
                        column: x => x.ProjectsId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationVersionProject_ApplicationVersionsVersion_ApplicationVersionsApplicationId",
                table: "ApplicationVersionProject",
                columns: new[] { "ApplicationVersionsVersion", "ApplicationVersionsApplicationId" });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationVersions_ApplicationId",
                table: "ApplicationVersions",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Nodes_ApplicationId",
                table: "Nodes",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_NodeId",
                table: "Projects",
                column: "NodeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationVersionProject");

            migrationBuilder.DropTable(
                name: "ApplicationVersions");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "Nodes");

            migrationBuilder.DropTable(
                name: "Applications");
        }
    }
}
