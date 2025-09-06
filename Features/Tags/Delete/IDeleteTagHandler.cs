namespace Desafio_ICI_Samuel.Features.Tags.Delete;

public interface IDeleteTagHandler
{
    Task Handle(int id, CancellationToken ct = default);
}
