using Desafio_ICI_Samuel.Features.News.Delete;
using Desafio_ICI_Samuel.Features.News.Edit;
using Desafio_ICI_Samuel.Features.News.Form;
using Desafio_ICI_Samuel.Features.News.Get;
using Desafio_ICI_Samuel.Features.News.ListNews;
using Desafio_ICI_Samuel.Features.Tags.Create;
using Desafio_ICI_Samuel.Features.Tags.Delete;
using Desafio_ICI_Samuel.Features.Tags.Edit;
using Desafio_ICI_Samuel.Features.Tags.Get;
using Desafio_ICI_Samuel.Features.Tags.List;
using Desafio_ICI_Samuel.Features;
using Desafio_ICI_Samuel.Data;
using Microsoft.EntityFrameworkCore;

namespace Desafio_ICI;

public static class DependencyInjection
{

    public static IServiceCollection AddHandlers(this IServiceCollection services)
    {
        services.AddScoped<ICreateTagHandler, CreateTagHandler>();
        services.AddScoped<IListTagsHandler, ListTagsHandler>();
        services.AddScoped<IEditTagHandler, EditTagHandler>();
        services.AddScoped<IDeleteTagHandler, DeleteTagHandler>();
        services.AddScoped<IGetTagHandler, GetTagHandler>();
        services.AddScoped<ICreateNewsHandler, CreateNewsHandler>();
        services.AddScoped<IEditNewsHandler, EditNewsHandler>();
        services.AddScoped<IDeleteNewsHandler, DeleteNewsHandler>();
        services.AddScoped<IGetNewsHandler, GetNewsHandler>();
        services.AddScoped<IListNewsHandler, ListNewsHandler>();
        services.AddScoped<IFormBuilder, FormBuilder>();

        return services;
    }

    public static IServiceCollection AddPersistence(this IServiceCollection services, WebApplicationBuilder builder)
    {

        var conectionString = new Desafio_ICI_Samuel.Data.SqlitePath(
            builder.Configuration,
            builder.Environment.ContentRootPath
        ).BuildConnectionString();

        services.AddDbContext<AppDbContext>(opt =>
            opt.UseSqlite(conectionString));

        return services;
    }

}
