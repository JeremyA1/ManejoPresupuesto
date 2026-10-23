using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;


namespace ManejoPresupuesto.Models
{
    public class TransaccionCreacionViewModel : Transaccion
    {
        [ValidateNever]
        public IEnumerable<SelectListItem> Categorias { get; set; }
            = Enumerable.Empty<SelectListItem>();

        [ValidateNever]
        public IEnumerable<SelectListItem> Cuentas { get; set; }
            = Enumerable.Empty<SelectListItem>();


    }
}
