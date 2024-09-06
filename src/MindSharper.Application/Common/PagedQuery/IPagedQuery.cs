namespace MindSharper.Application.Common.PagedQuery;

public interface IPagedQuery
{
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
}