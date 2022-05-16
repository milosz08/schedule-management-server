using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace asp_net_po_schedule_management_server.Migrations
{
    public partial class AddCathedraAndDepartment2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "departments",
                columns: table => new
                {
                    primarykey = table.Column<long>(name: "primary-key", type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    deptname = table.Column<string>(name: "dept-name", type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deptalias = table.Column<string>(name: "dept-alias", type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createddate = table.Column<DateTime>(name: "created-date", type: "datetime(6)", nullable: false),
                    updateddate = table.Column<DateTime>(name: "updated-date", type: "datetime(6)", nullable: false),
                    dictionaryhash = table.Column<string>(name: "dictionary-hash", type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_departments", x => x.primarykey);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "cathedrals",
                columns: table => new
                {
                    primarykey = table.Column<long>(name: "primary-key", type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    cathname = table.Column<string>(name: "cath-name", type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cathalias = table.Column<string>(name: "cath-alias", type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deptkey = table.Column<long>(name: "dept-key", type: "bigint", nullable: false),
                    createddate = table.Column<DateTime>(name: "created-date", type: "datetime(6)", nullable: false),
                    updateddate = table.Column<DateTime>(name: "updated-date", type: "datetime(6)", nullable: false),
                    dictionaryhash = table.Column<string>(name: "dictionary-hash", type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cathedrals", x => x.primarykey);
                    table.ForeignKey(
                        name: "FK_cathedrals_departments_dept-key",
                        column: x => x.deptkey,
                        principalTable: "departments",
                        principalColumn: "primary-key",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_cathedrals_dept-key",
                table: "cathedrals",
                column: "dept-key");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cathedrals");

            migrationBuilder.DropTable(
                name: "departments");
        }
    }
}
