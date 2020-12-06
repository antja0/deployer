using Microsoft.EntityFrameworkCore.Migrations;

namespace Deployer.Data.Migrations
{
    public partial class DeploymentGroupsAndRules : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "DeploymentRules",
                columns: table => new
                {
                    Type = table.Column<int>(type: "int", nullable: false),
                    DeploymentGroupId = table.Column<string>(type: "CHAR(36)", maxLength: 36, nullable: false),
                    ApplicationId = table.Column<string>(type: "CHAR(36)", maxLength: 36, nullable: false),
                    DeployAutomatically = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeploymentRules", x => new { x.Type, x.DeploymentGroupId, x.ApplicationId });
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
                name: "DeploymentGroups");
        }
    }
}
