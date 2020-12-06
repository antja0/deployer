using Microsoft.EntityFrameworkCore.Migrations;

namespace Deployer.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Nodes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "CHAR(36)", maxLength: 36, nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR(128)", maxLength: 128, nullable: true),
                    ApiEndpoint = table.Column<string>(type: "NVARCHAR(256)", maxLength: 256, nullable: true),
                    Description = table.Column<string>(type: "NVARCHAR(1024)", maxLength: 1024, nullable: true),
                    Registered = table.Column<bool>(type: "bit", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nodes", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Nodes");
        }
    }
}
