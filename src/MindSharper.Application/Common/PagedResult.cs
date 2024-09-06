namespace MindSharper.Application.Common;

public class PagedResult<T>
{
    public List<T> Results { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int ItemFrom { get; init; }
    public int ItemTo { get; init; }

    public PagedResult(List<T> results, int pageNumber, int pageSize)
    {
        Results = results;
        PageSize = pageSize;
        PageNumber = pageNumber;
        ItemFrom = results.Count != 0 ? (pageNumber - 1) * pageSize + 1 : 0;
        ItemTo = results.Count != 0 ? ItemFrom + pageSize - 1 : 0;
    }
}