﻿namespace MindSharper.Application.Common;

public class PagedResult<T>
{
    public IEnumerable<T> Results { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages { get; init; }
    public int ItemFrom { get; init; }
    public int ItemTo { get; init; }

    public PagedResult(IEnumerable<T> results, int total, int pageNumber, int pageSize)
    {
        Results = results;
        TotalCount = total;
        TotalPages = (int) Math.Ceiling(total / (double) pageSize);
        ItemFrom = total != 0 ? (pageNumber - 1) * pageSize + 1 : 0;
        ItemTo = total != 0 ? ItemFrom + pageSize - 1 : 0;
    }
}