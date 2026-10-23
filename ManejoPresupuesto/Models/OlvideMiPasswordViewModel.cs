using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Models
{
    public class OlvideMiPasswordViewModel
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [EmailAddress(ErrorMessage = "El campo debe ser un un correo electronico valido")]
        public required string Email { get; set; }
    }
}
