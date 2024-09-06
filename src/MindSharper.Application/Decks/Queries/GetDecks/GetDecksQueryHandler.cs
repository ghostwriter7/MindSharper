using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using MindSharper.Application.Common;
using MindSharper.Application.Decks.Dtos;
using MindSharper.Application.Helpers;
using MindSharper.Application.Users;
using MindSharper.Domain.Entities;
using MindSharper.Domain.Interfaces;
using MindSharper.Domain.Repositories;

namespace MindSharper.Application.Decks.Queries.GetDecks;

public class GetDecksQueryHandler(
    ILogger<GetDecksQueryHandler> logger,
    IDeckRepository repository,
    IMapper mapper,
    IUserContext userContext)
    : IRequestHandler<GetDecksQuery, PagedResult<MinimalDeckDto>>
{
    public async Task<PagedResult<MinimalDeckDto>> Handle(GetDecksQuery request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser()!;
        logger.LogInformation("Retrieving all {Resource} for User: {UserId}", nameof(Deck), currentUser.Id);
        var (decks, total) = await repository.GetDecksByUserIdAsync(currentUser.Id, request.PageNumber, request.PageSize);
        var deckDtos = mapper.Map<IEnumerable<MinimalDeckDto>>(decks);
        var pagedResult = PagingHelper.GetPagedResult(deckDtos, total, request);
        return pagedResult;
    }
}