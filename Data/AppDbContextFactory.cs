using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Desafio_ICI_Samuel.Data;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var contentRoot = Directory.GetCurrentDirectory();
        var conectionString = SqlitePath.FromAppSettings(contentRoot).BuildConnectionString();

        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(conectionString)
            .Options;

        return new AppDbContext(opts);
    }
}
