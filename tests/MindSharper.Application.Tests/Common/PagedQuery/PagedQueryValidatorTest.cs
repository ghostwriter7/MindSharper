using FluentValidation.TestHelper;
using JetBrains.Annotations;
using MindSharper.Application.Common.PagedQuery;
using Xunit;

namespace MindSharper.Application.Tests.Common.PagedQuery;

[TestSubject(typeof(PagedQueryValidator))]
public class PagedQueryValidatorTest
{
    private readonly PagedQueryValidator _validator = new PagedQueryValidator();
    
    [Theory]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(15)]
    public void Validator_ForValidPageSizes_ShouldNotHaveAnyValidationErrors(int pageSize)
    {
        var command = new PagedQuery<int[]>() { PageSize = pageSize, PageNumber = 1 };

        var results = _validator.TestValidate(command);
        
        results.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void Validator_ForPageNumberLessThanOne_ShouldHaveValidationError(int pageNumber)
    {
        var command = new PagedQuery<int[]>() { PageSize = 5, PageNumber = pageNumber };

        var results = _validator.TestValidate(command);

        results.ShouldHaveValidationErrorFor(pagedQuery => pagedQuery.PageNumber);
        results.ShouldNotHaveValidationErrorFor(pagedQuery => pagedQuery.PageSize);
    }
    
    
    [Theory]
    [InlineData(3)]
    [InlineData(13)]
    [InlineData(-30)]
    [InlineData(0)]
    public void Validator_ForInvalidPageSizes_ShouldHaveValidationError(int pageSize)
    {
        var command = new PagedQuery<int[]>() { PageSize = pageSize, PageNumber = 1 };

        var results = _validator.TestValidate(command);

        results.ShouldHaveValidationErrorFor(pagedQuery => pagedQuery.PageSize);
        results.ShouldNotHaveValidationErrorFor(pagedQuery => pagedQuery.PageNumber);
    }
}