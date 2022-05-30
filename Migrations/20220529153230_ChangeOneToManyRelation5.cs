using Microsoft.EntityFrameworkCore.Migrations;

namespace asp_net_po_schedule_management_server.Migrations
{
    public partial class ChangeOneToManyRelation5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_schedule-subjects_study-groups_group-key",
                table: "schedule-subjects");

            migrationBuilder.DropIndex(
                name: "IX_schedule-subjects_group-key",
                table: "schedule-subjects");

            migrationBuilder.DropColumn(
                name: "group-key",
                table: "schedule-subjects");

            migrationBuilder.CreateTable(
                name: "schedule-groups-binding",
                columns: table => new
                {
                    groupkey = table.Column<long>(name: "group-key", type: "bigint", nullable: false),
                    schedulesubjectkey = table.Column<long>(name: "schedule-subject-key", type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_schedule-groups-binding", x => new { x.groupkey, x.schedulesubjectkey });
                    table.ForeignKey(
                        name: "FK_schedule-groups-binding_schedule-subjects_schedule-subject-k~",
                        column: x => x.schedulesubjectkey,
                        principalTable: "schedule-subjects",
                        principalColumn: "primary-key",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_schedule-groups-binding_study-groups_group-key",
                        column: x => x.groupkey,
                        principalTable: "study-groups",
                        principalColumn: "primary-key",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_schedule-groups-binding_schedule-subject-key",
                table: "schedule-groups-binding",
                column: "schedule-subject-key");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "schedule-groups-binding");

            migrationBuilder.AddColumn<long>(
                name: "group-key",
                table: "schedule-subjects",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_schedule-subjects_group-key",
                table: "schedule-subjects",
                column: "group-key");

            migrationBuilder.AddForeignKey(
                name: "FK_schedule-subjects_study-groups_group-key",
                table: "schedule-subjects",
                column: "group-key",
                principalTable: "study-groups",
                principalColumn: "primary-key",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
