using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace asp_net_po_schedule_management_server.Migrations
{
    public partial class AddRoomsAndRoomTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "room-types",
                columns: table => new
                {
                    primarykey = table.Column<long>(name: "primary-key", type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    roomtypename = table.Column<string>(name: "room-type-name", type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    roomtypealias = table.Column<string>(name: "room-type-alias", type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    roomtypecolor = table.Column<string>(name: "room-type-color", type: "varchar(6)", maxLength: 6, nullable: true)
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
                name: "study-rooms",
                columns: table => new
                {
                    primarykey = table.Column<long>(name: "primary-key", type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    studyroomname = table.Column<string>(name: "study-room-name", type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    studyroomdesc = table.Column<string>(name: "study-room-desc", type: "varchar(150)", maxLength: 150, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
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
                        name: "FK_study-rooms_room-types_room-type-key",
                        column: x => x.roomtypekey,
                        principalTable: "room-types",
                        principalColumn: "primary-key",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_study-rooms_cathedral-key",
                table: "study-rooms",
                column: "cathedral-key");

            migrationBuilder.CreateIndex(
                name: "IX_study-rooms_room-type-key",
                table: "study-rooms",
                column: "room-type-key");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "study-rooms");

            migrationBuilder.DropTable(
                name: "room-types");
        }
    }
}
