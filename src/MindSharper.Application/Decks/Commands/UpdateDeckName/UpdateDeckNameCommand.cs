using MediatR;

namespace MindSharper.Application.Decks.Commands.UpdateDeckName;

public class UpdateDeckNameCommand(int deckId, string name) : IRequest
{
    public int DeckId { get; } = deckId;
    public string Name { get; } = name;

    public void Deconstruct(out int deckId, out string name)
    {
        deckId = DeckId;
        name = Name;
    }
}