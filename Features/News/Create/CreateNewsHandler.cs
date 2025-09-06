using Desafio_ICI_Samuel.Data;
using Desafio_ICI_Samuel.Domain;

namespace Desafio_ICI_Samuel.Features;

public class CreateNewsHandler
{
    private readonly AppDbContext _db;
    public CreateNewsHandler(AppDbContext db) => _db = db;

    public async Task<int> Handle(NewsForm vm, CancellationToken ct = default)
    {
        var n = new News
        {
            Title = vm.Title.Trim(),
            Text = vm.Text.Trim(),
            UserId = vm.UserId!.Value
        };

        _db.News.Add(n);
        await _db.SaveChangesAsync(ct);

        _db.NewsTags.AddRange(vm.TagIds.Select(tid => new NewsTag { NewsId = n.Id, TagId = tid }));
        await _db.SaveChangesAsync(ct);

        return n.Id;
    }
}