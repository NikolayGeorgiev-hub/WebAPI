namespace Application.Services.Models;

public class PaginationResponseModel<TModel>
{
    public required IReadOnlyList<TModel> Items { get; set; }

    public required int TotalItems { get; set; }

    public required int PageNumber { get; set; }

    public required int ItemsPerPage { get; set; }

    public int PagesCount { get; set; }
}
