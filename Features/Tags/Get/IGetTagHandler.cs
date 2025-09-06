namespace Desafio_ICI_Samuel.Features.Tags.Get;

public interface IGetTagHandler
{
    Task<GetTagVm> Handle(int id, CancellationToken ct = default);
}
