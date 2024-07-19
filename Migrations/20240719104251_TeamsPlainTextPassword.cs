using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PirateConquest.Migrations
{
    /// <inheritdoc />
    public partial class TeamsPlainTextPassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.RenameColumn(
            //    name: "PlainTextPassword",
            //    table: "Users",
            //    newName: "Password");

            migrationBuilder.AddColumn<string>(
                name: "PlainTextPassword",
                table: "Teams",
                type: "TEXT",
                nullable: false,
                defaultValue: ""
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "PlainTextPassword", table: "Teams");

            //migrationBuilder.RenameColumn(
            //    name: "Password",
            //    table: "Users",
            //    newName: "PlainTextPassword");
        }
    }
}
