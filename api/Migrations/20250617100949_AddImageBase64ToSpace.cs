using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class AddImageBase64ToSpace : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SpaceRule",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Rule = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpaceRule", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResidentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApartmentInformation = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Space",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SpaceName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    Availability = table.Column<bool>(type: "bit", nullable: false),
                    SpaceRuleId = table.Column<int>(type: "int", nullable: true),
                    ImageBase64 = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Space", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Space_SpaceRule_SpaceRuleId",
                        column: x => x.SpaceRuleId,
                        principalTable: "SpaceRule",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Reservation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResidentId = table.Column<int>(type: "int", nullable: false),
                    SpaceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reservation_Space_SpaceId",
                        column: x => x.SpaceId,
                        principalTable: "Space",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reservation_User_ResidentId",
                        column: x => x.ResidentId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Review",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResidentId = table.Column<int>(type: "int", nullable: false),
                    SpaceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Review", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Review_Space_SpaceId",
                        column: x => x.SpaceId,
                        principalTable: "Space",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Review_User_ResidentId",
                        column: x => x.ResidentId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reservation_ResidentId",
                table: "Reservation",
                column: "ResidentId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservation_SpaceId",
                table: "Reservation",
                column: "SpaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Review_ResidentId",
                table: "Review",
                column: "ResidentId");

            migrationBuilder.CreateIndex(
                name: "IX_Review_SpaceId",
                table: "Review",
                column: "SpaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Space_SpaceRuleId",
                table: "Space",
                column: "SpaceRuleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reservation");

            migrationBuilder.DropTable(
                name: "Review");

            migrationBuilder.DropTable(
                name: "Space");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "SpaceRule");
        }
    }
}
