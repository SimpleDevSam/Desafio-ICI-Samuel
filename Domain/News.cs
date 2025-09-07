using System.ComponentModel.DataAnnotations;

namespace Desafio_ICI_Samuel.Domain;

public class News
{
    public int Id { get; set; }
    [Required, StringLength(250)]
    public string Title { get; set; } = string.Empty;
    [Required]
    public string Text { get; set; } = string.Empty;
    [Required]
    public int UserId { get; set; }
    [Required]
    public User User { get; set; } = default!;
    public ICollection<NewsTag> NewsTags { get; set; } = new List<NewsTag>();
}
