using Desafio_ICI_Samuel.Data;
using Desafio_ICI_Samuel.Features.News.Edit;
using Microsoft.EntityFrameworkCore;

namespace Desafio_ICI_Samuel.Features.News.Delete;

public class DeleteNewsHandler : IDeleteNewsHandler
{
    private readonly AppDbContext _db;
    public DeleteNewsHandler(AppDbContext db)
    {
        _db = db;
    }
    public async Task Handle(int id, CancellationToken ct = default)
    {
        var news = await _db.News.FirstOrDefaultAsync(x => x.Id == id);

        if (news is null)
        {
            throw new Exception("Error when deleting news, News was not found");
        }

        _db.News.Remove(news);
        await _db.SaveChangesAsync();
    }
}
