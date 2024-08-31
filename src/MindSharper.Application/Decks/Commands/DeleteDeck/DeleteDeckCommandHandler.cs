using MediatR;
using Microsoft.Extensions.Logging;
using MindSharper.Domain.Entities;
using MindSharper.Domain.Exceptions;
using MindSharper.Domain.Repositories;

namespace MindSharper.Application.Decks.Commands.DeleteDeck;

public class DeleteDeckCommandHandler(ILogger<DeleteDeckCommandHandler> logger, IDeckRepository repository) : IRequestHandler<DeleteDeckCommand>
{
    public async Task Handle(DeleteDeckCommand request, CancellationToken cancellationToken)
    {
        var deckId = request.DeckId;
        logger.LogWarning($"Attempt to delete {nameof(Deck)} with ID: {deckId}");
        var deck = await repository.GetDeckByIdAsync(deckId)
                   ?? throw new NotFoundException(nameof(Deck), deckId.ToString());

        await repository.DeleteDeckAsync(deck);
        logger.LogWarning($"{nameof(Deck)} ({deckId}) has been successfully deleted");    }
}