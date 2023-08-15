using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlayerManagement.Data.PlayerManagementMigrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "SL");

            migrationBuilder.CreateTable(
                name: "Fields",
                schema: "SL",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Comments = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    GoogleMapsLink = table.Column<string>(type: "TEXT", maxLength: 700, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fields", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Leagues",
                schema: "SL",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    LeagueFoundation = table.Column<string>(type: "TEXT", maxLength: 4, nullable: false),
                    LeagueBudget = table.Column<double>(type: "REAL", nullable: false),
                    NumberOfTeams = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leagues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlayerPositions",
                schema: "SL",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PlayerPos = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerPositions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TeamStatsVM",
                schema: "SL",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TeamName = table.Column<string>(type: "TEXT", nullable: true),
                    Goals = table.Column<int>(type: "INTEGER", nullable: false),
                    RedCards = table.Column<int>(type: "INTEGER", nullable: false),
                    YellowCards = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamStatsVM", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                schema: "SL",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LeagueId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "BLOB", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Teams_Leagues_LeagueId",
                        column: x => x.LeagueId,
                        principalSchema: "SL",
                        principalTable: "Leagues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MatchSchedules",
                schema: "SL",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MatchDay = table.Column<int>(type: "INTEGER", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Time = table.Column<string>(type: "TEXT", nullable: false),
                    FieldId = table.Column<int>(type: "INTEGER", nullable: false),
                    HomeTeamId = table.Column<int>(type: "INTEGER", nullable: false),
                    AwayTeamId = table.Column<int>(type: "INTEGER", nullable: false),
                    HomeTeamScore = table.Column<int>(type: "INTEGER", nullable: true),
                    AwayTeamScore = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MatchSchedules_Fields_FieldId",
                        column: x => x.FieldId,
                        principalSchema: "SL",
                        principalTable: "Fields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MatchSchedules_Teams_AwayTeamId",
                        column: x => x.AwayTeamId,
                        principalSchema: "SL",
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MatchSchedules_Teams_HomeTeamId",
                        column: x => x.HomeTeamId,
                        principalSchema: "SL",
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                schema: "SL",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    DOB = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TeamId = table.Column<int>(type: "INTEGER", nullable: false),
                    PlayerPositionId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "BLOB", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Players_PlayerPositions_PlayerPositionId",
                        column: x => x.PlayerPositionId,
                        principalSchema: "SL",
                        principalTable: "PlayerPositions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Players_Teams_TeamId",
                        column: x => x.TeamId,
                        principalSchema: "SL",
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerMatchs",
                schema: "SL",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Goals = table.Column<int>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    RedCards = table.Column<int>(type: "INTEGER", nullable: true),
                    YellowCards = table.Column<int>(type: "INTEGER", nullable: true),
                    MatchId = table.Column<int>(type: "INTEGER", nullable: false),
                    PlayerId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerMatchs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerMatchs_MatchSchedules_MatchId",
                        column: x => x.MatchId,
                        principalSchema: "SL",
                        principalTable: "MatchSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerMatchs_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalSchema: "SL",
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerPhotos",
                schema: "SL",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Content = table.Column<byte[]>(type: "BLOB", nullable: true),
                    MimeType = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    PlayerId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerPhotos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerPhotos_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalSchema: "SL",
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerTeam",
                schema: "SL",
                columns: table => new
                {
                    TeamId = table.Column<int>(type: "INTEGER", nullable: false),
                    PlayerId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerTeam", x => new { x.PlayerId, x.TeamId });
                    table.ForeignKey(
                        name: "FK_PlayerTeam_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalSchema: "SL",
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerTeam_Teams_TeamId",
                        column: x => x.TeamId,
                        principalSchema: "SL",
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerThumbnails",
                schema: "SL",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Content = table.Column<byte[]>(type: "BLOB", nullable: true),
                    MimeType = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    PlayerId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerThumbnails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerThumbnails_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalSchema: "SL",
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Plays",
                schema: "SL",
                columns: table => new
                {
                    PlayerId = table.Column<int>(type: "INTEGER", nullable: false),
                    PlayerPositionId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plays", x => new { x.PlayerId, x.PlayerPositionId });
                    table.ForeignKey(
                        name: "FK_Plays_PlayerPositions_PlayerPositionId",
                        column: x => x.PlayerPositionId,
                        principalSchema: "SL",
                        principalTable: "PlayerPositions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Plays_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalSchema: "SL",
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UploadedFiles",
                schema: "SL",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FileName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    MimeType = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    Discriminator = table.Column<string>(type: "TEXT", nullable: false),
                    PlayerId = table.Column<int>(type: "INTEGER", nullable: true),
                    TeamId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UploadedFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UploadedFiles_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalSchema: "SL",
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UploadedFiles_Teams_TeamId",
                        column: x => x.TeamId,
                        principalSchema: "SL",
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FileContent",
                schema: "SL",
                columns: table => new
                {
                    FileContentId = table.Column<int>(type: "INTEGER", nullable: false),
                    Content = table.Column<byte[]>(type: "BLOB", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileContent", x => x.FileContentId);
                    table.ForeignKey(
                        name: "FK_FileContent_UploadedFiles_FileContentId",
                        column: x => x.FileContentId,
                        principalSchema: "SL",
                        principalTable: "UploadedFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Leagues_Name",
                schema: "SL",
                table: "Leagues",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MatchSchedules_AwayTeamId",
                schema: "SL",
                table: "MatchSchedules",
                column: "AwayTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchSchedules_FieldId",
                schema: "SL",
                table: "MatchSchedules",
                column: "FieldId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchSchedules_HomeTeamId",
                schema: "SL",
                table: "MatchSchedules",
                column: "HomeTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerMatchs_MatchId",
                schema: "SL",
                table: "PlayerMatchs",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerMatchs_PlayerId",
                schema: "SL",
                table: "PlayerMatchs",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerPhotos_PlayerId",
                schema: "SL",
                table: "PlayerPhotos",
                column: "PlayerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerPositions_PlayerPos",
                schema: "SL",
                table: "PlayerPositions",
                column: "PlayerPos",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Players_Email_Phone",
                schema: "SL",
                table: "Players",
                columns: new[] { "Email", "Phone" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Players_PlayerPositionId",
                schema: "SL",
                table: "Players",
                column: "PlayerPositionId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_TeamId",
                schema: "SL",
                table: "Players",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerTeam_TeamId",
                schema: "SL",
                table: "PlayerTeam",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerThumbnails_PlayerId",
                schema: "SL",
                table: "PlayerThumbnails",
                column: "PlayerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Plays_PlayerPositionId",
                schema: "SL",
                table: "Plays",
                column: "PlayerPositionId");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_LeagueId",
                schema: "SL",
                table: "Teams",
                column: "LeagueId");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_Name",
                schema: "SL",
                table: "Teams",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UploadedFiles_PlayerId",
                schema: "SL",
                table: "UploadedFiles",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_UploadedFiles_TeamId",
                schema: "SL",
                table: "UploadedFiles",
                column: "TeamId");

            ExtraMigration.Steps(migrationBuilder);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileContent",
                schema: "SL");

            migrationBuilder.DropTable(
                name: "PlayerMatchs",
                schema: "SL");

            migrationBuilder.DropTable(
                name: "PlayerPhotos",
                schema: "SL");

            migrationBuilder.DropTable(
                name: "PlayerTeam",
                schema: "SL");

            migrationBuilder.DropTable(
                name: "PlayerThumbnails",
                schema: "SL");

            migrationBuilder.DropTable(
                name: "Plays",
                schema: "SL");

            migrationBuilder.DropTable(
                name: "TeamStatsVM",
                schema: "SL");

            migrationBuilder.DropTable(
                name: "UploadedFiles",
                schema: "SL");

            migrationBuilder.DropTable(
                name: "MatchSchedules",
                schema: "SL");

            migrationBuilder.DropTable(
                name: "Players",
                schema: "SL");

            migrationBuilder.DropTable(
                name: "Fields",
                schema: "SL");

            migrationBuilder.DropTable(
                name: "PlayerPositions",
                schema: "SL");

            migrationBuilder.DropTable(
                name: "Teams",
                schema: "SL");

            migrationBuilder.DropTable(
                name: "Leagues",
                schema: "SL");
        }
    }
}
