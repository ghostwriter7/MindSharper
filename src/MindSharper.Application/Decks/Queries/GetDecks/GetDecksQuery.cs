using MediatR;
using MindSharper.Application.Common;
using MindSharper.Application.Common.PagedQuery;
using MindSharper.Application.Decks.Dtos;

namespace MindSharper.Application.Decks.Queries.GetDecks;

public class GetDecksQuery : PagedQuery<PagedResult<MinimalDeckDto>>
{
    
}