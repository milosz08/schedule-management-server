using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace asp_net_po_schedule_management_server.Migrations
{
    public partial class CreateStudySubjects : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "study-subjects",
                columns: table => new
                {
                    primarykey = table.Column<long>(name: "primary-key", type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    studyspecname = table.Column<string>(name: "study-spec-name", type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    studyspecalias = table.Column<string>(name: "study-spec-alias", type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deptkey = table.Column<long>(name: "dept-key", type: "bigint", nullable: false),
                    studyspeckey = table.Column<long>(name: "study-spec-key", type: "bigint", nullable: false),
                    createddate = table.Column<DateTime>(name: "created-date", type: "datetime(6)", nullable: false),
                    updateddate = table.Column<DateTime>(name: "updated-date", type: "datetime(6)", nullable: false),
                    dictionaryhash = table.Column<string>(name: "dictionary-hash", type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_study-subjects", x => x.primarykey);
                    table.ForeignKey(
                        name: "FK_study-subjects_departments_dept-key",
                        column: x => x.deptkey,
                        principalTable: "departments",
                        principalColumn: "primary-key",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_study-subjects_study-specializations_study-spec-key",
                        column: x => x.studyspeckey,
                        principalTable: "study-specializations",
                        principalColumn: "primary-key",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "users-subjects-binding",
                columns: table => new
                {
                    studysubjectkey = table.Column<long>(name: "study-subject-key", type: "bigint", nullable: false),
                    userkey = table.Column<long>(name: "user-key", type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users-subjects-binding", x => new { x.studysubjectkey, x.userkey });
                    table.ForeignKey(
                        name: "FK_users-subjects-binding_person_user-key",
                        column: x => x.userkey,
                        principalTable: "person",
                        principalColumn: "primary-key",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_users-subjects-binding_study-subjects_study-subject-key",
                        column: x => x.studysubjectkey,
                        principalTable: "study-subjects",
                        principalColumn: "primary-key",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_study-subjects_dept-key",
                table: "study-subjects",
                column: "dept-key");

            migrationBuilder.CreateIndex(
                name: "IX_study-subjects_study-spec-key",
                table: "study-subjects",
                column: "study-spec-key");

            migrationBuilder.CreateIndex(
                name: "IX_users-subjects-binding_user-key",
                table: "users-subjects-binding",
                column: "user-key");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "users-subjects-binding");

            migrationBuilder.DropTable(
                name: "study-subjects");
        }
    }
}
