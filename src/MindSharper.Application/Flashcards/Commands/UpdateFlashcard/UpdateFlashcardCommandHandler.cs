using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using MindSharper.Domain.Entities;
using MindSharper.Domain.Exceptions;
using MindSharper.Domain.Repositories;

namespace MindSharper.Application.Flashcards.Commands.UpdateFlashcard;

public class UpdateFlashcardCommandHandler(
    ILogger<UpdateFlashcardCommandHandler> logger,
    IFlashcardRepository repository) : IRequestHandler<UpdateFlashcardCommand>
{
    public async Task Handle(UpdateFlashcardCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Updating {nameof(Flashcard)} identified by ID: {request.FlashcardId}");
        var flashcard = await repository.GetFlashcardByIdAsync(request.DeckId, request.FlashcardId)
                        ?? throw new NotFoundException(nameof(Flashcard), request.FlashcardId.ToString());

        flashcard.Frontside = request.Frontside;
        flashcard.Backside = request.Backside;

        await repository.UpdateFlashcardAsync(flashcard);
    }
}