using MediatR;
using MindSharper.Application.Flashcards.Dtos;

namespace MindSharper.Application.Flashcards.Queries.GetFlashcards;

public class GetFlashcardsQuery(int deckId) : IRequest<IEnumerable<FlashcardDto>>
{
    public int DeckId { get; } = deckId;
}