using MediatR;

namespace MindSharper.Application.Flashcards.Commands.DeleteFlashcard;

public class DeleteFlashcardCommand(int deckId, int flashcardId) : IRequest
{
    public int DeckId { get; } = deckId;
    public int FlashcardId { get; } = flashcardId;
}