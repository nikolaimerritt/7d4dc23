using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PirateConquest.Migrations
{
    /// <inheritdoc />
    public partial class InitialRoundExplicit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "Flags");

            //migrationBuilder.DropTable(
            //    name: "Guesses");

            //migrationBuilder.DropTable(
            //    name: "Hints");

            //migrationBuilder.DropTable(
            //    name: "UnlockedIntels");

            //migrationBuilder.DropTable(
            //    name: "Suspects");

            //migrationBuilder.DropTable(
            //    name: "Users");

            migrationBuilder.AddColumn<bool>(
                name: "IsInitial",
                table: "Rounds",
                type: "INTEGER",
                nullable: false,
                defaultValue: false
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "IsInitial", table: "Rounds");

            migrationBuilder.CreateTable(
                name: "Flags",
                columns: table => new
                {
                    FlagId = table
                        .Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Points = table.Column<int>(type: "INTEGER", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flags", x => x.FlagId);
                }
            );

            migrationBuilder.CreateTable(
                name: "Hints",
                columns: table => new
                {
                    HintId = table
                        .Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    RequiredPoints = table.Column<int>(type: "INTEGER", nullable: false),
                    Text = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hints", x => x.HintId);
                }
            );

            migrationBuilder.CreateTable(
                name: "Suspects",
                columns: table => new
                {
                    SuspectId = table
                        .Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsCulprit = table.Column<bool>(type: "INTEGER", nullable: false),
                    Location = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    OperatingSystem = table.Column<string>(type: "TEXT", nullable: false),
                    Skills = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suspects", x => x.SuspectId);
                }
            );

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table
                        .Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsAdmin = table.Column<bool>(type: "INTEGER", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: false),
                    Points = table.Column<int>(type: "INTEGER", nullable: false),
                    Username = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                }
            );

            migrationBuilder.CreateTable(
                name: "Guesses",
                columns: table => new
                {
                    GuessId = table
                        .Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MastermindSuspectId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    Correct = table.Column<bool>(type: "INTEGER", nullable: false),
                    TimeGuessed = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guesses", x => x.GuessId);
                    table.ForeignKey(
                        name: "FK_Guesses_Suspects_MastermindSuspectId",
                        column: x => x.MastermindSuspectId,
                        principalTable: "Suspects",
                        principalColumn: "SuspectId",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_Guesses_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "UnlockedIntels",
                columns: table => new
                {
                    UnlockedIntelId = table
                        .Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SuspectId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    TimeUnlocked = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnlockedIntels", x => x.UnlockedIntelId);
                    table.ForeignKey(
                        name: "FK_UnlockedIntels_Suspects_SuspectId",
                        column: x => x.SuspectId,
                        principalTable: "Suspects",
                        principalColumn: "SuspectId",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_UnlockedIntels_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_Guesses_MastermindSuspectId",
                table: "Guesses",
                column: "MastermindSuspectId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Guesses_UserId",
                table: "Guesses",
                column: "UserId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_UnlockedIntels_SuspectId",
                table: "UnlockedIntels",
                column: "SuspectId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_UnlockedIntels_UserId",
                table: "UnlockedIntels",
                column: "UserId"
            );
        }
    }
}
