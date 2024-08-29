using MindSharper.Domain.Entities;

namespace MindSharper.Application.Decks.Dtos;

public class MinimalDeckDto
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public DateOnly CreatedAt { get; set; } = default!;
    public int Rate { get; set; }
}