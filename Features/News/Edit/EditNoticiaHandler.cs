using Desafio_ICI_Samuel.Data;
using Desafio_ICI_Samuel.Domain;
using Microsoft.EntityFrameworkCore;

namespace Desafio_ICI_Samuel.Features;

public class EditNewsHandler
{
    private readonly AppDbContext _db;
    public EditNewsHandler(AppDbContext db) => _db = db;

    public async Task Handle(NewsForm vm, CancellationToken ct = default)
    {
        var n = await _db.News .Include(x => x.NewsTags).FirstOrDefaultAsync(x => x.Id == vm.Id!.Value, ct)
            ?? throw new KeyNotFoundException("News not found.");

        var userExists = await _db.Users.AnyAsync(u => u.Id == vm.UserId, ct);

        if (!userExists)
        {
            throw new KeyNotFoundException("Author not found.");
        }

        n.Title = vm.Title.Trim();
        n.Text = vm.Text.Trim();
        n.UserId = vm.UserId!.Value;

        _db.NewsTags.RemoveRange(n.NewsTags);
        _db.NewsTags.AddRange(vm.TagIds.Select(tid => new NewsTag { NewsId = n.Id, TagId = tid }));

        await _db.SaveChangesAsync(ct);
    }
}

