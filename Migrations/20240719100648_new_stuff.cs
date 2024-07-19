using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PirateConquest.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateTable(
            //    name: "AppConfigs",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "INTEGER", nullable: false)
            //            .Annotation("Sqlite:Autoincrement", true),
            //        Name = table.Column<string>(type: "TEXT", nullable: false),
            //        Value = table.Column<string>(type: "TEXT", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_AppConfigs", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Flags",
            //    columns: table => new
            //    {
            //        FlagId = table.Column<int>(type: "INTEGER", nullable: false)
            //            .Annotation("Sqlite:Autoincrement", true),
            //        Name = table.Column<string>(type: "TEXT", nullable: false),
            //        Value = table.Column<string>(type: "TEXT", nullable: false),
            //        Points = table.Column<int>(type: "INTEGER", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Flags", x => x.FlagId);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Hints",
            //    columns: table => new
            //    {
            //        HintId = table.Column<int>(type: "INTEGER", nullable: false)
            //            .Annotation("Sqlite:Autoincrement", true),
            //        Name = table.Column<string>(type: "TEXT", nullable: false),
            //        Text = table.Column<string>(type: "TEXT", nullable: false),
            //        RequiredPoints = table.Column<int>(type: "INTEGER", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Hints", x => x.HintId);
            //    });

            migrationBuilder.CreateTable(
                name: "Rounds",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StartMoving = table.Column<DateTime>(type: "TEXT", nullable: false),
                    StartFighting = table.Column<DateTime>(type: "TEXT", nullable: false),
                    End = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rounds", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "Seas",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seas", x => x.Id);
                }
            );

            //migrationBuilder.CreateTable(
            //    name: "Suspects",
            //    columns: table => new
            //    {
            //        SuspectId = table.Column<int>(type: "INTEGER", nullable: false)
            //            .Annotation("Sqlite:Autoincrement", true),
            //        Name = table.Column<string>(type: "TEXT", nullable: false),
            //        Location = table.Column<string>(type: "TEXT", nullable: false),
            //        OperatingSystem = table.Column<string>(type: "TEXT", nullable: false),
            //        Skills = table.Column<string>(type: "TEXT", nullable: false),
            //        IsCulprit = table.Column<bool>(type: "INTEGER", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Suspects", x => x.SuspectId);
            //    });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ColourHexCode = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                }
            );

            //migrationBuilder.CreateTable(
            //    name: "Users",
            //    columns: table => new
            //    {
            //        UserId = table.Column<int>(type: "INTEGER", nullable: false)
            //            .Annotation("Sqlite:Autoincrement", true),
            //        Username = table.Column<string>(type: "TEXT", nullable: false),
            //        Password = table.Column<string>(type: "TEXT", nullable: false),
            //        Points = table.Column<int>(type: "INTEGER", nullable: false),
            //        IsAdmin = table.Column<bool>(type: "INTEGER", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Users", x => x.UserId);
            //    });

            migrationBuilder.CreateTable(
                name: "Moves",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoundId = table.Column<int>(type: "INTEGER", nullable: false),
                    TeamId = table.Column<int>(type: "INTEGER", nullable: false),
                    FromSeaId = table.Column<int>(type: "INTEGER", nullable: false),
                    ToSeaId = table.Column<int>(type: "INTEGER", nullable: false),
                    ShipCount = table.Column<int>(type: "INTEGER", nullable: false),
                    Creation = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Moves", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Moves_Rounds_RoundId",
                        column: x => x.RoundId,
                        principalTable: "Rounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_Moves_Seas_FromSeaId",
                        column: x => x.FromSeaId,
                        principalTable: "Seas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_Moves_Seas_ToSeaId",
                        column: x => x.ToSeaId,
                        principalTable: "Seas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_Moves_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "Outcomes",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoundId = table.Column<int>(type: "INTEGER", nullable: false),
                    TeamId = table.Column<int>(type: "INTEGER", nullable: false),
                    SeaId = table.Column<int>(type: "INTEGER", nullable: false),
                    ShipCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Outcomes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Outcomes_Rounds_RoundId",
                        column: x => x.RoundId,
                        principalTable: "Rounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_Outcomes_Seas_SeaId",
                        column: x => x.SeaId,
                        principalTable: "Seas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_Outcomes_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "Purchases",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TeamId = table.Column<int>(type: "INTEGER", nullable: false),
                    RoundId = table.Column<int>(type: "INTEGER", nullable: false),
                    Points = table.Column<int>(type: "INTEGER", nullable: false),
                    ShipCount = table.Column<int>(type: "INTEGER", nullable: false),
                    Creation = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Purchases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Purchases_Rounds_RoundId",
                        column: x => x.RoundId,
                        principalTable: "Rounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_Purchases_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            //migrationBuilder.CreateTable(
            //    name: "Guesses",
            //    columns: table => new
            //    {
            //        GuessId = table.Column<int>(type: "INTEGER", nullable: false)
            //            .Annotation("Sqlite:Autoincrement", true),
            //        UserId = table.Column<int>(type: "INTEGER", nullable: false),
            //        Correct = table.Column<bool>(type: "INTEGER", nullable: false),
            //        MastermindSuspectId = table.Column<int>(type: "INTEGER", nullable: false),
            //        TimeGuessed = table.Column<DateTime>(type: "TEXT", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Guesses", x => x.GuessId);
            //        table.ForeignKey(
            //            name: "FK_Guesses_Suspects_MastermindSuspectId",
            //            column: x => x.MastermindSuspectId,
            //            principalTable: "Suspects",
            //            principalColumn: "SuspectId",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_Guesses_Users_UserId",
            //            column: x => x.UserId,
            //            principalTable: "Users",
            //            principalColumn: "UserId",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "UnlockedIntels",
            //    columns: table => new
            //    {
            //        UnlockedIntelId = table.Column<int>(type: "INTEGER", nullable: false)
            //            .Annotation("Sqlite:Autoincrement", true),
            //        UserId = table.Column<int>(type: "INTEGER", nullable: false),
            //        SuspectId = table.Column<int>(type: "INTEGER", nullable: false),
            //        TimeUnlocked = table.Column<DateTime>(type: "TEXT", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_UnlockedIntels", x => x.UnlockedIntelId);
            //        table.ForeignKey(
            //            name: "FK_UnlockedIntels_Suspects_SuspectId",
            //            column: x => x.SuspectId,
            //            principalTable: "Suspects",
            //            principalColumn: "SuspectId",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_UnlockedIntels_Users_UserId",
            //            column: x => x.UserId,
            //            principalTable: "Users",
            //            principalColumn: "UserId",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateIndex(
            //    name: "IX_Guesses_MastermindSuspectId",
            //    table: "Guesses",
            //    column: "MastermindSuspectId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Guesses_UserId",
            //    table: "Guesses",
            //    column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Moves_FromSeaId",
                table: "Moves",
                column: "FromSeaId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Moves_RoundId",
                table: "Moves",
                column: "RoundId"
            );

            migrationBuilder.CreateIndex(name: "IX_Moves_TeamId", table: "Moves", column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Moves_ToSeaId",
                table: "Moves",
                column: "ToSeaId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Outcomes_RoundId",
                table: "Outcomes",
                column: "RoundId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Outcomes_SeaId",
                table: "Outcomes",
                column: "SeaId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Outcomes_TeamId",
                table: "Outcomes",
                column: "TeamId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_RoundId",
                table: "Purchases",
                column: "RoundId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_TeamId",
                table: "Purchases",
                column: "TeamId"
            );

            //migrationBuilder.CreateIndex(
            //    name: "IX_UnlockedIntels_SuspectId",
            //    table: "UnlockedIntels",
            //    column: "SuspectId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_UnlockedIntels_UserId",
            //    table: "UnlockedIntels",
            //    column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "AppConfigs");

            migrationBuilder.DropTable(name: "Flags");

            migrationBuilder.DropTable(name: "Guesses");

            migrationBuilder.DropTable(name: "Hints");

            migrationBuilder.DropTable(name: "Moves");

            migrationBuilder.DropTable(name: "Outcomes");

            migrationBuilder.DropTable(name: "Purchases");

            migrationBuilder.DropTable(name: "UnlockedIntels");

            migrationBuilder.DropTable(name: "Seas");

            migrationBuilder.DropTable(name: "Rounds");

            migrationBuilder.DropTable(name: "Teams");

            migrationBuilder.DropTable(name: "Suspects");

            migrationBuilder.DropTable(name: "Users");
        }
    }
}
