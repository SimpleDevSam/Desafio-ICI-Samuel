namespace Desafio_ICI_Samuel.Features.Tags.List;

public interface IListTagsHandler
{
    Task<ListTagsVm> Handle(ListTagsQuery q, CancellationToken ct = default);
}
