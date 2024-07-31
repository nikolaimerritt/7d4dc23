using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PirateConquest.Migrations
{
    /// <inheritdoc />
    public partial class ShipsBeforeAfter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.RenameColumn(
            //    name: "ShipsAfter",
            //    table: "Purchases",
            //    newName: "ShipCount");

            //migrationBuilder.RenameColumn(
            //    name: "SeasHeld",
            //    table: "Purchases",
            //    newName: "Points");

            //migrationBuilder.RenameColumn(
            //    name: "ShipsAfter",
            //    table: "Moves",
            //    newName: "ShipCount");
            migrationBuilder.RenameColumn(
                name: "ShipCount",
                table: "Outcomes",
                newName: "ShipsAfter"
            );

            migrationBuilder.AddColumn<int>(
                name: "ShipsBefore",
                table: "Outcomes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "ShipsBefore", table: "Outcomes");

            migrationBuilder.RenameColumn(
                name: "ShipCount",
                table: "Purchases",
                newName: "ShipsAfter"
            );

            migrationBuilder.RenameColumn(name: "Points", table: "Purchases", newName: "SeasHeld");

            migrationBuilder.RenameColumn(name: "ShipCount", table: "Moves", newName: "ShipsAfter");
        }
    }
}
