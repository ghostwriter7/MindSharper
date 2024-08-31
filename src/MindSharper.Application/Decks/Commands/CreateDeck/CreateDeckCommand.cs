using MediatR;

namespace MindSharper.Application.Decks.Commands.CreateDeck;

public class CreateDeckCommand : IRequest<int>
{
    public string Name { get; set; } = default!;
}