using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuestos.Models
{
    public class Transaccion
    {
        public int Id { get; set; }
        [Display(Name ="Fecha transaccion")]
        [DataType(DataType.Date)]
        public int UsuarioId { get; set; }

        public DateTime FechaTransaccion { get; set; } = DateTime.Today;

        public decimal Monto { get; set; }


        [Range(1, maximum: int.MaxValue, ErrorMessage = "Debe seleccionar una categoria")]
        [Display(Name = "Categoría")]
        public int CategoriaId { get; set; }


        [StringLength(maximumLength: 1000, ErrorMessage ="La nota no puede ser mayor de {1} caracteres")]
        public string Nota { get; set; }


        [Range(1, maximum: int.MaxValue, ErrorMessage = "Debe seleccionar una cuenta")]
        [Display(Name = "Cuenta")]
        public int CuentaId { get; set; }


        [Display(Name = "Tipo Operación")]
        public TipoOperacion TipoOperacionId { get; set; } = TipoOperacion.Ingreso;

        public string Cuenta { get; set; }
        public string Categoria { get; set; }



    }
}
