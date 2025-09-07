using Desafio_ICI_Samuel.Features.News.Create;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Desafio_ICI_Samuel.Features;

public class NewsForm
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "O campo Título é obrigatório."), StringLength(250)]
    public string Title { get; set; } = "";

    [Required(ErrorMessage = "O campo Texto é obrigatório.")]
    public string Text { get; set; } = "";

    [Required(ErrorMessage = "Selecione o autor")]
    public int? UserId { get; set; }

    [RequiredArray(ErrorMessage = "Selecione pelo menos uma tag")]
    public int[] TagIds { get; set; } = Array.Empty<int>();
    public IEnumerable<SelectListItem> AvailableUsers { get; set; } = Enumerable.Empty<SelectListItem>();
    public IEnumerable<SelectListItem> AvailableTags { get; set; } = Enumerable.Empty<SelectListItem>();
}
