using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PirateConquest.Migrations
{
    /// <inheritdoc />
    public partial class _2024_08_05 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartMoving",
                table: "Rounds",
                newName: "StartPlanning"
            );

            migrationBuilder.RenameColumn(
                name: "StartFighting",
                table: "Rounds",
                newName: "StartCooldown"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.RenameColumn(
            //    name: "Name",
            //    table: "Teams",
            //    newName: "Key");

            //migrationBuilder.RenameColumn(
            //    name: "Name",
            //    table: "Seas",
            //    newName: "Key");
        }
    }
}
