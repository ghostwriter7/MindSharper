using MindSharper.Domain.Entities;

namespace MindSharper.Application.Decks.Dtos;

public abstract class DeckDto : MinimalDeckDto
{
    public IEnumerable<Flashcard> Flashcards { get; set; }
}