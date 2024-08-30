using FluentValidation;
using MindSharper.Application.Decks.Dtos;
using MindSharper.Domain.Entities;

namespace MindSharper.Application.Decks.Validators;

public class CreateDeckDtoValidator : AbstractValidator<CreateDeckDto>
{
    public CreateDeckDtoValidator()
    {
        RuleFor(createDeckDto => createDeckDto.Name)
            .MaximumLength(20);
    }
}