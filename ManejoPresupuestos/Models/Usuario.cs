namespace ManejoPresupuestos.Models
{
    public class Usuario
    {

        //Invocamos las columnas existente desde nuestra base de datos / Tabla Usuarios
        public int Id { get; set; }

        public string Email { get; set; }

        public string EmailNormalizado { get; set; }

        public string PasswordHash { get; set; }

    }


}
