using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using MindSharper.Application.Decks.Dtos;
using MindSharper.Domain.Entities;
using MindSharper.Domain.Repositories;

namespace MindSharper.Application.Decks.Queries.GetDecks;

public class GetDecksQueryHandler(ILogger<GetDecksQueryHandler> logger, IDeckRepository repository, IMapper mapper)
    : IRequestHandler<GetDecksQuery, IEnumerable<MinimalDeckDto>>
{
    public async Task<IEnumerable<MinimalDeckDto>> Handle(GetDecksQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Retrieving all {nameof(Deck)}");
        var decks = await repository.GetDecksAsync();
        var deckDtos = mapper.Map<IEnumerable<MinimalDeckDto>>(decks);
        return deckDtos;
    }
}