using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuestos.Models
{
    public class RegistroViewModel
    {

        [Required(ErrorMessage ="El campo {0} es requerido")]
        [EmailAddress(ErrorMessage ="El campo debe ser un correo electronico válido")]
        public string Email { get; set; }


        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int Password { get; set; }

    }
}
