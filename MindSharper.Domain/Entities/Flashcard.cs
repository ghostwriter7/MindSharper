namespace MindSharper.Domain.Entities;

public class Flashcard
{
    public int Id { get; set; }
    public string Frontside { get; set; } = default!;
    public string Backside { get; set; } = default!;
    public DateOnly CreatedAt { get; set; } = default;
    public DateTime? ReviewedAt { get; set; } 
}