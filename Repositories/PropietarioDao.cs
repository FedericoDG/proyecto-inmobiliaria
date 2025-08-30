using System.Data;
using MySql.Data.MySqlClient;
using inmobiliaria.Models;
using inmobiliaria.Data;

namespace inmobiliaria.Repositories
{
    public class PropietarioDao(string connectionString)
    {
        private readonly string _connectionString = connectionString;
        public List<Propietario> ObtenerPaginados(int page, int pageSize)
        {
            try
            {
                var lista = new List<Propietario>();
                using var conn = Conexion.ObtenerConexion(_connectionString);
                var cmd = new MySqlCommand(@"SELECT * FROM propietarios 
                                WHERE activo = 1 
                                LIMIT @limit OFFSET @offset", conn);
                cmd.Parameters.AddWithValue("@limit", pageSize);
                cmd.Parameters.AddWithValue("@offset", (page - 1) * pageSize);

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    lista.Add(MapearPropietario(reader));
                }
                return lista;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return [];
            }
        }

        public int ContarPropietarios()
        {
            try
            {
                using var conn = Conexion.ObtenerConexion(_connectionString);
                var cmd = new MySqlCommand("SELECT COUNT(*) FROM propietarios WHERE activo = 1", conn);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 0;
            }
        }
        public List<Propietario> ObtenerTodos()
        {
            try
            {
                var lista = new List<Propietario>();
                using var conn = Conexion.ObtenerConexion(_connectionString);
                var cmd = new MySqlCommand("SELECT * FROM propietarios WHERE activo = 1", conn);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    lista.Add(MapearPropietario(reader));
                }
                return lista;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return [];
            }
        }

        public Propietario? ObtenerPorId(int id)
        {
            try
            {
                using var conn = Conexion.ObtenerConexion(_connectionString);
                var cmd = new MySqlCommand("SELECT * FROM propietarios WHERE id_propietario = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return MapearPropietario(reader);
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        public List<Propietario> BuscarPorDni(string dni)
        {
            try
            {
                var lista = new List<Propietario>();
                using var conn = Conexion.ObtenerConexion(_connectionString);
                using var cmd = new MySqlCommand("SELECT * FROM propietarios WHERE dni LIKE @dni AND activo = 1", conn);
                cmd.Parameters.AddWithValue("@dni", $"%{dni}%");
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    lista.Add(MapearPropietario(reader));
                }
                return lista;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return [];
            }
        }

        public bool CrearPropietario(Propietario propietario)
        {
            try
            {
                using var conn = Conexion.ObtenerConexion(_connectionString);
                var cmd = new MySqlCommand(@"INSERT INTO propietarios (dni, nombre, apellido, telefono, email, direccion) VALUES (@dni, @nombre, @apellido, @telefono, @email, @direccion)", conn);
                cmd.Parameters.AddWithValue("@dni", propietario.Dni);
                cmd.Parameters.AddWithValue("@nombre", propietario.Nombre);
                cmd.Parameters.AddWithValue("@apellido", propietario.Apellido);
                cmd.Parameters.AddWithValue("@telefono", propietario.Telefono ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@email", propietario.Email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@direccion", propietario.Direccion ?? (object)DBNull.Value);
                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public bool ActualizarPropietario(Propietario propietario)
        {
            using var conn = Conexion.ObtenerConexion(_connectionString);
            var cmd = new MySqlCommand(@"UPDATE propietarios SET dni = @dni, nombre = @nombre, apellido = @apellido, telefono = @telefono, email = @email, direccion = @direccion WHERE id_propietario = @id", conn);
            cmd.Parameters.AddWithValue("@id", propietario.IdPropietario);
            cmd.Parameters.AddWithValue("@dni", propietario.Dni);
            cmd.Parameters.AddWithValue("@nombre", propietario.Nombre);
            cmd.Parameters.AddWithValue("@apellido", propietario.Apellido);
            cmd.Parameters.AddWithValue("@telefono", propietario.Telefono ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@email", propietario.Email ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@direccion", propietario.Direccion ?? (object)DBNull.Value);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool EliminarPropietario(int id)
        {
            using var conn = Conexion.ObtenerConexion(_connectionString);
            var cmd = new MySqlCommand("UPDATE propietarios SET activo = 0 WHERE id_propietario = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            return cmd.ExecuteNonQuery() > 0;
        }

        private static Propietario MapearPropietario(IDataRecord reader)
        {
            return new Propietario
            {
                IdPropietario = reader.GetInt32(reader.GetOrdinal("id_propietario")),
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
