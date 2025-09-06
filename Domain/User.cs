using System.ComponentModel.DataAnnotations;

namespace Desafio_ICI_Samuel.Domain
{
    public class User
    {
        public int Id { get; set; }
        [Required, StringLength(250)]
        public string Name { get; set; } = string.Empty;
        [Required, StringLength(250)]
        public string Password { get; set; } = string.Empty;
        [Required, StringLength(250)]
        public string Email { get; set; } = string.Empty;
    }
}
