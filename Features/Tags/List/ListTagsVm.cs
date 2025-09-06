namespace Desafio_ICI_Samuel.Features.Tags.List;

public record ListTagsVm(int Total, int Page, int PageSize, IReadOnlyList<ListTagsVm.Row> Rows)
{
    public int TotalPages => Math.Max(1, (int)Math.Ceiling((double)Total / PageSize));
    public record Row(int Id, string Nome);
}
