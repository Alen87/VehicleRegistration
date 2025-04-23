namespace Project.Common.Paging;


public class PagedResult<T>
{
    /// <summary>
    /// Trenutna stranica
    /// </summary>
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public List<T> Data { get; set; } = new List<T>();

    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;

    public PagedResult()
    {
    }

    public PagedResult(List<T> data, int totalCount, int pageNumber, int pageSize)
    {
        Data = data;
        TotalCount = totalCount;
        CurrentPage = pageNumber;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
    }
}