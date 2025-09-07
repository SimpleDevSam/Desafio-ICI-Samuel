using Desafio_ICI_Samuel.Features.News.ListNews;

namespace Desafio_ICI_Samuel.Features.News.View;

public interface IViewNewsHandler
{
    Task<NewsRow> Handle(int id, CancellationToken ct = default);
}
