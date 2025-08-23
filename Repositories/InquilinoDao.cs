using System.Data;
using MySql.Data.MySqlClient;
using inmobiliaria.Models;
using inmobiliaria.Data;

namespace inmobiliaria.Repositories
{
    public class InquilinoDao(string connectionString)
    {
        private readonly string _connectionString = connectionString;

        public List<Inquilino> ObtenerPaginados(int page, int pageSize)
        {
            var lista = new List<Inquilino>();
            using var conn = Conexion.ObtenerConexion(_connectionString);
            using var cmd = new MySqlCommand(@"SELECT * FROM inquilinos 
                                      WHERE activo = 1 
                                      LIMIT @limit OFFSET @offset", conn);
            cmd.Parameters.AddWithValue("@limit", pageSize);
            cmd.Parameters.AddWithValue("@offset", (page - 1) * pageSize);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(MapearInquilino(reader));
            }
            return lista;
        }

        public int ContarInquilinos()
        {
            using var conn = Conexion.ObtenerConexion(_connectionString);
            using var cmd = new MySqlCommand("SELECT COUNT(*) FROM inquilinos WHERE activo = 1", conn);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public List<Inquilino> ObtenerTodos()
        {
            var lista = new List<Inquilino>();
            using var conn = Conexion.ObtenerConexion(_connectionString);
            using var cmd = new MySqlCommand("SELECT * FROM inquilinos WHERE activo = 1", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(MapearInquilino(reader));
            }
            return lista;
        }

        public Inquilino? ObtenerPorId(int id)
        {
            using var conn = Conexion.ObtenerConexion(_connectionString);
            using var cmd = new MySqlCommand("SELECT * FROM inquilinos WHERE id_inquilino = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return MapearInquilino(reader);
            }
            return null;
        }

        public bool CrearInquilino(Inquilino inquilino)
        {
            using var conn = Conexion.ObtenerConexion(_connectionString);
            using var cmd = new MySqlCommand(@"INSERT INTO inquilinos (dni, nombre, apellido, telefono, email, direccion) VALUES (@dni, @nombre, @apellido, @telefono, @email, @direccion)", conn);
            cmd.Parameters.AddWithValue("@dni", inquilino.Dni);
            cmd.Parameters.AddWithValue("@nombre", inquilino.Nombre);
            cmd.Parameters.AddWithValue("@apellido", inquilino.Apellido);
            cmd.Parameters.AddWithValue("@telefono", inquilino.Telefono ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@email", inquilino.Email ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@direccion", inquilino.Direccion ?? (object)DBNull.Value);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool ActualizarInquilino(Inquilino inquilino)
        {
            using var conn = Conexion.ObtenerConexion(_connectionString);
            using var cmd = new MySqlCommand(@"UPDATE inquilinos SET dni = @dni, nombre = @nombre, apellido = @apellido, telefono = @telefono, email = @email, direccion = @direccion WHERE id_inquilino = @id", conn);
            cmd.Parameters.AddWithValue("@id", inquilino.IdInquilino);
            cmd.Parameters.AddWithValue("@dni", inquilino.Dni);
            cmd.Parameters.AddWithValue("@nombre", inquilino.Nombre);
            cmd.Parameters.AddWithValue("@apellido", inquilino.Apellido);
            cmd.Parameters.AddWithValue("@telefono", inquilino.Telefono ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@email", inquilino.Email ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@direccion", inquilino.Direccion ?? (object)DBNull.Value);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool EliminarInquilino(int id)
        {
            using var conn = Conexion.ObtenerConexion(_connectionString);
            using var cmd = new MySqlCommand("UPDATE inquilinos SET activo = 0 WHERE id_inquilino = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            return cmd.ExecuteNonQuery() > 0;
        }

        public List<Inquilino> BuscarPorDni(string dni)
        {
            var lista = new List<Inquilino>();
            using var conn = Conexion.ObtenerConexion(_connectionString);
            using var cmd = new MySqlCommand("SELECT * FROM inquilinos WHERE dni LIKE @dni AND activo = 1", conn);
            cmd.Parameters.AddWithValue("@dni", $"%{dni}%");
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(MapearInquilino(reader));
            }
            return lista;
        }

        private static Inquilino MapearInquilino(IDataRecord reader)
        {
            return new Inquilino
            {
                IdInquilino = reader.GetInt32(reader.GetOrdinal("id_inquilino")),
                Dni = reader.GetString(reader.GetOrdinal("dni")),
                Nombre = reader.GetString(reader.GetOrdinal("nombre")),
                Apellido = reader.GetString(reader.GetOrdinal("apellido")),
                Telefono = reader.IsDBNull(reader.GetOrdinal("telefono")) ? null : reader.GetString(reader.GetOrdinal("telefono")),
                Email = reader.IsDBNull(reader.GetOrdinal("email")) ? null : reader.GetString(reader.GetOrdinal("email")),
                Direccion = reader.IsDBNull(reader.GetOrdinal("direccion")) ? null : reader.GetString(reader.GetOrdinal("direccion"))
            };
        }
    }
}
