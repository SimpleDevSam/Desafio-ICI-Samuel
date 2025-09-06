namespace Desafio_ICI_Samuel.Features.News.Form;

public interface IFormBuilder
{
    Task<NewsForm> Build(Domain.News? n = null);
}
