using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PirateConquest.Migrations
{
    /// <inheritdoc />
    public partial class PurchaseTiedToSea : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SeaId",
                table: "Purchases",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_SeaId",
                table: "Purchases",
                column: "SeaId"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_Seas_SeaId",
                table: "Purchases",
                column: "SeaId",
                principalTable: "Seas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_Purchases_Seas_SeaId", table: "Purchases");

            migrationBuilder.DropIndex(name: "IX_Purchases_SeaId", table: "Purchases");

            migrationBuilder.DropColumn(name: "SeaId", table: "Purchases");
        }
    }
}
