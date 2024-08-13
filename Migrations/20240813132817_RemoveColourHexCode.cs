using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PirateConquest.Migrations
{
    /// <inheritdoc />
    public partial class RemoveColourHexCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_AdjacentSeasAsync_Seas_AdjacentToId",
            //    table: "AdjacentSeasAsync");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_AdjacentSeasAsync_Seas_SeaId",
            //    table: "AdjacentSeasAsync");

            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_AdjacentSeasAsync",
            //    table: "AdjacentSeasAsync");

            migrationBuilder.DropColumn(name: "ColourHexCode", table: "Teams");

            //migrationBuilder.RenameTable(
            //    name: "AdjacentSeasAsync",
            //    newName: "AdjacentSeas");

            //migrationBuilder.RenameIndex(
            //    name: "IX_AdjacentSeasAsync_SeaId",
            //    table: "AdjacentSeas",
            //    newName: "IX_AdjacentSeas_SeaId");

            //migrationBuilder.RenameIndex(
            //    name: "IX_AdjacentSeasAsync_AdjacentToId",
            //    table: "AdjacentSeas",
            //    newName: "IX_AdjacentSeas_AdjacentToId");

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK_AdjacentSeas",
            //    table: "AdjacentSeas",
            //    column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AdjacentSeas_Seas_AdjacentToId",
                table: "AdjacentSeas",
                column: "AdjacentToId",
                principalTable: "Seas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_AdjacentSeas_Seas_SeaId",
                table: "AdjacentSeas",
                column: "SeaId",
                principalTable: "Seas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdjacentSeas_Seas_AdjacentToId",
                table: "AdjacentSeas"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_AdjacentSeas_Seas_SeaId",
                table: "AdjacentSeas"
            );

            migrationBuilder.DropPrimaryKey(name: "PK_AdjacentSeas", table: "AdjacentSeas");

            migrationBuilder.RenameTable(name: "AdjacentSeas", newName: "AdjacentSeasAsync");

            migrationBuilder.RenameIndex(
                name: "IX_AdjacentSeas_SeaId",
                table: "AdjacentSeasAsync",
                newName: "IX_AdjacentSeasAsync_SeaId"
            );

            migrationBuilder.RenameIndex(
                name: "IX_AdjacentSeas_AdjacentToId",
                table: "AdjacentSeasAsync",
                newName: "IX_AdjacentSeasAsync_AdjacentToId"
            );

            migrationBuilder.AddColumn<string>(
                name: "ColourHexCode",
                table: "Teams",
                type: "TEXT",
                nullable: false,
                defaultValue: ""
            );

            migrationBuilder.AddPrimaryKey(
                name: "PK_AdjacentSeasAsync",
                table: "AdjacentSeasAsync",
                column: "Id"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_AdjacentSeasAsync_Seas_AdjacentToId",
                table: "AdjacentSeasAsync",
                column: "AdjacentToId",
                principalTable: "Seas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_AdjacentSeasAsync_Seas_SeaId",
                table: "AdjacentSeasAsync",
                column: "SeaId",
                principalTable: "Seas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );
        }
    }
}
