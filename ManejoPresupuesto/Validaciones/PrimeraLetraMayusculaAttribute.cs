using System.ComponentModel.DataAnnotations;
namespace ManejoPresupuesto.Validaciones
{
    public class PrimeraLetraMayusculaAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not string texto || string.IsNullOrWhiteSpace(texto))
            {
                return ValidationResult.Success;
            }

            if (!char.IsUpper(texto[0]))
            {
                return new ValidationResult("La primera letra debe ser mayúscula");
            }

            return ValidationResult.Success;
        }
    }
}
