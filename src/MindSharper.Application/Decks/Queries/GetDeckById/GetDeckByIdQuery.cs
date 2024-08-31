using MediatR;
using MindSharper.Application.Decks.Dtos;

namespace MindSharper.Application.Decks.Queries.GetDeckByIdQuery;

public class GetDeckByIdQuery(int deckId) : IRequest<DeckDto?>
{
    public int DeckId { get; } = deckId;
}