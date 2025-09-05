using System.ComponentModel.DataAnnotations;

namespace Desafio_ICI_Samuel.Features.Tags.Create;
public sealed class CreateTagForm
{
    [Required(ErrorMessage = "Informe o nome da tag")]
    [StringLength(100, ErrorMessage = "Máximo de 100 caracteres")]
    public string Nome { get; set; } = string.Empty;
}
