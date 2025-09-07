using System.ComponentModel.DataAnnotations;

namespace Desafio_ICI_Samuel.Models;

public class Tag
{
    [Required]
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = "";
}
