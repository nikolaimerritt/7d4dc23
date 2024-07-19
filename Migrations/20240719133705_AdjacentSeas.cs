using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PirateConquest.Migrations
{
    /// <inheritdoc />
    public partial class AdjacentSeas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StartingSeaId",
                table: "Teams",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder.CreateTable(
                name: "AdjacentSeas",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SeaId = table.Column<int>(type: "INTEGER", nullable: false),
                    AdjacentToId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdjacentSeas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdjacentSeas_Seas_AdjacentToId",
                        column: x => x.AdjacentToId,
                        principalTable: "Seas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_AdjacentSeas_Seas_SeaId",
                        column: x => x.SeaId,
                        principalTable: "Seas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_Teams_StartingSeaId",
                table: "Teams",
                column: "StartingSeaId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_AdjacentSeas_AdjacentToId",
                table: "AdjacentSeas",
                column: "AdjacentToId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_AdjacentSeas_SeaId",
                table: "AdjacentSeas",
                column: "SeaId"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_Seas_StartingSeaId",
                table: "Teams",
                column: "StartingSeaId",
                principalTable: "Seas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_Teams_Seas_StartingSeaId", table: "Teams");

            migrationBuilder.DropTable(name: "AdjacentSeas");

            migrationBuilder.DropIndex(name: "IX_Teams_StartingSeaId", table: "Teams");

            migrationBuilder.DropColumn(name: "StartingSeaId", table: "Teams");
        }
    }
}
