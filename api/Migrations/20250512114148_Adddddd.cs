using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class Adddddd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SpaceRuke_Space_SpaceId",
                table: "SpaceRuke");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SpaceRuke",
                table: "SpaceRuke");

            migrationBuilder.RenameTable(
                name: "SpaceRuke",
                newName: "SpaceRule");

            migrationBuilder.RenameIndex(
                name: "IX_SpaceRuke_SpaceId",
                table: "SpaceRule",
                newName: "IX_SpaceRule_SpaceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SpaceRule",
                table: "SpaceRule",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SpaceRule_Space_SpaceId",
                table: "SpaceRule",
                column: "SpaceId",
                principalTable: "Space",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SpaceRule_Space_SpaceId",
                table: "SpaceRule");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SpaceRule",
                table: "SpaceRule");

            migrationBuilder.RenameTable(
                name: "SpaceRule",
                newName: "SpaceRuke");

            migrationBuilder.RenameIndex(
                name: "IX_SpaceRule_SpaceId",
                table: "SpaceRuke",
                newName: "IX_SpaceRuke_SpaceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SpaceRuke",
                table: "SpaceRuke",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SpaceRuke_Space_SpaceId",
                table: "SpaceRuke",
                column: "SpaceId",
                principalTable: "Space",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
