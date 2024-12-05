namespace ManejoPresupuestos.Models
{
    public class ReporteTransaccionesDetalladas
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; } //MOSTRAMOS TRANSACCIONES DE TAL FECHA A TAL FECHA
        public IEnumerable<TransaccionesPorFecha> TransaccionesAgrupadas { get; set; }

        public decimal BalanceDepositos => 
            TransaccionesAgrupadas.Sum(x => x.BalanceDepositos); //CALCULAMOS EL TOTAL EN EL RANGO DE FECHO DE INGRESOS
        
        public decimal BalanceRetiros =>
            TransaccionesAgrupadas.Sum(x => x.BalanceRetiros); //CALCULAMOS EL TOTAL EN EL RANGO DE FECHO DE GASTOS

        public decimal Total => BalanceDepositos - BalanceRetiros; //TOTAL - RESTA ENTRE DEPOSITOS Y RETIROS



        //CREAMOS CLASE PARA AGRUPAR TRANSACCIONES POR FECHA 
        public class TransaccionesPorFecha
        {
            public DateTime FechaTransaccion { get; set; } //DETECTAMOS A QUE FECHA CORRESPONDEN LAS TRANSACCIONES UBICADAS ACÁ
            public IEnumerable<Transaccion> Transacciones { get; set; } //COLOCAMOS LAS TRANSACCIONES

            public decimal BalanceDepositos => Transacciones.Where(x => x.TipoOperacionId == TipoOperacion.Ingreso).
                Sum(x => x.Monto); //BALANCE DE LOS DEPOSITOS

            public decimal BalanceRetiros =>
                Transacciones.Where(x => x.TipoOperacionId == TipoOperacion.Gasto).
                Sum(x => x.Monto); //BALANCE DE LOS RETIROS

        }


    }
}
