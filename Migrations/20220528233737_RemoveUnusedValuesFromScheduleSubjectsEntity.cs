using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace asp_net_po_schedule_management_server.Migrations
{
    public partial class RemoveUnusedValuesFromScheduleSubjectsEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_schedule-subjects_study-types_study-type-key",
                table: "schedule-subjects");

            migrationBuilder.DropIndex(
                name: "IX_schedule-subjects_study-type-key",
                table: "schedule-subjects");

            migrationBuilder.DropColumn(
                name: "study-type-key",
                table: "schedule-subjects");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "start-time",
                table: "schedule-subjects",
                type: "time(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "end-time",
                table: "schedule-subjects",
                type: "time(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "start-time",
                table: "schedule-subjects",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "time(6)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "end-time",
                table: "schedule-subjects",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "time(6)");

            migrationBuilder.AddColumn<long>(
                name: "study-type-key",
                table: "schedule-subjects",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_schedule-subjects_study-type-key",
                table: "schedule-subjects",
                column: "study-type-key");

            migrationBuilder.AddForeignKey(
                name: "FK_schedule-subjects_study-types_study-type-key",
                table: "schedule-subjects",
                column: "study-type-key",
                principalTable: "study-types",
                principalColumn: "primary-key",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
