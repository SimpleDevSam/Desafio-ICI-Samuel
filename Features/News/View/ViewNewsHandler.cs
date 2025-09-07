using Desafio_ICI_Samuel.Data.Interfaces;
using Desafio_ICI_Samuel.Features.News.View;
using Microsoft.EntityFrameworkCore;

namespace Desafio_ICI_Samuel.Features.News.ListNews;

public class ViewNewsHandler : IViewNewsHandler
{
    private readonly IAppDbContext _db;
    public ViewNewsHandler(IAppDbContext db)
    {
        _db = db;
    }
    public async Task<NewsRow?> Handle(int id, CancellationToken ct = default)
    {
        return await _db.News
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(n => new NewsRow(
                n.Id,
                n.Title,
                n.User.Name,
                string.Join(",", n.NewsTags.Select(nt => nt.Tag.Name)),
                n.Text
            ))
            .FirstOrDefaultAsync(ct);
    }
}

