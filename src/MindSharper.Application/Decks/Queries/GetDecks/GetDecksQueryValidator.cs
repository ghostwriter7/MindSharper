using FluentValidation;
using MindSharper.Application.Common.PagedQuery;
using MindSharper.Application.Decks.Dtos;

namespace MindSharper.Application.Decks.Queries.GetDecks;

public class GetDecksQueryValidator : AbstractValidator<GetDecksQuery>
{
    public GetDecksQueryValidator()
    {
        Include(new PagedQueryValidator());
    }
}