using MediatR;
using Microsoft.Extensions.Logging;
using MindSharper.Application.Users;
using MindSharper.Domain.Constants;
using MindSharper.Domain.Entities;
using MindSharper.Domain.Exceptions;
using MindSharper.Domain.Interfaces;
using MindSharper.Domain.Repositories;

namespace MindSharper.Application.Flashcards.Commands.DeleteFlashcard;

public class DeleteFlashcardCommandHandler(
    ILogger<DeleteFlashcardCommandHandler> logger,
    IFlashcardRepository flashcardRepository,
    IDeckRepository deckRepository,
    IUserContext userContext,
    IResourceAuthorizationService<Deck> authorizationService) : IRequestHandler<DeleteFlashcardCommand>
{
    public async Task Handle(DeleteFlashcardCommand request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser()!;
        logger.LogWarning("Attempt to delete a Flashcard ({FlashcardId}) in Deck ({DeckId}) by User ({UserId})", request.FlashcardId, request.FlashcardId, currentUser.Id);

        var deck = await deckRepository.GetDeckByIdAsync(request.DeckId)
                   ?? throw new NotFoundException(nameof(Deck), request.DeckId.ToString());

        if (!authorizationService.IsAuthorized(deck, ResourceOperation.Update))
            throw new UnauthorizedException(nameof(Deck), deck.Id, currentUser.Id);
        
        var flashcard = await flashcardRepository.GetFlashcardByIdAsync(request.DeckId, request.FlashcardId)
                        ?? throw new NotFoundException(nameof(Flashcard), request.FlashcardId.ToString());

        await flashcardRepository.DeleteFlashcardAsync(flashcard);
    }
}