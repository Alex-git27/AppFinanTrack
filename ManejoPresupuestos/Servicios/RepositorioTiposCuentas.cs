using Dapper;
using ManejoPresupuestos.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuestos.Servicios
{

    public interface IRepositorioTiposCuentas{

        Task Actualizar(TipoCuenta tipoCuenta);

        Task Borrar (int id);
        Task Crear (TipoCuenta tipoCuenta);
        Task<bool> Existe(string nombre, int usuarioId);
        Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId);
        Task<TipoCuenta> ObtenerPorId(int id, int usuarioId);
        Task Ordenar(IEnumerable<TipoCuenta> tipoCuentasOrdenados);
    }

    public class RepositorioTiposCuentas: IRepositorioTiposCuentas
    {

        private readonly string connectionString;
        public RepositorioTiposCuentas(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");   
        }

        public async Task Crear(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>
                                                 ("TiposCuentas_Insertar",
                                                 new {usuarioId = tipoCuenta.UsuarioId,
                                                 nombre = tipoCuenta.Nombre},
                                                 commandType: System.Data.CommandType.StoredProcedure);

            tipoCuenta.Id = id;
        }

        //Verificar si ya existe 1 tipo cuenta con el nombre indicado para que no se repita en la DB 

        public async Task<bool> Existe (string nombre, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            var existe = await connection.QueryFirstOrDefaultAsync<int>(
                                                                 @"SELECT 1
                                                                 FROM TiposCuentas
                                                                 WHERE Nombre = @Nombre AND UsuarioId = @UsuarioId;",                                        
                                                                 new {nombre, usuarioId} ); //Retorna el primer registro que encuentre o un cero por defecto

            return existe == 1;
        }



        public async Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<TipoCuenta>(@"SELECT Id, Nombre, Orden
                                                           FROM TiposCuentas
                                                           WHERE UsuarioId = @UsuarioId
                                                           ORDER BY Orden", new { usuarioId });
        }

        //Permitir actualizar los tipos de cuentas, para que cuando sea creado, también sea actualizado

        public async Task Actualizar(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(connectionString); //ElEXECUTE permite ejecutar un QUERY que no va a retornar nada, este caso UPDATE
            await connection.ExecuteAsync(@"UPDATE TiposCuentas   
                                         SET Nombre = @Nombre
                                         WHERE Id = @Id", tipoCuenta);
        }


        public async Task<TipoCuenta> ObtenerPorId(int id, int usuarioId)  //Nos va a permitir el tipo de cuenta por ID
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<TipoCuenta>(@"
                                                                        SELECT Id, Nombre, Orden
                                                                        FROM TiposCuentas
                                                                        WHERE Id= @Id AND UsuarioId = @UsuarioId", 
                                                                        new { id, usuarioId });
        }

        //Metodo para Eliminar

        public async Task Borrar (int id)   
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("DELETE TiposCuentas WHERE Id = @Id", new { id });
        }

        public async Task Ordenar(IEnumerable<TipoCuenta> tipoCuentasOrdenados)
        {
            var query = "UPDATE TiposCuentas SET Orden = @Orden Where Id = @Id;";
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(query, tipoCuentasOrdenados);
        }

    }
}
