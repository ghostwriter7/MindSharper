using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MindSharper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueConstraintToFlashcardName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Flashcards_DeckId",
                table: "Flashcards");

            migrationBuilder.AlterColumn<string>(
                name: "Frontside",
                table: "Flashcards",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Flashcards_DeckId_Frontside",
                table: "Flashcards",
                columns: new[] { "DeckId", "Frontside" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Flashcards_DeckId_Frontside",
                table: "Flashcards");

            migrationBuilder.AlterColumn<string>(
                name: "Frontside",
                table: "Flashcards",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_Flashcards_DeckId",
                table: "Flashcards",
                column: "DeckId");
        }
    }
}
