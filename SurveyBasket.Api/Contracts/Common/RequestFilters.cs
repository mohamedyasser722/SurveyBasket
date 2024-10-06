namespace SurveyBasket.Api.Contracts.Common;

public record RequestFilters
{
    private const int MaxPageSize = 100; // Set a maximum limit for PageSize

    private int _pageSize = 10;

    public int PageNumber { get; init; } = 1;

    // Ensure that PageSize does not exceed MaxPageSize
    public int PageSize
    {
        get => _pageSize;
        init => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
    }
    public string? SearchValue { get; init; }
    public string? SortDirection { get; init; } = "ASC"; // Default value is "asc 
    public string? SortColumn { get; init; }
}
