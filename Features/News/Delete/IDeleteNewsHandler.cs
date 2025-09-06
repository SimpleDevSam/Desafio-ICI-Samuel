namespace Desafio_ICI_Samuel.Features.News.Delete;

public interface IDeleteNewsHandler
{
    Task Handle(int id, CancellationToken ct = default);
}
