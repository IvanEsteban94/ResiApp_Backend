using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservation_Space_SpaceId",
                table: "Reservation");

            migrationBuilder.DropIndex(
                name: "IX_Reservation_SpaceId",
                table: "Reservation");

            migrationBuilder.AddColumn<int>(
                name: "SpaceId",
                table: "Review",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SpaceId",
                table: "Review");

            migrationBuilder.CreateIndex(
                name: "IX_Reservation_SpaceId",
                table: "Reservation",
                column: "SpaceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservation_Space_SpaceId",
                table: "Reservation",
                column: "SpaceId",
                principalTable: "Space",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
