using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MemoryApp.Migrations
{
    /// <inheritdoc />
    public partial class AddNouns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Nouns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    German = table.Column<string>(type: "TEXT", nullable: false),
                    Article = table.Column<string>(type: "TEXT", nullable: false),
                    TranslationEs = table.Column<string>(type: "TEXT", nullable: false),
                    Level = table.Column<string>(type: "TEXT", nullable: false),
                    Plural = table.Column<string>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nouns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserNounProgress",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    NounId = table.Column<int>(type: "INTEGER", nullable: false),
                    Mastery = table.Column<int>(type: "INTEGER", nullable: false),
                    NextReviewAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Streak = table.Column<int>(type: "INTEGER", nullable: false),
                    LastResult = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserNounProgress", x => new { x.UserId, x.NounId });
                    table.ForeignKey(
                        name: "FK_UserNounProgress_Nouns_NounId",
                        column: x => x.NounId,
                        principalTable: "Nouns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserNounProgress_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Nouns_German",
                table: "Nouns",
                column: "German",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserNounProgress_NounId",
                table: "UserNounProgress",
                column: "NounId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserNounProgress");

            migrationBuilder.DropTable(
                name: "Nouns");
        }
    }
}
