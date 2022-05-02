using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace asp_net_po_schedule_management_server.Migrations
{
    public partial class AddOtpTokenMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "reset-password-otp",
                columns: table => new
                {
                    primarykey = table.Column<long>(name: "primary-key", type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    useremail = table.Column<string>(name: "user-email", type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    userotp = table.Column<string>(name: "user-otp", type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    otpexpired = table.Column<DateTime>(name: "otp-expired", type: "datetime(6)", nullable: false),
                    personkey = table.Column<long>(name: "person-key", type: "bigint", nullable: false),
                    createddate = table.Column<DateTime>(name: "created-date", type: "datetime(6)", nullable: false),
                    updateddate = table.Column<DateTime>(name: "updated-date", type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reset-password-otp", x => x.primarykey);
                    table.ForeignKey(
                        name: "FK_reset-password-otp_person_person-key",
                        column: x => x.personkey,
                        principalTable: "person",
                        principalColumn: "primary-key",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_reset-password-otp_person-key",
                table: "reset-password-otp",
                column: "person-key");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "reset-password-otp");
        }
    }
}
