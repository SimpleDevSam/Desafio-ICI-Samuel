using Desafio_ICI_Samuel.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using static Desafio_ICI_Samuel.Features.Tags.List.ListTagsVm;

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
            .OrderByDescending(n => n.Id)
            .Skip((q.Page - 1) * q.PageSize)
            .Take(q.PageSize)
            .Select(n => new NewsRow(
                n.Id,
                n.Title,
                n.User.Name,
                string.Join(",", n.NewsTags.Select(nt => nt.Tag.Name)),
                n.Text
            ))
            .ToListAsync(ct);

        return new ListNewsVm(total, q.Page, q.PageSize, rows);
    }
}
