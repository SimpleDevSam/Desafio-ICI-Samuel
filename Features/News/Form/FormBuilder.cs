using Desafio_ICI_Samuel.Data.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Desafio_ICI_Samuel.Features.News.Form;

public class FormBuilder : IFormBuilder
{
    private readonly IAppDbContext _db;

    public FormBuilder (IAppDbContext db)
    {
        _db = db;
    }

    public async Task<NewsForm> Build(Domain.News? n = null)
    {
        var users = await _db.Users
            .OrderBy(u => u.Name)
            .Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.Name })
            .AsNoTracking()
            .ToListAsync();

        var tags = await _db.Tags
            .OrderBy(t => t.Name)
            .Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.Name })
            .AsNoTracking()
            .ToListAsync();

        return new NewsForm
        {
            Id = n?.Id,
            Title = n?.Title ?? "",
            Text = n?.Text ?? "",
            UserId = n?.UserId,
            TagIds = n?.NewsTags.Select(nt => nt.TagId).ToArray() ?? Array.Empty<int>(),
            AvailableUsers = users,
            AvailableTags = tags
        };
    }
}
