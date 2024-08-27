using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PirateConquest.Migrations
{
    /// <inheritdoc />
    public partial class ReadByRecipient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Read",
                table: "Messages",
                newName: "ReadByRecipient"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) { }
    }
}
