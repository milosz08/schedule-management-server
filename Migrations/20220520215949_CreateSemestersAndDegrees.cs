using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace asp_net_po_schedule_management_server.Migrations
{
    public partial class CreateSemestersAndDegrees : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "degree-key",
                table: "study-specializations",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "semesters",
                columns: table => new
                {
                    primarykey = table.Column<long>(name: "primary-key", type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    semname = table.Column<string>(name: "sem-name", type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    semalias = table.Column<string>(name: "sem-alias", type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createddate = table.Column<DateTime>(name: "created-date", type: "datetime(6)", nullable: false),
                    updateddate = table.Column<DateTime>(name: "updated-date", type: "datetime(6)", nullable: false),
                    dictionaryhash = table.Column<string>(name: "dictionary-hash", type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_semesters", x => x.primarykey);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "study-degrees",
                columns: table => new
                {
                    primarykey = table.Column<long>(name: "primary-key", type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    degreename = table.Column<string>(name: "degree-name", type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    degreealias = table.Column<string>(name: "degree-alias", type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    degreelevel = table.Column<int>(name: "degree-level", type: "int", nullable: false),
                    createddate = table.Column<DateTime>(name: "created-date", type: "datetime(6)", nullable: false),
                    updateddate = table.Column<DateTime>(name: "updated-date", type: "datetime(6)", nullable: false),
                    dictionaryhash = table.Column<string>(name: "dictionary-hash", type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_study-degrees", x => x.primarykey);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "study-groups",
                columns: table => new
                {
                    primarykey = table.Column<long>(name: "primary-key", type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    groupname = table.Column<string>(name: "group-name", type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deptkey = table.Column<long>(name: "dept-key", type: "bigint", nullable: false),
                    studyspeckey = table.Column<long>(name: "study-spec-key", type: "bigint", nullable: false),
                    semkey = table.Column<long>(name: "sem-key", type: "bigint", nullable: false),
                    createddate = table.Column<DateTime>(name: "created-date", type: "datetime(6)", nullable: false),
                    updateddate = table.Column<DateTime>(name: "updated-date", type: "datetime(6)", nullable: false),
                    dictionaryhash = table.Column<string>(name: "dictionary-hash", type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_study-groups", x => x.primarykey);
                    table.ForeignKey(
                        name: "FK_study-groups_departments_dept-key",
                        column: x => x.deptkey,
                        principalTable: "departments",
                        principalColumn: "primary-key",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_study-groups_semesters_sem-key",
                        column: x => x.semkey,
                        principalTable: "semesters",
                        principalColumn: "primary-key",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_study-groups_study-specializations_study-spec-key",
                        column: x => x.studyspeckey,
                        principalTable: "study-specializations",
                        principalColumn: "primary-key",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_study-specializations_degree-key",
                table: "study-specializations",
                column: "degree-key");

            migrationBuilder.CreateIndex(
                name: "IX_study-groups_dept-key",
                table: "study-groups",
                column: "dept-key");

            migrationBuilder.CreateIndex(
                name: "IX_study-groups_sem-key",
                table: "study-groups",
                column: "sem-key");

            migrationBuilder.CreateIndex(
                name: "IX_study-groups_study-spec-key",
                table: "study-groups",
                column: "study-spec-key");

            migrationBuilder.AddForeignKey(
                name: "FK_study-specializations_study-degrees_degree-key",
                table: "study-specializations",
                column: "degree-key",
                principalTable: "study-degrees",
                principalColumn: "primary-key",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_study-specializations_study-degrees_degree-key",
                table: "study-specializations");

            migrationBuilder.DropTable(
                name: "study-degrees");

            migrationBuilder.DropTable(
                name: "study-groups");

            migrationBuilder.DropTable(
                name: "semesters");

            migrationBuilder.DropIndex(
                name: "IX_study-specializations_degree-key",
                table: "study-specializations");

            migrationBuilder.DropColumn(
                name: "degree-key",
                table: "study-specializations");
        }
    }
}
