using MindSharper.Application.Flashcards.Dtos;
using MindSharper.Domain.Entities;

namespace MindSharper.Application.Decks.Dtos;

public class DeckDto : MinimalDeckDto
{
    public IEnumerable<FlashcardDto> Flashcards { get; set; }
}