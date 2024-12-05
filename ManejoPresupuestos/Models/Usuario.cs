namespace ManejoPresupuestos.Models
{
    public class Usuario
    {

        //Invocamos las columnas existente desde nuestra base de datos / Tabla Usuarios
        public int Id { get; set; }

        public string Email { get; set; }

        public int EmailNormalizado { get; set; }

        public int PasswordHash { get; set; }

    }


}
