namespace Desafio_ICI_Samuel.Features.News.ListNews;

public record ListNewsVm(int Total, int Page, int PageSize, IReadOnlyList<NewsRow> Rows)
{
    public int TotalPages => Math.Max(1, (int)Math.Ceiling((double)Total / PageSize));
    public record NewsRow(int Id, string Title, int UserId);
}
