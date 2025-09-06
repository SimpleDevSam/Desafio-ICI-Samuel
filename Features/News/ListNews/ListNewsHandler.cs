using Desafio_ICI_Samuel.Data;
using Microsoft.EntityFrameworkCore;

namespace Desafio_ICI_Samuel.Features.News.ListNews;

public class ListNewsHandler : IListNewsHandler
{
    private readonly AppDbContext _db;
    public ListNewsHandler (AppDbContext db)
    {
        _db = db;
    }
    public async Task<ICollection<NewsRow>> Handle (ListNewsQuery q, CancellationToken ct = default)
    {
        return await _db.News
            .AsNoTracking()
            .OrderByDescending(n => n.Id)
            .Select(n => new NewsRow(n.Id, n.Title, n.UserId))
            .ToListAsync();
    }
}
