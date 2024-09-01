using FluentValidation;

namespace MindSharper.Application.Flashcards.Commands.CreateFlashcard;

public class CreateFlashcardCommandValidator : AbstractValidator<CreateFlashcardCommand>
{
    public CreateFlashcardCommandValidator()
    {
        RuleFor(command => command.Frontside)
            .Length(3, 100);

        RuleFor(command => command.Backside)
            .Length(3, 200);
    }
}