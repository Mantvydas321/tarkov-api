using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tarkov.API.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Achievements",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Hidden = table.Column<bool>(type: "bit", nullable: false),
                    Side = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Rarity = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    PlayersCompletedPercentage = table.Column<float>(type: "real", nullable: false),
                    AdjustedPlayersCompletedPercentage = table.Column<float>(type: "real", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Achievements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CronExpression = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NextScheduledRun = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastRun = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastRunSuccessful = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AchievementTranslations",
                columns: table => new
                {
                    AchievementId = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Language = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    Field = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AchievementTranslations", x => new { x.AchievementId, x.Language, x.Field });
                    table.ForeignKey(
                        name: "FK_AchievementTranslations_Achievements_AchievementId",
                        column: x => x.AchievementId,
                        principalTable: "Achievements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskExecutions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Success = table.Column<bool>(type: "bit", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", maxLength: 4096, nullable: true),
                    EntitiesUpdated = table.Column<int>(type: "int", nullable: false),
                    EntitiesCreated = table.Column<int>(type: "int", nullable: false),
                    EntitiesDeleted = table.Column<int>(type: "int", nullable: false),
                    Start = table.Column<DateTime>(type: "datetime2", nullable: false),
                    End = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Duration = table.Column<long>(type: "bigint", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskExecutions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskExecutions_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskExecutions_TaskId",
                table: "TaskExecutions",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_Name",
                table: "Tasks",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AchievementTranslations");

            migrationBuilder.DropTable(
                name: "TaskExecutions");

            migrationBuilder.DropTable(
                name: "Achievements");

            migrationBuilder.DropTable(
                name: "Tasks");
        }
    }
}
