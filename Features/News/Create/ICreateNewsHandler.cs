namespace Desafio_ICI_Samuel.Features
{
    public interface ICreateNewsHandler
    {
        Task<int> Handle(NewsForm vm, CancellationToken ct = default);
    }
}
