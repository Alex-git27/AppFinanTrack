using ManejoPresupuestos.Validaciones;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuestos.Models
{
    public class TipoCuenta
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="El campo {0} es requerido")] ///Mensaje de error cuando no se llena el campo de nombre
        [PrimeraLetraMayuscula]
        [Remote(action:"VerificarExisteTipoCuenta", controller: "TiposCuentas")]
        public String Nombre { get; set; }
        public int UsuarioId { get; set; }
        public int Orden { get; set; }

    }
}
