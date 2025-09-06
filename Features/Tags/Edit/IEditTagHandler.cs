namespace Desafio_ICI_Samuel.Features.Tags.Edit;

public interface IEditTagHandler
{
    Task Handle(EditTagForm vm, CancellationToken ct = default);
}
