using Dapper;
using ManejoPresupuestos.Models;
using Microsoft.AspNetCore.Server.Kestrel.Core.Features;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuestos.Servicios
{
    public interface IRepositorioTransacciones
    {
        Task Actualizar(Transaccion transaccion, decimal montoAnterior, int cuentaAnterior);
        Task Borrar(int id);
        Task Crear(Transaccion transaccion);
        Task<IEnumerable<Transaccion>> ObtenerPorCuentaId(ObtenerTransaccionesPorCuenta modelo);
        Task<Transaccion> ObtenerPorId(int id, int usuarioid);
        Task<IEnumerable<ResultadoObtenerPorMes>> ObtenerPorMes(int usuarioId, int año);
        Task<IEnumerable<ResultadoObtenerPorSemana>> ObtenerPorSemana(ParametroObtenerTransaccionesPorUsuario modelo);
        Task<IEnumerable<Transaccion>> ObtenerPorUsuarioId(ParametroObtenerTransaccionesPorUsuario modelo);
    }
    public class RepositorioTransacciones: IRepositorioTransacciones
    {
        private readonly string connectionString;
        public RepositorioTransacciones(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
            
        }

        public async Task Crear(Transaccion transaccion)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>("Transacciones_Insertar",
               new { 
                    transaccion.UsuarioId, 
                    transaccion.FechaTransaccion,
                    transaccion.Monto,
                    transaccion.Nota,
                    transaccion.CuentaId,
                    transaccion.CategoriaId
                }, 
                commandType: System.Data.CommandType.StoredProcedure);

            transaccion.Id = id;

        }

        //METODO QUE NOS VA A AYUDAR A BUSCAR LAS TRANSACCIONES POR CUENTA

        public async Task<IEnumerable<Transaccion>> ObtenerPorCuentaId(ObtenerTransaccionesPorCuenta modelo)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Transaccion>(
                @"SELECT t.Id, t.Monto, t.FechaTransaccion, C.Nombre as Categoria, 
                cu.Nombre as Cuenta, c.TipoOperacionId
                FROM Transacciones t
                INNER JOIN Categorias c
                ON c.Id = t.CategoriaId
                INNER JOIN Cuentas cu
                ON cu.Id = t.CuentaId
                WHERE t.CuentaId = @CuentaId AND t.UsuarioId = @UsuarioId
                AND FechaTransaccion BETWEEN @FechaInicio AND @FechaFin", modelo);
        }


        ///METODO PARA EL CONSULTA DEL REPORTE DIARIO

        public async Task<IEnumerable<Transaccion>> ObtenerPorUsuarioId(ParametroObtenerTransaccionesPorUsuario modelo)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Transaccion>(
                @"SELECT t.Id, t.Monto, t.FechaTransaccion, C.Nombre as Categoria, 
                cu.Nombre as Cuenta, c.TipoOperacionId, Nota
                FROM Transacciones t
                INNER JOIN Categorias c
                ON c.Id = t.CategoriaId
                INNER JOIN Cuentas cu
                ON cu.Id = t.CuentaId
                WHERE t.UsuarioId = @UsuarioId
                AND FechaTransaccion BETWEEN @FechaInicio AND @FechaFin
                ORDER BY t.FechaTransaccion DESC", modelo);
        }


        //LLAMAMOS A NUESTRO PROCEDIMIENTO ALMACENADO DE TRANSACCIONES_ACTUALIZAR

        public async Task Actualizar(Transaccion transaccion, decimal montoAnterior, 
            int cuentaAnteriorId)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("Transacciones_Actualizar",
                new
                {
                    transaccion.Id,
                    transaccion.FechaTransaccion,
                    transaccion.Monto,
                    transaccion.CategoriaId,
                    transaccion.CuentaId,
                    transaccion.Nota,
                    montoAnterior,
                    cuentaAnteriorId
                }, commandType: System.Data.CommandType.StoredProcedure);
        }



        //METODO PARA OBTENER LA TRANSACCION POR ID

        public async Task<Transaccion> ObtenerPorId(int id, int usuarioid)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Transaccion>(
                @"SELECT Transacciones.*, cat.TipoOperacionId 
                FROM Transacciones
                INNER JOIN Categorias cat
                ON cat.Id = Transacciones.CategoriaId
                WHERE Transacciones.Id = @Id AND Transacciones.UsuarioId = @UsuarioId", new { id, usuarioid });
        }


        public async Task<IEnumerable<ResultadoObtenerPorSemana>> ObtenerPorSemana
            (ParametroObtenerTransaccionesPorUsuario modelo)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<ResultadoObtenerPorSemana>(@"
                    Select datediff(d, @fechaInicio, FechaTransaccion) / 7 + 1 as Semana,
                    SUM(Monto) as Monto, cat.TipoOperacionId
                    FROM Transacciones
                    INNER JOIN Categorias cat
                    ON cat.Id = Transacciones.CategoriaId
                    WHERE Transacciones.UsuarioId = @usuarioId AND
                    FechaTransaccion BETWEEN @fechaInicio and @fechaFin
                    GROUP BY datediff(d, @fechaInicio, FechaTransaccion) / 7, cat.TipoOperacionId", modelo);
        }

        public async Task<IEnumerable<ResultadoObtenerPorMes>> ObtenerPorMes(int usuarioId, int año)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<ResultadoObtenerPorMes>(@"
                        SELECT MONTH (FechaTransaccion) as Mes, 
                        SUM (Monto) as Monto, cat.TipoOperacionId
                        FROM Transacciones
                        INNER JOIN Categorias cat
                        ON cat.Id = Transacciones.CategoriaId
                        WHERE Transacciones.UsuarioId = @usuarioId AND YEAR(FechaTransaccion) = @Año
                        GROUP BY Month(FechaTransaccion), cat.TipoOperacionId", new {usuarioId, año});
        }





        //Implementamos método de Borrar con nuestro procedimiento almacenado

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("Transacciones_Borrar",
                new { id }, commandType: System.Data.CommandType.StoredProcedure); //Hacemos referencia a nuestro procedimiento almacenado
        }
    }
}
