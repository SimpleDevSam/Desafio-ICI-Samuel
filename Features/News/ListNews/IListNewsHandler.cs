using Desafio_ICI_Samuel.Features.Tags.List;
using Desafio_ICI_Samuel.Features.Tags;

namespace Desafio_ICI_Samuel.Features.News.ListNews;

public interface IListNewsHandler
{
    Task<ListNewsVm> Handle(ListNewsQuery q, CancellationToken ct = default);
}
