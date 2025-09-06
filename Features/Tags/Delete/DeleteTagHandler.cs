using Desafio_ICI_Samuel.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Desafio_ICI_Samuel.Features.Tags.Delete;

public sealed class DeleteTagHandler : IDeleteTagHandler
{
    private readonly IAppDbContext _db;
    public DeleteTagHandler(IAppDbContext db) => _db = db;

    public async Task Handle(int id, CancellationToken ct = default)
    {
        var tag = await _db.Tags.FirstOrDefaultAsync(t => t.Id == id, ct) ?? throw new KeyNotFoundException("Tag não encontrada.");

        var isUsedByNews = await _db.NewsTags.AnyAsync(nt => nt.TagId == id, ct);

        if (isUsedByNews)
        {
            throw new InvalidOperationException("Não foi possível deletar a tag porque ela está sendo utilizada por uma notícia");
        }

        _db.Tags.Remove(tag);
        await _db.SaveChangesAsync(ct);
    }
}
