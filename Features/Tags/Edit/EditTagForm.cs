using System.ComponentModel.DataAnnotations;

namespace Desafio_ICI_Samuel.Features.Tags.Edit;

public sealed class EditTagForm
{
    [Required] public int Id { get; set; }

    [Required(ErrorMessage = "Informe o nome da tag")]
    [StringLength(100, ErrorMessage = "Máximo de 100 caracteres")]
    public string Name { get; set; } = "";
}
