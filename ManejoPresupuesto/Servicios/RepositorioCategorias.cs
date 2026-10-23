using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;
using System.Data.SqlTypes;

namespace ManejoPresupuesto.Servicios
{

    public interface IRepositorioCategorias
    {
        Task Actualizar(Categoria categoria);
        Task Borrar(int id);
        Task<int> Contar(int usuarioId);
        Task Crear(Categoria categoria);
        Task<IEnumerable<Categoria>> Obtener(int usuarioId, PaginacionViewModel paginacion);
        Task<IEnumerable<Categoria>> Obtener(int usuarioId, TipoOperacion TipoOperacionId);
        Task<Categoria?> ObtenerPorId(int id, int usuarioId);
    }
    public class RepositorioCategorias: IRepositorioCategorias
    {
        private readonly string connectionString;

        public RepositorioCategorias(IConfiguration configuration)
        {
            connectionString =
                configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "No se encontró la cadena de conexión 'DefaultConnection'.");
        }

        public async Task Crear(Categoria categoria)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(@"
                                        Insert into Categorias (Nombre, TipoOperacionId, UsuarioId)
                                        values (@Nombre, @TipoOperacionId, @UsuarioId)

                                        SELECT CAST(SCOPE_IDENTITY() AS int);
                                        ", categoria);

            categoria.Id = id;
        }

        public async Task<IEnumerable<Categoria>> Obtener(int usuarioId, PaginacionViewModel paginacion)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Categoria>("" +
                @$"SELECT * 
                  FROM Categorias 
                  WHERE UsuarioId = @UsuarioId
                  ORDER BY Nombre
                  OFFSET { paginacion.RecordsASaltar} ROWS FETCH NEXT { paginacion.RecordsPorPagina } 
                  ROWS ONLY",
                new {usuarioId});
        }

        public async Task<int>Contar(int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM Categorias WHERE UsuarioId = @usuarioId"
                , new { usuarioId });
        }

        public async Task<IEnumerable<Categoria>> Obtener(int usuarioId,
            TipoOperacion TipoOperacionId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Categoria>("" +
                @"SELECT * 
                FROM Categorias 
                WHERE UsuarioId = @UsuarioId AND TipoOperacionId = @TipoOperacionId",
                new { usuarioId, TipoOperacionId });
        }


        public async Task<Categoria?>ObtenerPorId (int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Categoria>(@"
                            Select * FROM categorias WHERE Id = @Id AND UsuarioId = @UsuarioId"
                            , new { id, usuarioId });
        }

        public async Task Actualizar (Categoria categoria)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"UPDATE Categorias
                                           SET Nombre = @Nombre, TipoOperacionId = @TipoOperacionId
                                           WHERE Id = @id", 
                                           categoria);

        }

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("DELETE Categorias WHERE Id = @Id",
                                            new { id });
        }



    }
}
