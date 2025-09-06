using Desafio_ICI_Samuel.Domain;
using Desafio_ICI_Samuel.Models;
using Microsoft.EntityFrameworkCore;

namespace Desafio_ICI_Samuel.Data.Interfaces;

public interface IAppDbContext
{
    DbSet<Tag> Tags { get; }
    DbSet<News> News { get; }
    DbSet<NewsTag> NewsTags { get; }
    DbSet<User> Users { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
