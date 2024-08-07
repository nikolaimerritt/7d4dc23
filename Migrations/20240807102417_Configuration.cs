using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PirateConquest.Migrations
{
    /// <inheritdoc />
    public partial class Configuration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CooldownMinutes",
                table: "AppConfigs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder.AddColumn<int>(
                name: "CtfId",
                table: "AppConfigs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "FirstRoundStartUtc",
                table: "AppConfigs",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)
            );

            migrationBuilder.AddColumn<int>(
                name: "PlanningMinutes",
                table: "AppConfigs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder.AddColumn<string>(
                name: "PlaygroundLeaderboardUrl",
                table: "AppConfigs",
                type: "TEXT",
                nullable: false,
                defaultValue: ""
            );

            migrationBuilder.AddColumn<int>(
                name: "RoundsCount",
                table: "AppConfigs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder.CreateTable(
                name: "Configurations",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PlaygroundLeaderboardUrl = table.Column<string>(type: "TEXT", nullable: false),
                    CtfId = table.Column<int>(type: "INTEGER", nullable: false),
                    RoundsCount = table.Column<int>(type: "INTEGER", nullable: false),
                    PlanningMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                    CooldownMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                    FirstRoundStart = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configurations", x => x.Id);
                }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Configurations");

            migrationBuilder.DropColumn(name: "CooldownMinutes", table: "AppConfigs");

            migrationBuilder.DropColumn(name: "CtfId", table: "AppConfigs");

            migrationBuilder.DropColumn(name: "FirstRoundStartUtc", table: "AppConfigs");

            migrationBuilder.DropColumn(name: "PlanningMinutes", table: "AppConfigs");

            migrationBuilder.DropColumn(name: "PlaygroundLeaderboardUrl", table: "AppConfigs");

            migrationBuilder.DropColumn(name: "RoundsCount", table: "AppConfigs");
        }
    }
}
