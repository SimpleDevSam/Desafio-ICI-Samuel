using Desafio_ICI_Samuel.Features.Tags.List;
using Desafio_ICI_Samuel.Features.Tags;

namespace Desafio_ICI_Samuel.Features.News.ListNews;

public interface IListNewsHandler
{
    Task<ICollection<NewsRow>> Handle(ListNewsQuery q, CancellationToken ct = default);
}
