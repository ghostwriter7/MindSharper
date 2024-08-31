using FluentValidation;

namespace MindSharper.Application.Decks.Commands.CreateDeck;

public class CreateDeckCommandValidator : AbstractValidator<CreateDeckCommand>
{
    public CreateDeckCommandValidator()
    {
        RuleFor(command => command.Name)
            .MinimumLength(2)
            .MaximumLength(20);
    }
}