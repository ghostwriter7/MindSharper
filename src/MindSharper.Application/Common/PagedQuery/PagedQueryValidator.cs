using FluentValidation;

namespace MindSharper.Application.Common.PagedQuery;

public class PagedQueryValidator : AbstractValidator<IPagedQuery>
{
    private readonly int[] _validPageSizes = [5, 10, 15];
    
    public PagedQueryValidator()
    {
            RuleFor(pagedQuery => pagedQuery.PageNumber)
            .GreaterThanOrEqualTo(1);

        RuleFor(pagedQuery => pagedQuery.PageSize)
            .Must(_validPageSizes.Contains)
            .WithMessage(pagedQuery => $"Page size must be either 5, 10, or 15. Provided: {pagedQuery.PageSize}");
    }
}