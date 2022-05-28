using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace asp_net_po_schedule_management_server.Migrations
{
    public partial class ScheduleSubjectTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "subject-type-key",
                table: "schedule-subjects",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "schedule-types",
                columns: table => new
                {
                    primarykey = table.Column<long>(name: "primary-key", type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    scheduletypename = table.Column<string>(name: "schedule-type-name", type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    scheduletypealias = table.Column<string>(name: "schedule-type-alias", type: "varchar(5)", maxLength: 5, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    scheduletypecolor = table.Column<string>(name: "schedule-type-color", type: "varchar(7)", maxLength: 7, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createddate = table.Column<DateTime>(name: "created-date", type: "datetime(6)", nullable: false),
                    updateddate = table.Column<DateTime>(name: "updated-date", type: "datetime(6)", nullable: false),
                    dictionaryhash = table.Column<string>(name: "dictionary-hash", type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_schedule-types", x => x.primarykey);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_schedule-subjects_subject-type-key",
                table: "schedule-subjects",
                column: "subject-type-key");

            migrationBuilder.AddForeignKey(
                name: "FK_schedule-subjects_schedule-types_subject-type-key",
                table: "schedule-subjects",
                column: "subject-type-key",
                principalTable: "schedule-types",
                principalColumn: "primary-key",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_schedule-subjects_schedule-types_subject-type-key",
                table: "schedule-subjects");

            migrationBuilder.DropTable(
                name: "schedule-types");

            migrationBuilder.DropIndex(
                name: "IX_schedule-subjects_subject-type-key",
                table: "schedule-subjects");

            migrationBuilder.DropColumn(
                name: "subject-type-key",
                table: "schedule-subjects");
        }
    }
}
