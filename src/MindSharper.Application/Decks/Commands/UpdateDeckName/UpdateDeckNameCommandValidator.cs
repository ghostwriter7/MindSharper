using FluentValidation;

namespace MindSharper.Application.Decks.Commands.UpdateDeckName;

public class UpdateDeckNameCommandValidator : AbstractValidator<UpdateDeckNameCommand>
{
    public UpdateDeckNameCommandValidator()
    {
        RuleFor(command => command.Name)
            .Length(2, 20);
    }
}