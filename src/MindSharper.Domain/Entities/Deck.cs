namespace MindSharper.Domain.Entities;

public class Deck
{
    public int Id { get; set; }
    public string UserId { get; set; } = default!;
    public User Owner { get; set; } = default!;
    public string Name { get; set; } = default!;
    public List<Flashcard> Flashcards { get; set; } = [];
    public DateOnly CreatedAt { get; set; } = default;
    public byte Rate { get; set; }
}