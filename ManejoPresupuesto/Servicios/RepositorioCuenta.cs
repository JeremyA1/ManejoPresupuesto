using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{
    public interface IRepositorioCuentas
    {
        Task Actualizar(CuentaCreacionViewModel cuenta);
        Task Borrar(int id);
        Task<IEnumerable<Cuenta>> Buscar(int usuarioId);
        Task Crear(Cuenta cuenta);
        Task<Cuenta?> ObtenerPorId(int id, int usuarioId);
    }

    public class RepositorioCuentas: IRepositorioCuentas
    {
        private readonly string connectionString;

        public RepositorioCuentas(IConfiguration configuration)
        {
            connectionString =
                configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "No se encontró la cadena de conexión 'DefaultConnection'.");
        }

        public async Task Crear(Cuenta cuenta)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(@"
                        INSERT INTO Cuentas (Nombre, TipoCuentaId, Descripcion, Balance)
                        VALUES (@Nombre, @TipoCuentaId, @Descripcion, @Balance);
                        SELECT CAST(SCOPE_IDENTITY() AS int);",
               cuenta);

            cuenta.Id = id;
        }

        public async Task<IEnumerable<Cuenta>> Buscar(int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Cuenta>(@"
                            SELECT
                            Cuentas.Id,
                            Cuentas.Nombre,
                            Cuentas.Balance,
                            tc.Nombre AS TipoCuenta
                            FROM Cuentas
                            INNER JOIN TiposCuentas AS tc
                            ON tc.Id = Cuentas.TipoCuentaId
                            WHERE tc.UsuarioId = @UsuarioId
                            ORDER BY tc.Orden;",
                            new { usuarioId });
        }


        public async Task<Cuenta?> ObtenerPorId (int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Cuenta>(
                @"          SELECT
                            Cuentas.Id,
                            Cuentas.Nombre,
                            Cuentas.Balance,
                            Descripcion,
                            TipoCuentaId
                            FROM Cuentas
                            INNER JOIN TiposCuentas AS tc
                            ON tc.Id = Cuentas.TipoCuentaId
                            WHERE tc.UsuarioId = @UsuarioId AND Cuentas.Id = @Id", 
                new {id, usuarioId});
        }

        public async Task Actualizar(CuentaCreacionViewModel cuenta)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(
                            @"                      
                            Update Cuentas
                            set Nombre = @Nombre, 
                            Balance = @Balance, 
                            Descripcion = @Descripcion,
                            TipoCuentaId = @TipoCuentaId
                            where Id = @Id;", 
                            cuenta);
        }


        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);
                await connection.ExecuteAsync("DELETE Cuentas Where Id = @Id", new { id });
        }
    }
}
