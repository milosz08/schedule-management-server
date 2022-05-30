using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace asp_net_po_schedule_management_server.Migrations
{
    public partial class ChangeOneToManyRelation3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_week-schedule-occur_schedule-subjects_subject-schedule-key",
                table: "week-schedule-occur");

            migrationBuilder.DropIndex(
                name: "IX_week-schedule-occur_subject-schedule-key",
                table: "week-schedule-occur");

            migrationBuilder.AddColumn<long>(
                name: "ScheduleSubject",
                table: "week-schedule-occur",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "week-occur",
                table: "week-schedule-occur",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "week-occur",
                table: "week-schedule-occur");

            migrationBuilder.CreateIndex(
                name: "IX_week-schedule-occur_subject-schedule-key",
                table: "week-schedule-occur",
                column: "subject-schedule-key");

            migrationBuilder.AddForeignKey(
                name: "FK_week-schedule-occur_schedule-subjects_subject-schedule-key",
                table: "week-schedule-occur",
                column: "subject-schedule-key",
                principalTable: "schedule-subjects",
                principalColumn: "primary-key",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
