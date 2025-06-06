using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class CheckForSpaceId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Space_SpaceRule_SpaceRuleId",
                table: "Space");

            migrationBuilder.DropForeignKey(
                name: "FK_SpaceRule_Space_SpaceId",
                table: "SpaceRule");

            migrationBuilder.DropIndex(
                name: "IX_SpaceRule_SpaceId",
                table: "SpaceRule");

            migrationBuilder.DropColumn(
                name: "SpaceId",
                table: "SpaceRule");

            migrationBuilder.AddForeignKey(
                name: "FK_Space_SpaceRule_SpaceRuleId",
                table: "Space",
                column: "SpaceRuleId",
                principalTable: "SpaceRule",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Space_SpaceRule_SpaceRuleId",
                table: "Space");

            migrationBuilder.AddColumn<int>(
                name: "SpaceId",
                table: "SpaceRule",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SpaceRule_SpaceId",
                table: "SpaceRule",
                column: "SpaceId");

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
    }
}
