using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using MindSharper.Application.Decks.Dtos;
using MindSharper.Application.Users;
using MindSharper.Domain.Constants;
using MindSharper.Domain.Entities;
using MindSharper.Domain.Exceptions;
using MindSharper.Domain.Interfaces;
using MindSharper.Domain.Repositories;

namespace MindSharper.Application.Decks.Queries.GetDeckByIdQuery;

public class GetDeckByIdQueryHandler(
    ILogger<GetDeckByIdQueryHandler> logger,
    IDeckRepository deckRepository,
    IMapper mapper,
    IUserContext userContext,
    IResourceAuthorizationService<Deck> authorizationService) : IRequestHandler<GetDeckByIdQuery, DeckDto?>
{
    public async Task<DeckDto> Handle(GetDeckByIdQuery request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser()!;
        logger.LogInformation("Retrieving {resource} ID: {deckId} by User: {userId}", nameof(Deck), request.DeckId, currentUser.Id);
        
        var deck = await deckRepository.GetDeckByIdAsync(request.DeckId)
                   ?? throw new NotFoundException(nameof(Deck), request.DeckId.ToString());

        if (!authorizationService.IsAuthorized(deck, ResourceOperation.Read))
            throw new UnauthorizedException(nameof(Deck), deck.Id, currentUser.Id);
        
        var deckDto = mapper.Map<DeckDto>(deck);
        return deckDto;
    }
}