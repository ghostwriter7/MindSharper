using MediatR;
using MindSharper.Application.Flashcards.Dtos;

namespace MindSharper.Application.Flashcards.Queries.GetFlashcardById;

public class GetFlashcardByIdQuery(int deckId, int flashcardId) : IRequest<FlashcardDto?>
{
    public int DeckId { get; } = deckId;
    public int FlashcardId { get; } = flashcardId;
}