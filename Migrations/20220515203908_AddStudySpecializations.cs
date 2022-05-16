using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace asp_net_po_schedule_management_server.Migrations
{
    public partial class AddStudySpecializations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "study-specializations",
                columns: table => new
                {
                    primarykey = table.Column<long>(name: "primary-key", type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    studyspecname = table.Column<string>(name: "study-spec-name", type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    studyspecalias = table.Column<string>(name: "study-spec-alias", type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    studyspectype = table.Column<string>(name: "study-spec-type", type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deptkey = table.Column<long>(name: "dept-key", type: "bigint", nullable: false),
                    createddate = table.Column<DateTime>(name: "created-date", type: "datetime(6)", nullable: false),
                    updateddate = table.Column<DateTime>(name: "updated-date", type: "datetime(6)", nullable: false),
                    dictionaryhash = table.Column<string>(name: "dictionary-hash", type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_study-specializations", x => x.primarykey);
                    table.ForeignKey(
                        name: "FK_study-specializations_departments_dept-key",
                        column: x => x.deptkey,
                        principalTable: "departments",
                        principalColumn: "primary-key",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_study-specializations_dept-key",
                table: "study-specializations",
                column: "dept-key");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "study-specializations");
        }
    }
}
