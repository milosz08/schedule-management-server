using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace asp_net_po_schedule_management_server.Migrations
{
    public partial class BaseEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "persons-table",
                columns: table => new
                {
                    primarykey = table.Column<long>(name: "primary-key", type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    personidentifier = table.Column<string>(name: "person-identifier", type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    personshortcut = table.Column<string>(name: "person-shortcut", type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    applogin = table.Column<string>(name: "app-login", type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    apppassword = table.Column<string>(name: "app-password", type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createddate = table.Column<DateTime>(name: "created-date", type: "datetime(6)", nullable: false),
                    updateddate = table.Column<DateTime>(name: "updated-date", type: "datetime(6)", nullable: false),
                    artificianindex = table.Column<string>(name: "artifician-index", type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_persons-table", x => x.primarykey);
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
