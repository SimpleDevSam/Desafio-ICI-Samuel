namespace Desafio_ICI_Samuel.Domain;

public class News
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public int UserId { get; set; }
    public User User { get; set; } = default!;
    public ICollection<NewsTag> NewsTags { get; set; } = new List<NewsTag>();
}
