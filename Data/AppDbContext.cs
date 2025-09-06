using Desafio_ICI_Samuel.Domain;
using Desafio_ICI_Samuel.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;


namespace Desafio_ICI_Samuel.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<News> News => Set<News>();
    public DbSet<NewsTag> NewsTags => Set<NewsTag>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
