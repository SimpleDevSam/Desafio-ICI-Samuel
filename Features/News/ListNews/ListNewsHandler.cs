using Desafio_ICI_Samuel.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Desafio_ICI_Samuel.Features.News.ListNews;

public class ListNewsHandler : IListNewsHandler
{
    private readonly IAppDbContext _db;
    public ListNewsHandler (IAppDbContext db)
    {
        _db = db;
    }
    public async Task<ListNewsVm> Handle (ListNewsQuery q, CancellationToken ct = default)
    {

        var total = await _db.News.CountAsync(ct);

        var rows = await _db.News
            .AsNoTracking()
            .Skip((q.Page - 1) * q.PageSize)
            .Take(q.PageSize)
            .OrderByDescending(n => n.Id)
            .Select(n => new NewsRow(n.Id, n.Title, n.UserId))
            .ToListAsync(ct);

        return new ListNewsVm(total, q.Page, q.PageSize, rows);
    }
}
