using Microsoft.EntityFrameworkCore.Migrations;

namespace asp_net_po_schedule_management_server.Migrations
{
    public partial class ChangeOneToManyRelation2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_week-schedule-occur_schedule-subjects_ScheduleSubjectId",
                table: "week-schedule-occur");

            migrationBuilder.RenameColumn(
                name: "ScheduleSubjectId",
                table: "week-schedule-occur",
                newName: "subject-schedule-key");

            migrationBuilder.RenameIndex(
                name: "IX_week-schedule-occur_ScheduleSubjectId",
                table: "week-schedule-occur",
                newName: "IX_week-schedule-occur_subject-schedule-key");

            migrationBuilder.AlterColumn<long>(
                name: "subject-schedule-key",
                table: "week-schedule-occur",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_week-schedule-occur_schedule-subjects_subject-schedule-key",
                table: "week-schedule-occur",
                column: "subject-schedule-key",
                principalTable: "schedule-subjects",
                principalColumn: "primary-key",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_week-schedule-occur_schedule-subjects_subject-schedule-key",
                table: "week-schedule-occur");

            migrationBuilder.RenameColumn(
                name: "subject-schedule-key",
                table: "week-schedule-occur",
                newName: "ScheduleSubjectId");

            migrationBuilder.RenameIndex(
                name: "IX_week-schedule-occur_subject-schedule-key",
                table: "week-schedule-occur",
                newName: "IX_week-schedule-occur_ScheduleSubjectId");

            migrationBuilder.AlterColumn<long>(
                name: "ScheduleSubjectId",
                table: "week-schedule-occur",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_week-schedule-occur_schedule-subjects_ScheduleSubjectId",
                table: "week-schedule-occur",
                column: "ScheduleSubjectId",
                principalTable: "schedule-subjects",
                principalColumn: "primary-key",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
