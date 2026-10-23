using ManejoPresupuesto.Validaciones;
using System.ComponentModel.DataAnnotations;
using Xunit.Abstractions;

namespace ManejoPresupuesto.Models
{
    public class Cuenta
    {
        public int Id {get; set;}
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 50)]
        [PrimeraLetraMayuscula]
        public string Nombre { get; set; } = string.Empty;
        [Display(Name = "Tipo cuenta")]
        public int TipoCuentaId  { get; set;}
        public decimal Balance { get; set;}
        public string? Descripcion { get; set; }
        public string? TipoCuenta { get; set; }

    }
}
