using Microsoft.EntityFrameworkCore.Migrations;

namespace asp_net_po_schedule_management_server.Migrations
{
    public partial class ScheduleSubjectTypesChangename : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_schedule-subjects_schedule-types_subject-type-key",
                table: "schedule-subjects");

            migrationBuilder.DropPrimaryKey(
                name: "PK_schedule-types",
                table: "schedule-types");

            migrationBuilder.RenameTable(
                name: "schedule-types",
                newName: "schedule-subject-types");

            migrationBuilder.AddPrimaryKey(
                name: "PK_schedule-subject-types",
                table: "schedule-subject-types",
                column: "primary-key");

            migrationBuilder.AddForeignKey(
                name: "FK_schedule-subjects_schedule-subject-types_subject-type-key",
                table: "schedule-subjects",
                column: "subject-type-key",
                principalTable: "schedule-subject-types",
                principalColumn: "primary-key",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_schedule-subjects_schedule-subject-types_subject-type-key",
                table: "schedule-subjects");

            migrationBuilder.DropPrimaryKey(
                name: "PK_schedule-subject-types",
                table: "schedule-subject-types");

            migrationBuilder.RenameTable(
                name: "schedule-subject-types",
                newName: "schedule-types");

            migrationBuilder.AddPrimaryKey(
                name: "PK_schedule-types",
                table: "schedule-types",
                column: "primary-key");

            migrationBuilder.AddForeignKey(
                name: "FK_schedule-subjects_schedule-types_subject-type-key",
                table: "schedule-subjects",
                column: "subject-type-key",
                principalTable: "schedule-types",
                principalColumn: "primary-key",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
