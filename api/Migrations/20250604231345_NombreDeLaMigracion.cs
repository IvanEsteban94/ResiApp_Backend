using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class NombreDeLaMigracion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SpaceRule_Space_SpaceId",
                table: "SpaceRule");

            migrationBuilder.AlterColumn<int>(
                name: "SpaceId",
                table: "SpaceRule",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "SpaceRuleId",
                table: "Space",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Space_SpaceRuleId",
                table: "Space",
                column: "SpaceRuleId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Space_SpaceRule_SpaceRuleId",
                table: "Space",
                column: "SpaceRuleId",
                principalTable: "SpaceRule",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SpaceRule_Space_SpaceId",
                table: "SpaceRule",
                column: "SpaceId",
                principalTable: "Space",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservation_Space_SpaceId",
                table: "Reservation");

            migrationBuilder.DropForeignKey(
                name: "FK_Space_SpaceRule_SpaceRuleId",
                table: "Space");

            migrationBuilder.DropForeignKey(
                name: "FK_SpaceRule_Space_SpaceId",
                table: "SpaceRule");

            migrationBuilder.DropIndex(
                name: "IX_Space_SpaceRuleId",
                table: "Space");

            migrationBuilder.DropIndex(
                name: "IX_Reservation_SpaceId",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "SpaceRuleId",
                table: "Space");

            migrationBuilder.AlterColumn<int>(
                name: "SpaceId",
                table: "SpaceRule",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SpaceRule_Space_SpaceId",
                table: "SpaceRule",
                column: "SpaceId",
                principalTable: "Space",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
