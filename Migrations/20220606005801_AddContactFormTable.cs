using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace asp_net_po_schedule_management_server.Migrations
{
    public partial class AddContactFormTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "contact-form-issue-types",
                columns: table => new
                {
                    primarykey = table.Column<long>(name: "primary-key", type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    cathname = table.Column<string>(name: "cath-name", type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createddate = table.Column<DateTime>(name: "created-date", type: "datetime(6)", nullable: false),
                    updateddate = table.Column<DateTime>(name: "updated-date", type: "datetime(6)", nullable: false),
                    dictionaryhash = table.Column<string>(name: "dictionary-hash", type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contact-form-issue-types", x => x.primarykey);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "contact-messages",
                columns: table => new
                {
                    primarykey = table.Column<long>(name: "primary-key", type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    anonymousname = table.Column<string>(name: "anonymous-name", type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    anonymoussurname = table.Column<string>(name: "anonymous-surname", type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    anonymousemail = table.Column<string>(name: "anonymous-email", type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ifanonymous = table.Column<bool>(name: "if-anonymous", type: "tinyint(1)", nullable: false),
                    deptkey = table.Column<long>(name: "dept-key", type: "bigint", nullable: false),
                    userkey = table.Column<long>(name: "user-key", type: "bigint", nullable: false),
                    issuetypekey = table.Column<long>(name: "issue-type-key", type: "bigint", nullable: false),
                    createddate = table.Column<DateTime>(name: "created-date", type: "datetime(6)", nullable: false),
                    updateddate = table.Column<DateTime>(name: "updated-date", type: "datetime(6)", nullable: false),
                    dictionaryhash = table.Column<string>(name: "dictionary-hash", type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contact-messages", x => x.primarykey);
                    table.ForeignKey(
                        name: "FK_contact-messages_contact-form-issue-types_issue-type-key",
                        column: x => x.issuetypekey,
                        principalTable: "contact-form-issue-types",
                        principalColumn: "primary-key",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_contact-messages_departments_dept-key",
                        column: x => x.deptkey,
                        principalTable: "departments",
                        principalColumn: "primary-key",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_contact-messages_person_user-key",
                        column: x => x.userkey,
                        principalTable: "person",
                        principalColumn: "primary-key",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "contact-messages-groups-binding",
                columns: table => new
                {
                    contactmessagekey = table.Column<long>(name: "contact-message-key", type: "bigint", nullable: false),
                    groupkey = table.Column<long>(name: "group-key", type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contact-messages-groups-binding", x => new { x.contactmessagekey, x.groupkey });
                    table.ForeignKey(
                        name: "FK_contact-messages-groups-binding_contact-messages_contact-mes~",
                        column: x => x.contactmessagekey,
                        principalTable: "contact-messages",
                        principalColumn: "primary-key",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_contact-messages-groups-binding_study-groups_group-key",
                        column: x => x.groupkey,
                        principalTable: "study-groups",
                        principalColumn: "primary-key",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_contact-messages_dept-key",
                table: "contact-messages",
                column: "dept-key");

            migrationBuilder.CreateIndex(
                name: "IX_contact-messages_issue-type-key",
                table: "contact-messages",
                column: "issue-type-key");

            migrationBuilder.CreateIndex(
                name: "IX_contact-messages_user-key",
                table: "contact-messages",
                column: "user-key");

            migrationBuilder.CreateIndex(
                name: "IX_contact-messages-groups-binding_group-key",
                table: "contact-messages-groups-binding",
                column: "group-key");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "contact-messages-groups-binding");

            migrationBuilder.DropTable(
                name: "contact-messages");

            migrationBuilder.DropTable(
                name: "contact-form-issue-types");
        }
    }
}
