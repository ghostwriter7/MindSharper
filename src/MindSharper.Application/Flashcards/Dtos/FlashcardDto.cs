namespace MindSharper.Application.Flashcards.Dtos;

public class FlashcardDto
{
    public int Id { get; set; }
    public string Frontside { get; set; } = default!;
    public string Backside { get; set; } = default!;
    public DateOnly CreatedAt { get; set; } = default;
}