using MediatR;

namespace MindSharper.Application.Flashcards.Commands.UpdateFlashcard;

public class UpdateFlashcardCommand : IRequest
{
    public int FlashcardId { get; init; }
    public int DeckId { get; set; }
    public string Frontside { get; init; } = default!;
    public string Backside { get; init; } = default!;
}