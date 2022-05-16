using Microsoft.EntityFrameworkCore.Migrations;

namespace asp_net_po_schedule_management_server.Migrations
{
    public partial class AddRoomsAndRoom : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "department-key",
                table: "study-rooms",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_study-rooms_department-key",
                table: "study-rooms",
                column: "department-key");

            migrationBuilder.AddForeignKey(
                name: "FK_study-rooms_departments_department-key",
                table: "study-rooms",
                column: "department-key",
                principalTable: "departments",
                principalColumn: "primary-key",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_study-rooms_departments_department-key",
                table: "study-rooms");

            migrationBuilder.DropIndex(
                name: "IX_study-rooms_department-key",
                table: "study-rooms");

            migrationBuilder.DropColumn(
                name: "department-key",
                table: "study-rooms");
        }
    }
}
