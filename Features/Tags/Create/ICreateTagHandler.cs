namespace Desafio_ICI_Samuel.Features.Tags.Create;

public interface ICreateTagHandler
{
    Task<int> Handle(CreateTagForm vm, CancellationToken ct = default);
}
