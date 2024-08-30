using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using MindSharper.Application.Decks.Dtos;
using MindSharper.Domain.Entities;
using MindSharper.Domain.Exceptions;
using MindSharper.Domain.Repositories;

namespace MindSharper.Application.Decks.Queries.GetDecksQuery;

public class GetDecksQueryHandler(ILogger<GetDecksQueryHandler> logger, IDeckRepository deckRepository, IMapper mapper) : IRequestHandler<GetDecksQuery, DeckDto>
{
    public async Task<DeckDto> Handle(GetDecksQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Retrieving {nameof(Deck)} by ID: {request.DeckId}");
        var deck = await deckRepository.GetDeckByIdAsync(request.DeckId)
                   ?? throw new NotFoundException(nameof(Deck), request.DeckId.ToString());
        var deckDto = mapper.Map<DeckDto>(deck);
        return deckDto;
    }
}