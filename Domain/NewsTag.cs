using Desafio_ICI_Samuel.Models;

namespace Desafio_ICI_Samuel.Domain;

public class NewsTag
{
    public int NewsId { get; set; }
    public News News { get; set; } = default!;
    public int TagId { get; set; }
    public Tag Tag { get; set; } = default!;
}
