using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace ManejoPresupuesto.Models
{
    public class CuentaCreacionViewModel : Cuenta
    {
        [ValidateNever]
        public IEnumerable<SelectListItem> TiposCuentas { get; set; }
            = Enumerable.Empty<SelectListItem>();
    }
}