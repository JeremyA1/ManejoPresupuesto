using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{

    public interface IRepositorioUsuarios
    {
        Task Actualizar(Usuario usuario);
        Task<Usuario> BuscarUsuarioPorEmail(string emailNormalizado);
        Task<int> CrearUsuario(Usuario usuario);
    }

    public class RepositorioUsuario : IRepositorioUsuarios
    {
        private readonly string connectionString;
        public RepositorioUsuario(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<int> CrearUsuario(Usuario usuario)
        {
            using var connection = new SqlConnection(connectionString);

            var usuarioId = await connection.QuerySingleAsync<int>(@"
                    INSERT INTO Usuarios (Email, EmailNormalizado, PasswordHash)
                    VALUES (@Email, @EmailNormalizado, @PasswordHash);

                    SELECT CAST(SCOPE_IDENTITY() AS int);",
                usuario);

            await connection.ExecuteAsync("CrearDatosUsuarioNuevo", new { usuarioId },
                commandType: System.Data.CommandType.StoredProcedure);

            return usuarioId;
        }

        public async Task<Usuario> BuscarUsuarioPorEmail(string emailNormalizado)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QuerySingleOrDefaultAsync<Usuario>(
                "SELECT * FROM Usuarios WHERE EmailNormalizado = @emailNormalizado",
                new { emailNormalizado });
        }

        public async Task Actualizar(Usuario usuario)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"
                
            Update Usuarios
            SET PasswordHash = @PasswordHash
            WHERE Id = @Id", usuario);
        }
    }
}
