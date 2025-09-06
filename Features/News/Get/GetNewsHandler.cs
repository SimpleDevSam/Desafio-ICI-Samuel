using Desafio_ICI_Samuel.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Desafio_ICI_Samuel.Features.News.Get;

public class GetNewsHandler : IGetNewsHandler
{
    private readonly IAppDbContext _db;
    public GetNewsHandler (IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Domain.News?> Handle(int id, CancellationToken ct = default)
    {
        var news = await _db.News
            .Include(x => x.NewsTags)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (news is null)
        {
            return null;
        }

        return news;
    }
}
