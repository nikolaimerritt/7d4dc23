using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PirateConquest.Migrations
{
    /// <inheritdoc />
    public partial class MaxMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxMessageCharacters",
                table: "Configurations",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder.AddColumn<int>(
                name: "MaxMessagesPerTeam",
                table: "Configurations",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "MaxMessageCharacters", table: "Configurations");

            migrationBuilder.DropColumn(name: "MaxMessagesPerTeam", table: "Configurations");
        }
    }
}
