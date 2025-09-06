using Desafio_ICI_Samuel.Data;
using Microsoft.EntityFrameworkCore;

namespace Desafio_ICI_Samuel.Features.Tags.List;

public sealed class ListTagsHandler : IListTagsHandler
{
    private readonly AppDbContext _db;
    public ListTagsHandler(AppDbContext db) => _db = db;

    public async Task<ListTagsVm> Handle(ListTagsQuery q, CancellationToken ct = default)
    {
        var baseQ = _db.Tags.AsNoTracking();

        var total = await baseQ.CountAsync(ct);

        var rows = await baseQ
            .OrderBy(t => t.Id)
            .Skip((q.Page - 1) * q.PageSize)
            .Take(q.PageSize)
            .Select(t => new ListTagsVm.Row(t.Id, t.Nome))
            .AsNoTracking()
            .ToListAsync(ct);

        return new ListTagsVm(total, q.Page, q.PageSize, rows);
    }
}