using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tarkov.API.Migrations
{
    /// <inheritdoc />
    public partial class AddItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ShortName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NormalizedName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    WikiLink = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    IconLink = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    IconLinkFallback = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ImageLink = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ImageLinkFallback = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Image512pxLink = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Image8xLink = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    BaseImageLink = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    GridImageLink = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    GridImageLinkFallback = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    InspectImageLink = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Link = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    BasePrice = table.Column<int>(type: "int", nullable: false),
                    BsgCategoryId = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Height = table.Column<float>(type: "real", nullable: false),
                    Width = table.Column<float>(type: "real", nullable: false),
                    Weight = table.Column<float>(type: "real", nullable: false),
                    AccuracyModifier = table.Column<float>(type: "real", nullable: true),
                    RecoilModifier = table.Column<float>(type: "real", nullable: true),
                    ErgonomicsModifier = table.Column<float>(type: "real", nullable: true),
                    Velocity = table.Column<float>(type: "real", nullable: true),
                    Loudness = table.Column<float>(type: "real", nullable: true),
                    BlocksHeadphones = table.Column<bool>(type: "bit", nullable: true),
                    HasGrid = table.Column<bool>(type: "bit", nullable: true),
                    BackgroundColor = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ItemTypes",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemTypes", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "ItemEntityItemTypeEntity",
                columns: table => new
                {
                    ItemsId = table.Column<string>(type: "nvarchar(30)", nullable: false),
                    TypesName = table.Column<string>(type: "nvarchar(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemEntityItemTypeEntity", x => new { x.ItemsId, x.TypesName });
                    table.ForeignKey(
                        name: "FK_ItemEntityItemTypeEntity_ItemTypes_TypesName",
                        column: x => x.TypesName,
                        principalTable: "ItemTypes",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemEntityItemTypeEntity_Items_ItemsId",
                        column: x => x.ItemsId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemEntityItemTypeEntity_TypesName",
                table: "ItemEntityItemTypeEntity",
                column: "TypesName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemEntityItemTypeEntity");

            migrationBuilder.DropTable(
                name: "ItemTypes");

            migrationBuilder.DropTable(
                name: "Items");
        }
    }
}
