using Microsoft.EntityFrameworkCore.Migrations;

namespace asp_net_po_schedule_management_server.Migrations
{
    public partial class ChangeNullableContactMessageEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_contact-messages_departments_dept-key",
                table: "contact-messages");

            migrationBuilder.DropForeignKey(
                name: "FK_contact-messages_person_user-key",
                table: "contact-messages");

            migrationBuilder.AlterColumn<long>(
                name: "user-key",
                table: "contact-messages",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "dept-key",
                table: "contact-messages",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_contact-messages_departments_dept-key",
                table: "contact-messages",
                column: "dept-key",
                principalTable: "departments",
                principalColumn: "primary-key",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_contact-messages_person_user-key",
                table: "contact-messages",
                column: "user-key",
                principalTable: "person",
                principalColumn: "primary-key",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_contact-messages_departments_dept-key",
                table: "contact-messages");

            migrationBuilder.DropForeignKey(
                name: "FK_contact-messages_person_user-key",
                table: "contact-messages");

            migrationBuilder.AlterColumn<long>(
                name: "user-key",
                table: "contact-messages",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "dept-key",
                table: "contact-messages",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_contact-messages_departments_dept-key",
                table: "contact-messages",
                column: "dept-key",
                principalTable: "departments",
                principalColumn: "primary-key",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_contact-messages_person_user-key",
                table: "contact-messages",
                column: "user-key",
                principalTable: "person",
                principalColumn: "primary-key",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
