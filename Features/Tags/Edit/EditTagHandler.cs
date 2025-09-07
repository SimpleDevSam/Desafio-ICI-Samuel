using System.Xml.Linq;
using Desafio_ICI_Samuel.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Desafio_ICI_Samuel.Features.Tags.Edit;

public class EditTagHandler : IEditTagHandler
{
    private readonly IAppDbContext _db;
    public EditTagHandler(IAppDbContext db) => _db = db;

    public async Task Handle(EditTagForm vm, CancellationToken ct = default)
    {
        var tag = await _db.Tags.FirstOrDefaultAsync(t => t.Id == vm.Id, ct) ?? throw new KeyNotFoundException("Tag não encontrada.");

        var newName = (vm.Name ?? "").Trim();

        var duplicada = await _db.Tags.AnyAsync(t => t.Id != vm.Id && t.Name.Equals(newName, StringComparison.OrdinalIgnoreCase), ct);

        if (duplicada)
        {
            throw new InvalidOperationException("Já existe uma tag com esse nome.");
        }
           
        tag.Name = newName;

        await _db.SaveChangesAsync(ct);
    }
}
