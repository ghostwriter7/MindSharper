using MediatR;

namespace MindSharper.Application.Decks.Commands.CreateDeck;

public class CreateDeckCommand(string name) : IRequest<int>
{
    public string Name { get; } = name;
}