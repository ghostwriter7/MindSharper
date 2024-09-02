using MediatR;

namespace MindSharper.Application.Flashcards.Commands.CreateFlashcard;

public class CreateFlashcardCommand : IRequest<int>
{
    public int DeckId { get; set; }
    public string Frontside { get; init; } = default!;
    public string Backside { get; init; } = default!;
}