using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MemoryApp.Migrations
{
    /// <inheritdoc />
    public partial class AddAppSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AiFeedbackEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    AiEndpoint = table.Column<string>(type: "TEXT", nullable: true),
                    AiModel = table.Column<string>(type: "TEXT", nullable: true),
                    AiApiKey = table.Column<string>(type: "TEXT", nullable: true),
                    AiTimeoutSeconds = table.Column<int>(type: "INTEGER", nullable: false),
                    AiSystemPrompt = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppSettings", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppSettings");
        }
    }
}
