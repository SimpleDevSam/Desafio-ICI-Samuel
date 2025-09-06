using Desafio_ICI_Samuel.Data;
using Desafio_ICI_Samuel.Features.Tags.List;
using Microsoft.EntityFrameworkCore;
using static Desafio_ICI_Samuel.Features.Tags.List.ListTagsVm;

namespace Desafio_ICI_Samuel.Features.News.ListNews;

public class ListNewsHandler : IListNewsHandler
{
    private readonly AppDbContext _db;
    public ListNewsHandler (AppDbContext db)
    {
        _db = db;
    }
    public async Task<ListNewsVm> Handle (ListNewsQuery q, CancellationToken ct = default)
    {

        var total = await _db.News.CountAsync();

        var rows = await _db.News
            .AsNoTracking()
            .Skip((q.Page - 1) * q.PageSize)
            .Take(q.PageSize)
            .OrderByDescending(n => n.Id)
            .Select(n => new NewsRow(n.Id, n.Title, n.UserId))
            .ToListAsync();

        return new ListNewsVm(total, q.Page, q.PageSize, rows);
    }
}
