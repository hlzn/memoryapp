using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MemoryApp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Key = table.Column<string>(type: "TEXT", nullable: false),
                    AltText = table.Column<string>(type: "TEXT", nullable: false),
                    PathOrData = table.Column<string>(type: "TEXT", nullable: false),
                    Style = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Verbs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Infinitive = table.Column<string>(type: "TEXT", nullable: false),
                    TranslationEs = table.Column<string>(type: "TEXT", nullable: false),
                    Level = table.Column<string>(type: "TEXT", nullable: false),
                    IsSeparable = table.Column<bool>(type: "INTEGER", nullable: false),
                    Prefix = table.Column<string>(type: "TEXT", nullable: true),
                    IsIrregular = table.Column<bool>(type: "INTEGER", nullable: false),
                    Auxiliary = table.Column<string>(type: "TEXT", nullable: false),
                    PartizipII = table.Column<string>(type: "TEXT", nullable: false),
                    PraeteritumIch = table.Column<string>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Verbs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Attempts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Mode = table.Column<string>(type: "TEXT", nullable: false),
                    ItemType = table.Column<string>(type: "TEXT", nullable: false),
                    ItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    IsCorrect = table.Column<bool>(type: "INTEGER", nullable: false),
                    ScoreDelta = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attempts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSettings",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PreferredLevel = table.Column<string>(type: "TEXT", nullable: false),
                    DailyGoal = table.Column<int>(type: "INTEGER", nullable: false),
                    UiTheme = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSettings", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UserSettings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sentences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Level = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    SentenceTemplate = table.Column<string>(type: "TEXT", nullable: false),
                    CorrectAnswer = table.Column<string>(type: "TEXT", nullable: false),
                    Hint = table.Column<string>(type: "TEXT", nullable: true),
                    TranslationEs = table.Column<string>(type: "TEXT", nullable: true),
                    VerbId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sentences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sentences_Verbs_VerbId",
                        column: x => x.VerbId,
                        principalTable: "Verbs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "UserVerbProgress",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    VerbId = table.Column<int>(type: "INTEGER", nullable: false),
                    Mastery = table.Column<int>(type: "INTEGER", nullable: false),
                    NextReviewAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Streak = table.Column<int>(type: "INTEGER", nullable: false),
                    LastResult = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserVerbProgress", x => new { x.UserId, x.VerbId });
                    table.ForeignKey(
                        name: "FK_UserVerbProgress_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserVerbProgress_Verbs_VerbId",
                        column: x => x.VerbId,
                        principalTable: "Verbs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VerbForms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VerbId = table.Column<int>(type: "INTEGER", nullable: false),
                    Tense = table.Column<string>(type: "TEXT", nullable: false),
                    Person = table.Column<string>(type: "TEXT", nullable: false),
                    Form = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerbForms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VerbForms_Verbs_VerbId",
                        column: x => x.VerbId,
                        principalTable: "Verbs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VerbImages",
                columns: table => new
                {
                    VerbId = table.Column<int>(type: "INTEGER", nullable: false),
                    ImageId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerbImages", x => new { x.VerbId, x.ImageId });
                    table.ForeignKey(
                        name: "FK_VerbImages_Images_ImageId",
                        column: x => x.ImageId,
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VerbImages_Verbs_VerbId",
                        column: x => x.VerbId,
                        principalTable: "Verbs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Attempts_UserId",
                table: "Attempts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Images_Key",
                table: "Images",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sentences_VerbId",
                table: "Sentences",
                column: "VerbId");

            migrationBuilder.CreateIndex(
                name: "IX_UserVerbProgress_VerbId",
                table: "UserVerbProgress",
                column: "VerbId");

            migrationBuilder.CreateIndex(
                name: "IX_VerbForms_VerbId",
                table: "VerbForms",
                column: "VerbId");

            migrationBuilder.CreateIndex(
                name: "IX_VerbImages_ImageId",
                table: "VerbImages",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Verbs_Infinitive",
                table: "Verbs",
                column: "Infinitive",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Attempts");

            migrationBuilder.DropTable(
                name: "Sentences");

            migrationBuilder.DropTable(
                name: "UserSettings");

            migrationBuilder.DropTable(
                name: "UserVerbProgress");

            migrationBuilder.DropTable(
                name: "VerbForms");

            migrationBuilder.DropTable(
                name: "VerbImages");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropTable(
                name: "Verbs");
        }
    }
}
