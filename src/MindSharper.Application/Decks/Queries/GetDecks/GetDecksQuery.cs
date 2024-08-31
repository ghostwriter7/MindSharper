using MediatR;
using MindSharper.Application.Decks.Dtos;

namespace MindSharper.Application.Decks.Queries.GetDecks;

public class GetDecksQuery : IRequest<IEnumerable<MinimalDeckDto>>
{
    
}