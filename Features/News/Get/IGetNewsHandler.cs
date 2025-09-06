namespace Desafio_ICI_Samuel.Features.News.Get;

public interface IGetNewsHandler
{
    Task<Domain.News?> Handle(int id, CancellationToken ct = default);
}
