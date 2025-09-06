namespace Desafio_ICI_Samuel.Features.News.Edit;

public interface IEditNewsHandler
{
    Task Handle(NewsForm vm, CancellationToken ct = default);
}
