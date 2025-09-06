namespace Desafio_ICI_Samuel.Features.News.ListNews;

public sealed record ListNewsQuery(int Page = 1, int PageSize = 5, string? Search = null);