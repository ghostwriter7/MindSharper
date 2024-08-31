using MediatR;

namespace MindSharper.Application.Decks.Commands.DeleteDeck;

public class DeleteDeckCommand : IRequest
{
    public int DeckId { get; set; }
}