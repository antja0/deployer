using Microsoft.EntityFrameworkCore.Migrations;

namespace Deployer.Data.Migrations
{
    public partial class FixProjectNodeLinking : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NodeId1",
                table: "Projects",
                type: "CHAR(36)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_NodeId1",
                table: "Projects",
                column: "NodeId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Nodes_NodeId1",
                table: "Projects",
                column: "NodeId1",
                principalTable: "Nodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Nodes_NodeId1",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_NodeId1",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "NodeId1",
                table: "Projects");
        }
    }
}
