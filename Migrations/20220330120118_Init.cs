using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace asp_net_po_schedule_management_server.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "persons-table",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    personidentifier = table.Column<string>(name: "person-identifier", type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    personshortcut = table.Column<string>(name: "person-shortcut", type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    applogin = table.Column<string>(name: "app-login", type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    apppassword = table.Column<string>(name: "app-password", type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_persons-table", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "persons-table");
        }
    }
}
