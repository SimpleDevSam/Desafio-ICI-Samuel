using Desafio_ICI_Samuel.Data;
using Microsoft.EntityFrameworkCore;

namespace Desafio_ICI_Samuel.Features.Tags.Edit;

public sealed class EditTagHandler : IEditTagHandler
{
    private readonly AppDbContext _db;
    public EditTagHandler(AppDbContext db) => _db = db;

    public async Task Handle(EditTagForm vm, CancellationToken ct = default)
    {
        var tag = await _db.Tags.FirstOrDefaultAsync(t => t.Id == vm.Id, ct) ?? throw new KeyNotFoundException("Tag não encontrada.");

        var novoNome = (vm.Name ?? "").Trim();

        var duplicada = await _db.Tags.AnyAsync(t => t.Id != vm.Id && EF.Functions.Collate(t.Nome, "NOCASE") == novoNome, ct);

        if (duplicada)
        {
            throw new InvalidOperationException("Já existe uma tag com esse nome.");
        }
           
        tag.Nome = novoNome;

        await _db.SaveChangesAsync(ct);
    }
}