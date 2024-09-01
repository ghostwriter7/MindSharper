using FluentValidation;

namespace MindSharper.Application.Flashcards.Commands.UpdateFlashcard;

public class UpdateFlashcardCommandValidator : AbstractValidator<UpdateFlashcardCommand>
{
    public UpdateFlashcardCommandValidator()
    {
        RuleFor(command => command.Frontside)
            .Length(3, 100);

        RuleFor(command => command.Backside)
            .Length(3, 200);
    }
}