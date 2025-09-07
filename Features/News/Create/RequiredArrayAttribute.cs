using System.ComponentModel.DataAnnotations;

namespace Desafio_ICI_Samuel.Features.News.Create;

public class RequiredArrayAttribute : ValidationAttribute
{
    public RequiredArrayAttribute()
    {
        ErrorMessage = "Pelo menos um item deve ser selecionado.";
    }

    public override bool IsValid(object? value)
    {
        if (value is Array array)
        {
            return array.Length > 0;
        }

        if (value is System.Collections.IEnumerable enumerable)
        {
            return enumerable.Cast<object>().Any();
        }

        return false;
    }
}