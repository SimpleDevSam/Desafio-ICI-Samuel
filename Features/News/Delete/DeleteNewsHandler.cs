using Desafio_ICI_Samuel.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Desafio_ICI_Samuel.Features.News.Delete;

public class DeleteNewsHandler : IDeleteNewsHandler
{
    private readonly IAppDbContext _db;
    public DeleteNewsHandler(IAppDbContext db)
    {
        _db = db;
    }
    public async Task Handle(int id, CancellationToken ct = default)
    {
        var news = await _db.News.FirstOrDefaultAsync(x => x.Id == id, ct);

        if (news is null)
        {
            throw new Exception("Error when deleting news, News was not found");
        }

        _db.News.Remove(news);
        await _db.SaveChangesAsync(ct);
    }
}
