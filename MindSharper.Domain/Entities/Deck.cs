namespace MindSharper.Domain.Entities;

public class Deck
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public List<Flashcard> Flashcards { get; set; } = [];
    public DateOnly CreatedAt { get; set; } = default;
    public byte Rate { get; set; }
}