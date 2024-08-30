using MediatR;
using MindSharper.Application.Decks.Dtos;

namespace MindSharper.Application.Decks.Queries.GetDecksQuery;

public class GetDecksQuery : IRequest<DeckDto?>
{
    public int DeckId { get; init; }
}