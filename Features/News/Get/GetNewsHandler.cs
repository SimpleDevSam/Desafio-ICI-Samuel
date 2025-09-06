using Desafio_ICI_Samuel.Data;
using Microsoft.EntityFrameworkCore;

namespace Desafio_ICI_Samuel.Features.News.Get;

public class GetNewsHandler : IGetNewsHandler
{
    private readonly AppDbContext _db;
    public GetNewsHandler (AppDbContext db)
    {
        _db = db;
    }

    public async Task<Domain.News?> Handle(int id, CancellationToken ct = default)
    {
        var news = await _db.News
            .Include(x => x.NewsTags)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (news is null)
        {
            return null;
        }

        return news;
    }
}
