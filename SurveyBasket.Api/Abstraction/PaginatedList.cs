namespace SurveyBasket.Api.Abstraction;

public class PaginatedList<T>
{
    public int PageNumber { get; private set; }
    public int PageSize { get; private set; } 
    public int TotalPages { get; private set; }
    public int TotalCount { get; private set; }
    public bool HasPreviousPage => (PageNumber > 1);
    public bool HasNextPage => (PageNumber < TotalPages);
    public List<T> Items { get; private set; }

    private PaginatedList(List<T> items, int count, int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        PageSize = pageSize;
        TotalCount = count;
        Items = items;
    }

    public static async Task<PaginatedList<T>> Create(IQueryable<T> source, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var count = await source.CountAsync(cancellationToken);
        var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        return new PaginatedList<T>(items, count, pageNumber, pageSize);
    }
}
