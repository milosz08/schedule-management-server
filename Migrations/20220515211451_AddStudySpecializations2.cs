using Microsoft.EntityFrameworkCore.Migrations;

namespace asp_net_po_schedule_management_server.Migrations
{
    public partial class AddStudySpecializations2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "study-spec-type",
                table: "study-specializations");

            migrationBuilder.AddColumn<long>(
                name: "study-type-key",
                table: "study-specializations",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_study-specializations_study-type-key",
                table: "study-specializations",
                column: "study-type-key");

            migrationBuilder.AddForeignKey(
                name: "FK_study-specializations_study-types_study-type-key",
                table: "study-specializations",
                column: "study-type-key",
                principalTable: "study-types",
                principalColumn: "primary-key",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_study-specializations_study-types_study-type-key",
                table: "study-specializations");

            migrationBuilder.DropIndex(
                name: "IX_study-specializations_study-type-key",
                table: "study-specializations");

            migrationBuilder.DropColumn(
                name: "study-type-key",
                table: "study-specializations");

            migrationBuilder.AddColumn<string>(
                name: "study-spec-type",
                table: "study-specializations",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
