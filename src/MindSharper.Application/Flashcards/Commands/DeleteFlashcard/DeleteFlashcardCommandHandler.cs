using MediatR;
using Microsoft.Extensions.Logging;
using MindSharper.Domain.Entities;
using MindSharper.Domain.Exceptions;
using MindSharper.Domain.Repositories;

namespace MindSharper.Application.Flashcards.Commands.DeleteFlashcard;

public class DeleteFlashcardCommandHandler(
    ILogger<DeleteFlashcardCommandHandler> logger,
    IFlashcardRepository flashcardRepository) : IRequestHandler<DeleteFlashcardCommand>
{
    public async Task Handle(DeleteFlashcardCommand request, CancellationToken cancellationToken)
    {
        logger.LogWarning($"Deleting a {nameof(Flashcard)} identified by ID: {request.FlashcardId}");
        var flashcard = await flashcardRepository.GetFlashcardByIdAsync(request.DeckId, request.FlashcardId)
                        ?? throw new NotFoundException(nameof(Flashcard), request.FlashcardId.ToString());

        await flashcardRepository.DeleteFlashcardAsync(flashcard);
    }
}