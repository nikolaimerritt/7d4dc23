using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PirateConquest.Migrations
{
    /// <inheritdoc />
    public partial class RemoveOldConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "AppConfigs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppConfigs",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CooldownMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                    CtfId = table.Column<int>(type: "INTEGER", nullable: false),
                    FirstRoundStart = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Key = table.Column<string>(type: "TEXT", nullable: false),
                    PlanningMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                    PlaygroundLeaderboardUrl = table.Column<string>(type: "TEXT", nullable: false),
                    RoundsCount = table.Column<int>(type: "INTEGER", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppConfigs", x => x.Id);
                }
            );
        }
    }
}
