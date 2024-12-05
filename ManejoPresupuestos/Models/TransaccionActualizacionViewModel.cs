namespace ManejoPresupuestos.Models
{
    public class TransaccionActualizacionViewModel: TransaccionCreacionViewModel
    {
        public int CuentaAnteriorId { get; set; }
        public int MontoAnterior { get; set; }
        public string urlRetorno { get; set; }


    }
}
