using FluentAssertions;
using JetBrains.Annotations;
using MindSharper.Application.Common.PagedQuery;
using MindSharper.Application.Helpers;
using Xunit;

namespace MindSharper.Application.Tests.Helpers;

[TestSubject(typeof(PagingHelper))]
public class PagingHelperTest
{

    [Fact]
    public void GetPagedResult_ForValidInvocation_ShouldReturnPagedResultInstance()
    {
        var pagedResult =
            PagingHelper.GetPagedResult([1, 2, 3], 3, new PagedQuery<int[]>() { PageSize = 5, PageNumber = 1 });

        pagedResult.Results.Should().BeEquivalentTo([1, 2, 3]);
        pagedResult.TotalCount.Should().Be(3);
        pagedResult.TotalPages.Should().Be(1);
        pagedResult.ItemFrom.Should().Be(1);
        pagedResult.ItemTo.Should().Be(5);
    }
}