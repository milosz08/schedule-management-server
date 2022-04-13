using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace asp_net_po_schedule_management_server.Migrations
{
    public partial class UserWithRolesMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "role",
                columns: table => new
                {
                    primarykey = table.Column<long>(name: "primary-key", type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createddate = table.Column<DateTime>(name: "created-date", type: "datetime(6)", nullable: false),
                    updateddate = table.Column<DateTime>(name: "updated-date", type: "datetime(6)", nullable: false),
                    dictionaryhash = table.Column<string>(name: "dictionary-hash", type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role", x => x.primarykey);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "person-table",
                columns: table => new
                {
                    primarykey = table.Column<long>(name: "primary-key", type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    surname = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    shortcut = table.Column<string>(type: "varchar(8)", maxLength: 8, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    applogin = table.Column<string>(name: "app-login", type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    apppassword = table.Column<string>(name: "app-password", type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    nationality = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    city = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RoleForeignKey = table.Column<int>(type: "int", nullable: false),
                    roleId = table.Column<long>(type: "bigint", nullable: true),
                    createddate = table.Column<DateTime>(name: "created-date", type: "datetime(6)", nullable: false),
                    updateddate = table.Column<DateTime>(name: "updated-date", type: "datetime(6)", nullable: false),
                    dictionaryhash = table.Column<string>(name: "dictionary-hash", type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_person-table", x => x.primarykey);
                    table.ForeignKey(
                        name: "FK_person-table_role_roleId",
                        column: x => x.roleId,
                        principalTable: "role",
                        principalColumn: "primary-key",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_person-table_roleId",
                table: "person-table",
                column: "roleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "person-table");

            migrationBuilder.DropTable(
                name: "role");
        }
    }
}
