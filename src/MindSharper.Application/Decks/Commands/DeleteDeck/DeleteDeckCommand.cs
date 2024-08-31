using MediatR;

namespace MindSharper.Application.Decks.Commands.DeleteDeck;

public class DeleteDeckCommand(int deckId) : IRequest
{
    public int DeckId { get; } = deckId;
}