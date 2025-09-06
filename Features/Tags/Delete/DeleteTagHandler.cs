using Desafio_ICI_Samuel.Data;
using Microsoft.EntityFrameworkCore;

namespace Desafio_ICI_Samuel.Features.Tags.Delete;

public sealed class DeleteTagHandler : IDeleteTagHandler
{
    private readonly AppDbContext _db;
    public DeleteTagHandler(AppDbContext db) => _db = db;

    public async Task Handle(int id, CancellationToken ct = default)
    {
        var tag = await _db.Tags.FirstOrDefaultAsync(t => t.Id == id, ct) ?? throw new KeyNotFoundException("Tag not found.");

        //var inUse = await _db.NoticiasTags.AnyAsync(nt => nt.TagId == id, ct);
        //if (inUse)
        //{
        //    throw new InvalidOperationException("Cannot delete this tag because it is referenced by one or more news items.");
        //}
            
        _db.Tags.Remove(tag);
        await _db.SaveChangesAsync(ct);
    }
}