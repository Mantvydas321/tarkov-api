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
                name: "TranslationKeys",
                columns: table => new
                {
                    Key = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TranslationKeys", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "Achievements",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    NameTranslationKey = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DescriptionTranslationKey = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Hidden = table.Column<bool>(type: "bit", nullable: false),
                    Side = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Rarity = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    PlayersCompletedPercentage = table.Column<float>(type: "real", nullable: false),
                    AdjustedPlayersCompletedPercentage = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Achievements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Achievements_TranslationKeys_DescriptionTranslationKey",
                        column: x => x.DescriptionTranslationKey,
                        principalTable: "TranslationKeys",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Achievements_TranslationKeys_NameTranslationKey",
                        column: x => x.NameTranslationKey,
                        principalTable: "TranslationKeys",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Translations",
                columns: table => new
                {
                    Key = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Language = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Translations", x => new { x.Key, x.Language });
                    table.ForeignKey(
                        name: "FK_Translations_TranslationKeys_Key",
                        column: x => x.Key,
                        principalTable: "TranslationKeys",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Achievements_DescriptionTranslationKey",
                table: "Achievements",
                column: "DescriptionTranslationKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Achievements_NameTranslationKey",
                table: "Achievements",
                column: "NameTranslationKey",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Achievements");

            migrationBuilder.DropTable(
                name: "Translations");

            migrationBuilder.DropTable(
                name: "TranslationKeys");
        }
    }
}
