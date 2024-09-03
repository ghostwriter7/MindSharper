using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using MindSharper.Application.Users;
using MindSharper.Domain.Constants;
using MindSharper.Domain.Entities;
using MindSharper.Domain.Exceptions;
using MindSharper.Domain.Interfaces;
using MindSharper.Domain.Repositories;

namespace MindSharper.Application.Flashcards.Commands.UpdateFlashcard;

public class UpdateFlashcardCommandHandler(
    ILogger<UpdateFlashcardCommandHandler> logger,
    IFlashcardRepository repository,
    IDeckRepository deckRepository,
    IUserContext userContext,
    IResourceAuthorizationService<Deck> authorizationService) : IRequestHandler<UpdateFlashcardCommand>
{
    public async Task Handle(UpdateFlashcardCommand request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser()!;
        logger.LogInformation("Attempt to update a Flashcard ({FlashcardId}) in Deck ({DeckId}) by User ({UserId})",
            request.FlashcardId, request.DeckId, currentUser.Id);

        var deck = await deckRepository.GetDeckByIdAsync(request.DeckId)
                   ?? throw new NotFoundException(nameof(Deck), request.DeckId.ToString());

        if (!authorizationService.IsAuthorized(deck, ResourceOperation.Update))
            throw new UnauthorizedException(nameof(Deck), request.DeckId, currentUser.Id);
        
        var flashcard = await repository.GetFlashcardByIdAsync(request.DeckId, request.FlashcardId)
                        ?? throw new NotFoundException(nameof(Flashcard), request.FlashcardId.ToString());
        
        flashcard.Frontside = request.Frontside;
        flashcard.Backside = request.Backside;

        await repository.UpdateFlashcardAsync(flashcard);
    }
}