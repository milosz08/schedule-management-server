using Microsoft.EntityFrameworkCore.Migrations;

namespace asp_net_po_schedule_management_server.Migrations
{
    public partial class ChangeOneToManyRelation4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_week-schedule-occur_schedule-subjects_ScheduleSubject",
                table: "week-schedule-occur");

            migrationBuilder.DropIndex(
                name: "IX_week-schedule-occur_ScheduleSubject",
                table: "week-schedule-occur");

            migrationBuilder.DropColumn(
                name: "ScheduleSubject",
                table: "week-schedule-occur");

            migrationBuilder.RenameColumn(
                name: "subject-schedule-key",
                table: "week-schedule-occur",
                newName: "schedule-subject-key");

            migrationBuilder.CreateIndex(
                name: "IX_week-schedule-occur_schedule-subject-key",
                table: "week-schedule-occur",
                column: "schedule-subject-key");

            migrationBuilder.AddForeignKey(
                name: "FK_week-schedule-occur_schedule-subjects_schedule-subject-key",
                table: "week-schedule-occur",
                column: "schedule-subject-key",
                principalTable: "schedule-subjects",
                principalColumn: "primary-key",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_week-schedule-occur_schedule-subjects_schedule-subject-key",
                table: "week-schedule-occur");

            migrationBuilder.DropIndex(
                name: "IX_week-schedule-occur_schedule-subject-key",
                table: "week-schedule-occur");

            migrationBuilder.RenameColumn(
                name: "schedule-subject-key",
                table: "week-schedule-occur",
                newName: "subject-schedule-key");

            migrationBuilder.AddColumn<long>(
                name: "ScheduleSubject",
                table: "week-schedule-occur",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_week-schedule-occur_ScheduleSubject",
                table: "week-schedule-occur",
                column: "ScheduleSubject");

            migrationBuilder.AddForeignKey(
                name: "FK_week-schedule-occur_schedule-subjects_ScheduleSubject",
                table: "week-schedule-occur",
                column: "ScheduleSubject",
                principalTable: "schedule-subjects",
                principalColumn: "primary-key",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
