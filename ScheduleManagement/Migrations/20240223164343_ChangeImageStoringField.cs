using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScheduleManagement.Migrations
{
    /// <inheritdoc />
    public partial class ChangeImageStoringField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudySpecializations_study-degrees_StudyDegreeId",
                table: "StudySpecializations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_study-degrees",
                table: "study-degrees");

            migrationBuilder.DropColumn(
                name: "HasPicture",
                table: "Persons");

            migrationBuilder.RenameTable(
                name: "study-degrees",
                newName: "StudyDegrees");

            migrationBuilder.AddColumn<string>(
                name: "ProfileImageUuid",
                table: "Persons",
                type: "varchar(36)",
                maxLength: 36,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudyDegrees",
                table: "StudyDegrees",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudySpecializations_StudyDegrees_StudyDegreeId",
                table: "StudySpecializations",
                column: "StudyDegreeId",
                principalTable: "StudyDegrees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudySpecializations_StudyDegrees_StudyDegreeId",
                table: "StudySpecializations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudyDegrees",
                table: "StudyDegrees");

            migrationBuilder.DropColumn(
                name: "ProfileImageUuid",
                table: "Persons");

            migrationBuilder.RenameTable(
                name: "StudyDegrees",
                newName: "study-degrees");

            migrationBuilder.AddColumn<bool>(
                name: "HasPicture",
                table: "Persons",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_study-degrees",
                table: "study-degrees",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudySpecializations_study-degrees_StudyDegreeId",
                table: "StudySpecializations",
                column: "StudyDegreeId",
                principalTable: "study-degrees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
