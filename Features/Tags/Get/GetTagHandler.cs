using Desafio_ICI_Samuel.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Desafio_ICI_Samuel.Features.Tags.Get;
public class GetTagHandler : IGetTagHandler
{
    private readonly IAppDbContext _db;
    public GetTagHandler(IAppDbContext db) => _db = db;

    public async Task<GetTagVm> Handle(int id, CancellationToken ct = default)
    {
        var vm = await _db.Tags
            .Where(t => t.Id == id)
            .Select(t => new GetTagVm { Id=t.Id, Name = t.Nome })
            .AsNoTracking()
            .FirstOrDefaultAsync(ct);

        if (vm is null)
        {
            throw new KeyNotFoundException("Tag not found.");
        }

        return vm;
    }
}

