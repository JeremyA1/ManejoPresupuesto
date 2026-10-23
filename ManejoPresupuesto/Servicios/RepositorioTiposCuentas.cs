using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{
    public interface IRepositorioTiposCuentas
    {
        Task Actualizar(TipoCuenta tipoCuenta);
        Task Borrar(int id);
        Task Crear(TipoCuenta tipoCuenta);
        Task<bool> Existe(string nombre, int usuarioId, int id = 0);
        Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId);
        Task<TipoCuenta?> ObtenerPorId(int id, int usuarioId);
        Task Ordernar(IEnumerable<TipoCuenta> tipoCuentasOrdenados);
    }

    public class RepositorioTiposCuentas : IRepositorioTiposCuentas
    {
        private readonly string _connectionString;

        public RepositorioTiposCuentas(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "No se encontró la cadena de conexión 'DefaultConnection'.");
        }

        public async Task Crear(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = "TiposCuentas_Insertar";

            var id = await connection.QuerySingleAsync<int>(
                     sql,
                     new
                     {
                         Nombre = tipoCuenta.Nombre,
                         UsuarioId = tipoCuenta.UsuarioId
                     },
                     commandType: System.Data.CommandType.StoredProcedure
             );

            tipoCuenta.Id = id;
        } 

        public async Task<bool> Existe(string nombre, int usuarioId, int id = 0)
        {
            using var connection = new SqlConnection(_connectionString);
            var existe = await connection.QueryFirstOrDefaultAsync<int>(@"
                        select 1
                        from TiposCuentas
                        where Nombre = @Nombre and UsuarioId = @UsuarioId AND Id <> @id;", 
                        new {nombre, usuarioId, id});

            return existe == 1;
        }

        public async Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<TipoCuenta>(@"
                        select Id, Nombre, Orden
                        from TiposCuentas
                        where UsuarioId = @UsuarioId
                        order by orden"
                        , new { usuarioId });
        }

        public async Task Actualizar(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(@"
                        UPDATE TiposCuentas
                        SET Nombre = @Nombre
                        WHERE Id = @Id",
                        tipoCuenta);
        }

        public async Task<TipoCuenta?> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(_connectionString);

            return await connection.QueryFirstOrDefaultAsync<TipoCuenta>(@"
                        Select Id, Nombre, Orden
                        from TiposCuentas
                        Where Id = @Id and UsuarioId = @UsuarioId",
                        new {id, usuarioId } );
        }


        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(@"delete TiposCuentas where id = @Id", new {id} );
        }


        public async Task Ordernar(IEnumerable<TipoCuenta> tipoCuentasOrdenados)
        {
            var query = @"
                        update TiposCuentas 
                        set Orden = @Orden 
                        where Id = @Id;";
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(query, tipoCuentasOrdenados);
        }
    }
}
