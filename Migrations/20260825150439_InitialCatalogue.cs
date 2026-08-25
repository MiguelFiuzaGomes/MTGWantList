using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MTGWantList.Migrations
{
    /// <inheritdoc />
    public partial class InitialCatalogue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ScryfallOracleId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CardSets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MtgJsonCode = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ReleaseDate = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardSets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Printings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MtgJsonUuid = table.Column<string>(type: "text", nullable: false),
                    CollectorNumber = table.Column<string>(type: "text", nullable: false),
                    Rarity = table.Column<string>(type: "text", nullable: true),
                    ScryfallId = table.Column<string>(type: "text", nullable: true),
                    CardmarketId = table.Column<string>(type: "text", nullable: true),
                    CardId = table.Column<int>(type: "integer", nullable: false),
                    CardSetId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Printings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Printings_CardSets_CardSetId",
                        column: x => x.CardSetId,
                        principalTable: "CardSets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Printings_Cards_CardId",
                        column: x => x.CardId,
                        principalTable: "Cards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PrintingVariants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Language = table.Column<string>(type: "text", nullable: false),
                    Finish = table.Column<string>(type: "text", nullable: false),
                    MtgJsonUuid = table.Column<string>(type: "text", nullable: true),
                    MtgJsonVersionId = table.Column<string>(type: "text", nullable: true),
                    ScryfallId = table.Column<string>(type: "text", nullable: true),
                    PrintingId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrintingVariants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrintingVariants_Printings_PrintingId",
                        column: x => x.PrintingId,
                        principalTable: "Printings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cards_ScryfallOracleId",
                table: "Cards",
                column: "ScryfallOracleId",
                unique: true,
                filter: "\"ScryfallOracleId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CardSets_MtgJsonCode",
                table: "CardSets",
                column: "MtgJsonCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Printings_CardId",
                table: "Printings",
                column: "CardId");

            migrationBuilder.CreateIndex(
                name: "IX_Printings_CardSetId",
                table: "Printings",
                column: "CardSetId");

            migrationBuilder.CreateIndex(
                name: "IX_Printings_MtgJsonUuid",
                table: "Printings",
                column: "MtgJsonUuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PrintingVariants_PrintingId_Language_Finish",
                table: "PrintingVariants",
                columns: new[] { "PrintingId", "Language", "Finish" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PrintingVariants");

            migrationBuilder.DropTable(
                name: "Printings");

            migrationBuilder.DropTable(
                name: "CardSets");

            migrationBuilder.DropTable(
                name: "Cards");
        }
    }
}
