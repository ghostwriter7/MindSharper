using MediatR;
using Microsoft.Extensions.Logging;
using MindSharper.Domain.Entities;
using MindSharper.Domain.Exceptions;
using MindSharper.Domain.Repositories;

namespace MindSharper.Application.Decks.Commands.UpdateDeckName;

public class UpdateDeckNameCommandHandler(ILogger<UpdateDeckNameCommandHandler> logger, IDeckRepository repository) : IRequestHandler<UpdateDeckNameCommand>
{
    public async Task Handle(UpdateDeckNameCommand request, CancellationToken cancellationToken)
    {
        var (deckId, name) = request;
        logger.LogInformation($"Updating {nameof(Deck)} ({deckId}) name");
        var deck = await repository.GetDeckByIdAsync(deckId)
                   ?? throw new NotFoundException(nameof(Deck), deckId.ToString());

        deck.Name = name;
        await repository.UpdateDeckAsync(deck);    }
}