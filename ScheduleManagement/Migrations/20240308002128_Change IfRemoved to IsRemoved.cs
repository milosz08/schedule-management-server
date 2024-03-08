using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScheduleManagement.Migrations
{
    /// <inheritdoc />
    public partial class ChangeIfRemovedtoIsRemoved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IfRemovable",
                table: "Persons",
                newName: "IsRemovable");

            migrationBuilder.RenameColumn(
                name: "IfRemovable",
                table: "Cathedrals",
                newName: "IsRemovable");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsRemovable",
                table: "Persons",
                newName: "IfRemovable");

            migrationBuilder.RenameColumn(
                name: "IsRemovable",
                table: "Cathedrals",
                newName: "IfRemovable");
        }
    }
}
