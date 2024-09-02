using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using MindSharper.Application.Decks.Dtos;
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
    : IRequestHandler<GetDecksQuery, IEnumerable<MinimalDeckDto>>
{
    public async Task<IEnumerable<MinimalDeckDto>> Handle(GetDecksQuery request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser()!;
        logger.LogInformation("Retrieving all {Resource} for User: {UserId}", nameof(Deck), currentUser.Id);
        var decks = await repository.GetDecksByUserIdAsync(currentUser.Id);
        var deckDtos = mapper.Map<IEnumerable<MinimalDeckDto>>(decks);
        return deckDtos;
    }
}