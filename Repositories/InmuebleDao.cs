using System.Data;
using MySql.Data.MySqlClient;
using inmobiliaria.Models;
using inmobiliaria.Data;

namespace inmobiliaria.Repositories
{
    public class InmuebleDao(string connectionString)
    {
        private readonly string _connectionString = connectionString;

        public List<Inmueble> ObtenerTodos()
        {
            var lista = new List<Inmueble>();
            using var conn = Conexion.ObtenerConexion(_connectionString);
            using var cmd = new MySqlCommand("SELECT * FROM inmuebles WHERE activo = 1", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(MapearInmueble(reader));
            }
            return lista;
        }

        public List<Inmueble> ObtenerPaginados(int page, int pageSize)
        {
            var lista = new List<Inmueble>();
            using var conn = Conexion.ObtenerConexion(_connectionString);
            using var cmd = new MySqlCommand(@"SELECT * FROM inmuebles WHERE activo = 1 LIMIT @limit OFFSET @offset", conn);
            cmd.Parameters.AddWithValue("@limit", pageSize);
            cmd.Parameters.AddWithValue("@offset", (page - 1) * pageSize);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(MapearInmueble(reader));
            }
            return lista;
        }
        public int ContarInmuebles()
        {
            using var conn = Conexion.ObtenerConexion(_connectionString);
            using var cmd = new MySqlCommand("SELECT COUNT(*) FROM inmuebles WHERE activo = 1", conn);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public Inmueble? ObtenerPorId(int id)
        {
            using var conn = Conexion.ObtenerConexion(_connectionString);
            using var cmd = new MySqlCommand("SELECT * FROM inmuebles WHERE id_inmueble = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return MapearInmueble(reader);
            }
            return null;
        }

        public bool CrearInmueble(Inmueble inmueble)
        {
            using var conn = Conexion.ObtenerConexion(_connectionString);
            using var cmd = new MySqlCommand(@"INSERT INTO inmuebles (id_propietario, id_tipo, uso, direccion, cantidad_ambientes, coordenadas, precio, estado) VALUES (@id_propietario, @id_tipo, @uso, @direccion, @cantidad_ambientes, @coordenadas, @precio, @estado)", conn);
            cmd.Parameters.AddWithValue("@id_propietario", inmueble.IdPropietario);
            cmd.Parameters.AddWithValue("@id_tipo", inmueble.IdTipo);
            cmd.Parameters.AddWithValue("@uso", inmueble.Uso);
            cmd.Parameters.AddWithValue("@direccion", inmueble.Direccion);
            cmd.Parameters.AddWithValue("@cantidad_ambientes", inmueble.CantidadAmbientes ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@coordenadas", inmueble.Coordenadas ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@precio", inmueble.Precio);
            cmd.Parameters.AddWithValue("@estado", inmueble.Estado ?? (object)DBNull.Value);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool ActualizarInmueble(Inmueble inmueble)
        {
            using var conn = Conexion.ObtenerConexion(_connectionString);
            using var cmd = new MySqlCommand(@"UPDATE inmuebles SET id_propietario = @id_propietario, id_tipo = @id_tipo, uso = @uso, direccion = @direccion, cantidad_ambientes = @cantidad_ambientes, coordenadas = @coordenadas, precio = @precio, estado = @estado WHERE id_inmueble = @id", conn);
            cmd.Parameters.AddWithValue("@id", inmueble.IdInmueble);
            cmd.Parameters.AddWithValue("@id_propietario", inmueble.IdPropietario);
            cmd.Parameters.AddWithValue("@id_tipo", inmueble.IdTipo);
            cmd.Parameters.AddWithValue("@uso", inmueble.Uso);
            cmd.Parameters.AddWithValue("@direccion", inmueble.Direccion);
            cmd.Parameters.AddWithValue("@cantidad_ambientes", inmueble.CantidadAmbientes ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@coordenadas", inmueble.Coordenadas ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@precio", inmueble.Precio);
            cmd.Parameters.AddWithValue("@estado", inmueble.Estado ?? (object)DBNull.Value);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool EliminarInmueble(int id)
        {
            using var conn = Conexion.ObtenerConexion(_connectionString);
            using var cmd = new MySqlCommand("UPDATE inmuebles SET activo = 0 WHERE id_inmueble = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            return cmd.ExecuteNonQuery() > 0;
        }

        public List<Inmueble> BuscarPorDireccion(string direccion)
        {
            var lista = new List<Inmueble>();
            using var conn = Conexion.ObtenerConexion(_connectionString);
            using var cmd = new MySqlCommand("SELECT * FROM inmuebles WHERE direccion LIKE @direccion AND activo = 1 AND estado = 'disponible'", conn);
            cmd.Parameters.AddWithValue("@direccion", $"%{direccion}%");
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(MapearInmueble(reader));
            }
            return lista;
        }

        public void ActualizarEstado(int idInmueble, string nuevoEstado)
        {
            Console.WriteLine($"ActualizarEstado llamado con idInmueble={idInmueble}, nuevoEstado={nuevoEstado}");
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();
            var query = "UPDATE inmuebles SET estado = @estado WHERE id_inmueble = @idInmueble";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@estado", nuevoEstado);
            command.Parameters.AddWithValue("@idInmueble", idInmueble);
            command.ExecuteNonQuery();
        }

        private static Inmueble MapearInmueble(IDataRecord reader)
        {
            return new Inmueble
            {
                IdInmueble = reader.GetInt32(reader.GetOrdinal("id_inmueble")),
                IdPropietario = reader.GetInt32(reader.GetOrdinal("id_propietario")),
                IdTipo = reader.GetInt32(reader.GetOrdinal("id_tipo")),
                Uso = reader.GetString(reader.GetOrdinal("uso")),
                Direccion = reader.GetString(reader.GetOrdinal("direccion")),
                CantidadAmbientes = reader.IsDBNull(reader.GetOrdinal("cantidad_ambientes")) ? null : reader.GetInt32(reader.GetOrdinal("cantidad_ambientes")),
                Coordenadas = reader.IsDBNull(reader.GetOrdinal("coordenadas")) ? null : reader.GetString(reader.GetOrdinal("coordenadas")),
                Precio = reader.GetDecimal(reader.GetOrdinal("precio")),
                Estado = reader.IsDBNull(reader.GetOrdinal("estado")) ? null : reader.GetString(reader.GetOrdinal("estado"))
            };
        }
    }
}
