using Desafio_ICI_Samuel.Data;
using Desafio_ICI_Samuel.Features.Tags.Create;
using Desafio_ICI_Samuel.Features.Tags.Delete;
using Desafio_ICI_Samuel.Features.Tags.Edit;
using Desafio_ICI_Samuel.Features.Tags.Get;
using Desafio_ICI_Samuel.Features.Tags.List;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var dbPath = Path.Combine(builder.Environment.ContentRootPath, "App_Data", "ici.db");
Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
builder.Services.AddDbContext<AppDbContext>(opt =>opt.UseSqlite($"Data Source={dbPath};Cache=Shared"));

builder.Services.AddScoped<CreateTagHandler>();
builder.Services.AddScoped<ListTagsHandler>();
builder.Services.AddScoped<EditTagHandler>();
builder.Services.AddScoped<DeleteTagHandler>();
builder.Services.AddScoped<ViewTagHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    app.Logger.LogInformation("SQLite file => {Path}",
        Path.GetFullPath(db.Database.GetDbConnection().DataSource ?? "unknown"));
    await db.Database.MigrateAsync();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
