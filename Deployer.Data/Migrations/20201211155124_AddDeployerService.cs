using Microsoft.EntityFrameworkCore.Migrations;

namespace Deployer.Data.Migrations
{
    public partial class AddDeployerService : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ScriptPath",
                table: "Applications",
                newName: "Script");

            migrationBuilder.AddColumn<bool>(
                name: "ListNewVersions",
                table: "Events",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "OutputFolder",
                table: "Applications",
                type: "NVARCHAR(128)",
                maxLength: 128,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ListNewVersions",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "OutputFolder",
                table: "Applications");

            migrationBuilder.RenameColumn(
                name: "Script",
                table: "Applications",
                newName: "ScriptPath");
        }
    }
}
