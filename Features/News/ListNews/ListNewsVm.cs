namespace Desafio_ICI_Samuel.Features.News.ListNews;

public sealed record ListNewsVm(int Total, int Page, int PageSize, IReadOnlyList<NewsRow> Rows)
{
    public int TotalPages => Math.Max(1, (int)Math.Ceiling((double)Total / PageSize));
    public sealed record NewsRow(int Id, string Title, int UserId);
}
