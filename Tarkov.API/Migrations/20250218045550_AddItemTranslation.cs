using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tarkov.API.Migrations
{
    /// <inheritdoc />
    public partial class AddItemTranslation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ItemTranslationEntity",
                columns: table => new
                {
                    ItemId = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Language = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    Field = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemTranslationEntity", x => new { x.ItemId, x.Language, x.Field });
                    table.ForeignKey(
                        name: "FK_ItemTranslationEntity_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemTranslationEntity");
        }
    }
}
