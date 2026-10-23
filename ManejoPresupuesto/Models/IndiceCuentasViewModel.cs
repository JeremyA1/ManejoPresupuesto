namespace ManejoPresupuesto.Models
{
    public class IndiceCuentasViewModel
    {
        public string TipoCuenta { get; set; }
            = string.Empty;

        public IEnumerable<Cuenta> Cuentas { get; set; }
            = Enumerable.Empty<Cuenta>();

        public decimal Balance =>
            Cuentas.Sum(x => x.Balance);
    }
}

