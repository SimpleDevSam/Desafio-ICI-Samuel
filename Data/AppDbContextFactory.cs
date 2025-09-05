using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Desafio_ICI_Samuel.Data;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "ici.db");
        Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);

        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite($"Data Source={dbPath};Cache=Shared")
            .Options;

        return new AppDbContext(opts);
    }
}
