using Desafio_ICI_Samuel.Data.Interfaces;
using Desafio_ICI_Samuel.Domain;
using Desafio_ICI_Samuel.Models;
using Microsoft.EntityFrameworkCore;

namespace Desafio_ICI_Samuel.Features.Tags.Create;

public sealed class CreateTagHandler : ICreateTagHandler
{
    private readonly IAppDbContext _db;
    public CreateTagHandler(IAppDbContext db) => _db = db;

    public async Task<int> Handle(CreateTagForm vm, CancellationToken ct = default)
    {
        var nome = (vm.Nome ?? string.Empty).Trim();

        var exists = await _db.Tags.AnyAsync(t => EF.Functions.Collate(t.Name, "NOCASE") == nome, ct);

        if (exists)
        {
            throw new InvalidOperationException("Já existe uma tag com esse nome.");
        }
            
        var tag = new Tag { Name = nome };
        _db.Tags.Add(tag);

        await _db.SaveChangesAsync(ct);

        return tag.Id;
    }
}
