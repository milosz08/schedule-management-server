using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace asp_net_po_schedule_management_server.Migrations
{
    public partial class DatabaseRefactoring : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "departments",
                columns: table => new
                {
                    primarykey = table.Column<long>(name: "primary-key", type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    deptname = table.Column<string>(name: "dept-name", type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deptalias = table.Column<string>(name: "dept-alias", type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ifremovable = table.Column<bool>(name: "if-removable", type: "tinyint(1)", nullable: false),
                    createddate = table.Column<DateTime>(name: "created-date", type: "datetime(6)", nullable: false),
                    updateddate = table.Column<DateTime>(name: "updated-date", type: "datetime(6)", nullable: false),
                    dictionaryhash = table.Column<string>(name: "dictionary-hash", type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_departments", x => x.primarykey);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "role",
                columns: table => new
                {
                    primarykey = table.Column<long>(name: "primary-key", type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createddate = table.Column<DateTime>(name: "created-date", type: "datetime(6)", nullable: false),
                    updateddate = table.Column<DateTime>(name: "updated-date", type: "datetime(6)", nullable: false),
                    dictionaryhash = table.Column<string>(name: "dictionary-hash", type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role", x => x.primarykey);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "room-types",
                columns: table => new
                {
                    primarykey = table.Column<long>(name: "primary-key", type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    roomtypename = table.Column<string>(name: "room-type-name", type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    roomtypealias = table.Column<string>(name: "room-type-alias", type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    roomtypecolor = table.Column<string>(name: "room-type-color", type: "varchar(6)", maxLength: 6, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createddate = table.Column<DateTime>(name: "created-date", type: "datetime(6)", nullable: false),
                    updateddate = table.Column<DateTime>(name: "updated-date", type: "datetime(6)", nullable: false),
                    dictionaryhash = table.Column<string>(name: "dictionary-hash", type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_room-types", x => x.primarykey);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "study-types",
                columns: table => new
                {
                    primarykey = table.Column<long>(name: "primary-key", type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    studytypename = table.Column<string>(name: "study-type-name", type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    studytypealias = table.Column<string>(name: "study-type-alias", type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createddate = table.Column<DateTime>(name: "created-date", type: "datetime(6)", nullable: false),
                    updateddate = table.Column<DateTime>(name: "updated-date", type: "datetime(6)", nullable: false),
                    dictionaryhash = table.Column<string>(name: "dictionary-hash", type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_study-types", x => x.primarykey);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "cathedrals",
                columns: table => new
                {
                    primarykey = table.Column<long>(name: "primary-key", type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    cathname = table.Column<string>(name: "cath-name", type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cathalias = table.Column<string>(name: "cath-alias", type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ifremovable = table.Column<bool>(name: "if-removable", type: "tinyint(1)", nullable: false),
                    deptkey = table.Column<long>(name: "dept-key", type: "bigint", nullable: false),
                    createddate = table.Column<DateTime>(name: "created-date", type: "datetime(6)", nullable: false),
                    updateddate = table.Column<DateTime>(name: "updated-date", type: "datetime(6)", nullable: false),
                    dictionaryhash = table.Column<string>(name: "dictionary-hash", type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cathedrals", x => x.primarykey);
                    table.ForeignKey(
                        name: "FK_cathedrals_departments_dept-key",
                        column: x => x.deptkey,
                        principalTable: "departments",
                        principalColumn: "primary-key",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "study-specializations",
                columns: table => new
                {
                    primarykey = table.Column<long>(name: "primary-key", type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    studyspecname = table.Column<string>(name: "study-spec-name", type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    studyspecalias = table.Column<string>(name: "study-spec-alias", type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    studytypekey = table.Column<long>(name: "study-type-key", type: "bigint", nullable: false),
                    deptkey = table.Column<long>(name: "dept-key", type: "bigint", nullable: false),
                    createddate = table.Column<DateTime>(name: "created-date", type: "datetime(6)", nullable: false),
                    updateddate = table.Column<DateTime>(name: "updated-date", type: "datetime(6)", nullable: false),
                    dictionaryhash = table.Column<string>(name: "dictionary-hash", type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_study-specializations", x => x.primarykey);
                    table.ForeignKey(
                        name: "FK_study-specializations_departments_dept-key",
                        column: x => x.deptkey,
                        principalTable: "departments",
                        principalColumn: "primary-key",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_study-specializations_study-types_study-type-key",
                        column: x => x.studytypekey,
                        principalTable: "study-types",
                        principalColumn: "primary-key",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "person",
                columns: table => new
                {
                    primarykey = table.Column<long>(name: "primary-key", type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    surname = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    shortcut = table.Column<string>(type: "varchar(8)", maxLength: 8, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    login = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    firstaccess = table.Column<bool>(name: "first-access", type: "tinyint(1)", nullable: false),
                    password = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    nationality = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    city = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    emailpassword = table.Column<string>(name: "email-password", type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    haspicture = table.Column<bool>(name: "has-picture", type: "tinyint(1)", nullable: false),
                    ifremovable = table.Column<bool>(name: "if-removable", type: "tinyint(1)", nullable: false),
                    rolekey = table.Column<long>(name: "role-key", type: "bigint", nullable: false),
                    deptkey = table.Column<long>(name: "dept-key", type: "bigint", nullable: true),
                    cathkey = table.Column<long>(name: "cath-key", type: "bigint", nullable: true),
                    createddate = table.Column<DateTime>(name: "created-date", type: "datetime(6)", nullable: false),
                    updateddate = table.Column<DateTime>(name: "updated-date", type: "datetime(6)", nullable: false),
                    dictionaryhash = table.Column<string>(name: "dictionary-hash", type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_person", x => x.primarykey);
                    table.ForeignKey(
                        name: "FK_person_cathedrals_cath-key",
                        column: x => x.cathkey,
                        principalTable: "cathedrals",
                        principalColumn: "primary-key",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_person_departments_dept-key",
                        column: x => x.deptkey,
                        principalTable: "departments",
                        principalColumn: "primary-key",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_person_role_role-key",
                        column: x => x.rolekey,
                        principalTable: "role",
                        principalColumn: "primary-key",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "study-rooms",
                columns: table => new
                {
                    primarykey = table.Column<long>(name: "primary-key", type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    studyroomname = table.Column<string>(name: "study-room-name", type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    studyroomdesc = table.Column<string>(name: "study-room-desc", type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    studyroomcapacity = table.Column<int>(name: "study-room-capacity", type: "int", nullable: false),
                    departmentkey = table.Column<long>(name: "department-key", type: "bigint", nullable: false),
                    cathedralkey = table.Column<long>(name: "cathedral-key", type: "bigint", nullable: false),
                    roomtypekey = table.Column<long>(name: "room-type-key", type: "bigint", nullable: false),
                    createddate = table.Column<DateTime>(name: "created-date", type: "datetime(6)", nullable: false),
                    updateddate = table.Column<DateTime>(name: "updated-date", type: "datetime(6)", nullable: false),
                    dictionaryhash = table.Column<string>(name: "dictionary-hash", type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_study-rooms", x => x.primarykey);
                    table.ForeignKey(
                        name: "FK_study-rooms_cathedrals_cathedral-key",
                        column: x => x.cathedralkey,
                        principalTable: "cathedrals",
                        principalColumn: "primary-key",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_study-rooms_departments_department-key",
                        column: x => x.departmentkey,
                        principalTable: "departments",
                        principalColumn: "primary-key",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_study-rooms_room-types_room-type-key",
                        column: x => x.roomtypekey,
                        principalTable: "room-types",
                        principalColumn: "primary-key",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "jwt-tokens",
                columns: table => new
                {
                    primarykey = table.Column<long>(name: "primary-key", type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    tokenvalue = table.Column<string>(name: "token-value", type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    personkey = table.Column<long>(name: "person-key", type: "bigint", nullable: false),
                    createddate = table.Column<DateTime>(name: "created-date", type: "datetime(6)", nullable: false),
                    updateddate = table.Column<DateTime>(name: "updated-date", type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_jwt-tokens", x => x.primarykey);
                    table.ForeignKey(
                        name: "FK_jwt-tokens_person_person-key",
                        column: x => x.personkey,
                        principalTable: "person",
                        principalColumn: "primary-key",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

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
                    ifused = table.Column<bool>(name: "if-used", type: "tinyint(1)", nullable: false),
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

            migrationBuilder.CreateTable(
                name: "students-specs-binding",
                columns: table => new
                {
                    studentkey = table.Column<long>(name: "student-key", type: "bigint", nullable: false),
                    studyspeckey = table.Column<long>(name: "study-spec-key", type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_students-specs-binding", x => new { x.studentkey, x.studyspeckey });
                    table.ForeignKey(
                        name: "FK_students-specs-binding_person_student-key",
                        column: x => x.studentkey,
                        principalTable: "person",
                        principalColumn: "primary-key",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_students-specs-binding_study-specializations_study-spec-key",
                        column: x => x.studyspeckey,
                        principalTable: "study-specializations",
                        principalColumn: "primary-key",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_cathedrals_dept-key",
                table: "cathedrals",
                column: "dept-key");

            migrationBuilder.CreateIndex(
                name: "IX_jwt-tokens_person-key",
                table: "jwt-tokens",
                column: "person-key");

            migrationBuilder.CreateIndex(
                name: "IX_person_cath-key",
                table: "person",
                column: "cath-key");

            migrationBuilder.CreateIndex(
                name: "IX_person_dept-key",
                table: "person",
                column: "dept-key");

            migrationBuilder.CreateIndex(
                name: "IX_person_role-key",
                table: "person",
                column: "role-key");

            migrationBuilder.CreateIndex(
                name: "IX_reset-password-otp_person-key",
                table: "reset-password-otp",
                column: "person-key");

            migrationBuilder.CreateIndex(
                name: "IX_students-specs-binding_study-spec-key",
                table: "students-specs-binding",
                column: "study-spec-key");

            migrationBuilder.CreateIndex(
                name: "IX_study-rooms_cathedral-key",
                table: "study-rooms",
                column: "cathedral-key");

            migrationBuilder.CreateIndex(
                name: "IX_study-rooms_department-key",
                table: "study-rooms",
                column: "department-key");

            migrationBuilder.CreateIndex(
                name: "IX_study-rooms_room-type-key",
                table: "study-rooms",
                column: "room-type-key");

            migrationBuilder.CreateIndex(
                name: "IX_study-specializations_dept-key",
                table: "study-specializations",
                column: "dept-key");

            migrationBuilder.CreateIndex(
                name: "IX_study-specializations_study-type-key",
                table: "study-specializations",
                column: "study-type-key");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "jwt-tokens");

            migrationBuilder.DropTable(
                name: "reset-password-otp");

            migrationBuilder.DropTable(
                name: "students-specs-binding");

            migrationBuilder.DropTable(
                name: "study-rooms");

            migrationBuilder.DropTable(
                name: "person");

            migrationBuilder.DropTable(
                name: "study-specializations");

            migrationBuilder.DropTable(
                name: "room-types");

            migrationBuilder.DropTable(
                name: "cathedrals");

            migrationBuilder.DropTable(
                name: "role");

            migrationBuilder.DropTable(
                name: "study-types");

            migrationBuilder.DropTable(
                name: "departments");
        }
    }
}
