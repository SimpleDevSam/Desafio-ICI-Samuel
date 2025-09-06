using System.IO;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace Desafio_ICI_Samuel.Data;

public class SqlitePath
{
    private readonly string _contentRoot;
    private readonly IConfiguration _config;

    public SqlitePath(IConfiguration config, string contentRoot)
    {
        _config = config;
        _contentRoot = contentRoot;
    }

    public static SqlitePath FromAppSettings(string contentRoot)
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        var config = new ConfigurationBuilder()
            .SetBasePath(contentRoot)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{env}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        return new SqlitePath(config, contentRoot);
    }

    public string BuildConnectionString()
    {
        var p1 = _config["DbFolder:Path1"] ?? "Data";
        var p2 = _config["DbFolder:Path2"] ?? "DB";
        var file = _config["DbFolder:FileName"] ?? "ici.db";

        var dir = Path.Combine(_contentRoot, p1, p2);
        Directory.CreateDirectory(dir);
        var fullPath = Path.Combine(dir, file);

        return $"Data Source={fullPath};Cache=Shared";
    }
}
