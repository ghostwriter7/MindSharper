using MediatR;
using MindSharper.Application.Decks.Dtos;

namespace MindSharper.Application.Decks.Queries.GetDeckByIdQuery;

public class GetDeckByIdQuery : IRequest<DeckDto?>
{
    public int DeckId { get; init; }
}