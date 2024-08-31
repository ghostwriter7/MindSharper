using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using MindSharper.Application.Helpers;
using MindSharper.Domain.Entities;
using MindSharper.Domain.Exceptions;
using MindSharper.Domain.Repositories;

namespace MindSharper.Application.Decks.Commands.CreateDeck;

public class CreateDeckCommandHandler(ILogger<CreateDeckCommandHandler> logger, IMapper mapper, IDeckRepository repository) : IRequestHandler<CreateDeckCommand, int>
{
    public async Task<int> Handle(CreateDeckCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Creating a {nameof(Deck)}");
        var deck = mapper.Map<Deck>(request);
        deck.CreatedAt = DateOnly.FromDateTime(DateTime.Now);

        try
        {
            var deckId = await repository.CreateDeckAsync(deck);
            return deckId;
        }
        catch (Exception exception) when (ExceptionHelper.IsUniqueConstraintViolationException(exception))
        {
            throw new DuplicateResourceException(nameof(Deck), nameof(Deck.Name), deck.Name);
        }
    }
}