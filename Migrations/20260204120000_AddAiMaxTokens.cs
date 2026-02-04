using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MemoryApp.Migrations
{
    /// <inheritdoc />
    public partial class AddAiMaxTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AiMaxTokens",
                table: "AppSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 150);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AiMaxTokens",
                table: "AppSettings");
        }
    }
}
