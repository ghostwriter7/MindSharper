using FluentValidation;

namespace MindSharper.Application.Decks.Commands.CreateDeck;

public class CreateDeckCommandValidator : AbstractValidator<CreateDeckCommand>
{
    public CreateDeckCommandValidator()
    {
        RuleFor(command => command.Name)
            .Length(2, 20);
    }
}