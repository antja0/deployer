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
                name: "DeploymentGroups",
                columns: table => new
                {
                    Id = table.Column<string>(type: "CHAR(36)", maxLength: 36, nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR(512)", maxLength: 512, nullable: true),
                    UpdateAutomatically = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeploymentGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<string>(type: "CHAR(36)", maxLength: 36, nullable: false),
                    EventId = table.Column<string>(type: "NVARCHAR(64)", maxLength: 64, nullable: true),
                    Description = table.Column<string>(type: "NVARCHAR(1024)", maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
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
                name: "DeploymentRules",
                columns: table => new
                {
                    EventId = table.Column<string>(type: "CHAR(36)", maxLength: 36, nullable: false),
                    DeploymentGroupId = table.Column<string>(type: "CHAR(36)", maxLength: 36, nullable: false),
                    ApplicationId = table.Column<string>(type: "CHAR(36)", maxLength: 36, nullable: false),
                    DeployAutomatically = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeploymentRules", x => new { x.EventId, x.DeploymentGroupId, x.ApplicationId });
                    table.ForeignKey(
                        name: "FK_DeploymentRules_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeploymentRules_DeploymentGroups_DeploymentGroupId",
                        column: x => x.DeploymentGroupId,
                        principalTable: "DeploymentGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeploymentRules_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Versions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "CHAR(36)", maxLength: 36, nullable: false),
                    Name = table.Column<string>(type: "CHAR(16)", maxLength: 16, nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UnListed = table.Column<bool>(type: "bit", nullable: false),
                    EventId = table.Column<string>(type: "CHAR(36)", maxLength: 36, nullable: true),
                    ApplicationId = table.Column<string>(type: "CHAR(36)", maxLength: 36, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Versions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Versions_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Versions_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DeploymentGroupNode",
                columns: table => new
                {
                    DeploymentGroupsId = table.Column<string>(type: "CHAR(36)", nullable: false),
                    NodesId = table.Column<string>(type: "CHAR(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeploymentGroupNode", x => new { x.DeploymentGroupsId, x.NodesId });
                    table.ForeignKey(
                        name: "FK_DeploymentGroupNode_DeploymentGroups_DeploymentGroupsId",
                        column: x => x.DeploymentGroupsId,
                        principalTable: "DeploymentGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeploymentGroupNode_Nodes_NodesId",
                        column: x => x.NodesId,
                        principalTable: "Nodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "DeploymentGroupProject",
                columns: table => new
                {
                    DeploymentGroupsId = table.Column<string>(type: "CHAR(36)", nullable: false),
                    ProjectsId = table.Column<string>(type: "CHAR(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeploymentGroupProject", x => new { x.DeploymentGroupsId, x.ProjectsId });
                    table.ForeignKey(
                        name: "FK_DeploymentGroupProject_DeploymentGroups_DeploymentGroupsId",
                        column: x => x.DeploymentGroupsId,
                        principalTable: "DeploymentGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeploymentGroupProject_Projects_ProjectsId",
                        column: x => x.ProjectsId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectVersion",
                columns: table => new
                {
                    ProjectsId = table.Column<string>(type: "CHAR(36)", nullable: false),
                    VersionsId = table.Column<string>(type: "CHAR(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectVersion", x => new { x.ProjectsId, x.VersionsId });
                    table.ForeignKey(
                        name: "FK_ProjectVersion_Projects_ProjectsId",
                        column: x => x.ProjectsId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectVersion_Versions_VersionsId",
                        column: x => x.VersionsId,
                        principalTable: "Versions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeploymentGroupNode_NodesId",
                table: "DeploymentGroupNode",
                column: "NodesId");

            migrationBuilder.CreateIndex(
                name: "IX_DeploymentGroupProject_ProjectsId",
                table: "DeploymentGroupProject",
                column: "ProjectsId");

            migrationBuilder.CreateIndex(
                name: "IX_DeploymentRules_ApplicationId",
                table: "DeploymentRules",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_DeploymentRules_DeploymentGroupId",
                table: "DeploymentRules",
                column: "DeploymentGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_EventId",
                table: "Events",
                column: "EventId",
                unique: true,
                filter: "[EventId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Nodes_ApplicationId",
                table: "Nodes",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_NodeId",
                table: "Projects",
                column: "NodeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectVersion_VersionsId",
                table: "ProjectVersion",
                column: "VersionsId");

            migrationBuilder.CreateIndex(
                name: "IX_Versions_ApplicationId",
                table: "Versions",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Versions_EventId",
                table: "Versions",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Versions_Name_ApplicationId_EventId",
                table: "Versions",
                columns: new[] { "Name", "ApplicationId", "EventId" },
                unique: true,
                filter: "[Name] IS NOT NULL AND [ApplicationId] IS NOT NULL AND [EventId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeploymentGroupNode");

            migrationBuilder.DropTable(
                name: "DeploymentGroupProject");

            migrationBuilder.DropTable(
                name: "DeploymentRules");

            migrationBuilder.DropTable(
                name: "ProjectVersion");

            migrationBuilder.DropTable(
                name: "DeploymentGroups");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "Versions");

            migrationBuilder.DropTable(
                name: "Nodes");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "Applications");
        }
    }
}
