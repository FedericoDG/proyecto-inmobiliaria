using System.Data;
using MySql.Data.MySqlClient;
using inmobiliaria.Models;
using inmobiliaria.Data;

namespace inmobiliaria.Repositories
{
    public class InmuebleDao(string connectionString)
    {
        private readonly string _connectionString = connectionString;

        // Filtra inmuebles por propietario, estado y fechas de ocupación
        public List<Inmueble> ObtenerFiltrados(int page, int pageSize, int? propietarioId, string? estado, DateTime? fechaInicio, DateTime? fechaFin)
        {
            var lista = new List<Inmueble>();
            try
            {
                using var conn = Conexion.ObtenerConexion(_connectionString);
                var sql = "SELECT * FROM inmuebles WHERE activo = 1";
                if (propietarioId != null)
                    sql += " AND id_propietario = @propietarioId";
                if (!string.IsNullOrEmpty(estado))
                    sql += " AND estado = @estado";
                if (fechaInicio != null && fechaFin != null)
                {
                    sql += @" AND NOT EXISTS (
                                SELECT 1 FROM contratos c
                                WHERE c.id_inmueble = inmuebles.id_inmueble
                                  AND NOT (c.fecha_fin_original < @fechaInicio OR c.fecha_inicio > @fechaFin)
                            )";
                }
                sql += " LIMIT @limit OFFSET @offset";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@limit", pageSize);
                cmd.Parameters.AddWithValue("@offset", (page - 1) * pageSize);
                if (propietarioId != null)
                    cmd.Parameters.AddWithValue("@propietarioId", propietarioId);
                if (!string.IsNullOrEmpty(estado))
                    cmd.Parameters.AddWithValue("@estado", estado);
                if (fechaInicio != null && fechaFin != null)
                {
                    cmd.Parameters.AddWithValue("@fechaInicio", fechaInicio.Value.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@fechaFin", fechaFin.Value.ToString("yyyy-MM-dd"));
                }
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    lista.Add(MapearInmueble(reader));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ObtenerFiltrados error: {ex.Message}");
            }
            return lista;
        }

        // Cuenta inmuebles filtrados por propietario, estado y fechas de ocupación
        public int ContarFiltrados(int? propietarioId, string? estado, DateTime? fechaInicio, DateTime? fechaFin)
        {
            try
            {
                using var conn = Conexion.ObtenerConexion(_connectionString);
                var sql = "SELECT COUNT(*) FROM inmuebles WHERE activo = 1";
                if (propietarioId != null)
                    sql += " AND id_propietario = @propietarioId";
                if (!string.IsNullOrEmpty(estado))
                    sql += " AND estado = @estado";
                if (fechaInicio != null && fechaFin != null)
                {
                    sql += @" AND NOT EXISTS (
                                SELECT 1 FROM contratos c
                                WHERE c.id_inmueble = inmuebles.id_inmueble
                                  AND NOT (c.fecha_fin_original < @fechaInicio OR c.fecha_inicio > @fechaFin)
                            )";
                }
                using var cmd = new MySqlCommand(sql, conn);
                if (propietarioId != null)
                    cmd.Parameters.AddWithValue("@propietarioId", propietarioId);
                if (!string.IsNullOrEmpty(estado))
                    cmd.Parameters.AddWithValue("@estado", estado);
                if (fechaInicio != null && fechaFin != null)
                {
                    cmd.Parameters.AddWithValue("@fechaInicio", fechaInicio.Value.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@fechaFin", fechaFin.Value.ToString("yyyy-MM-dd"));
                }
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ContarFiltrados error: {ex.Message}");
                return 0;
            }
        }

        public List<Inmueble> ObtenerTodos()
        {
            try
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
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new List<Inmueble>();
            }
        }


        public List<Inmueble> ObtenerPaginadosPorEstado(int page, int pageSize, string? estado)
        {
            try
            {
                var lista = new List<Inmueble>();
                using var conn = Conexion.ObtenerConexion(_connectionString);
                string sql = "SELECT * FROM inmuebles WHERE activo = 1";
                if (!string.IsNullOrEmpty(estado))
                    sql += " AND estado = @estado";
                sql += " LIMIT @limit OFFSET @offset";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@limit", pageSize);
                cmd.Parameters.AddWithValue("@offset", (page - 1) * pageSize);
                if (!string.IsNullOrEmpty(estado))
                    cmd.Parameters.AddWithValue("@estado", estado);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    lista.Add(MapearInmueble(reader));
                }
                return lista;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return [];
            }
        }

        public List<Inmueble> ObtenerPaginadosPorPropietarioYEstado(int page, int pageSize, int? propietarioId, string? estado)
        {
            try
            {
                var lista = new List<Inmueble>();
                using var conn = Conexion.ObtenerConexion(_connectionString);
                string sql = "SELECT * FROM inmuebles WHERE activo = 1";
                if (propietarioId != null)
                    sql += " AND id_propietario = @propietarioId";
                if (!string.IsNullOrEmpty(estado))
                    sql += " AND estado = @estado";
                sql += " LIMIT @limit OFFSET @offset";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@limit", pageSize);
                cmd.Parameters.AddWithValue("@offset", (page - 1) * pageSize);
                if (propietarioId != null)
                    cmd.Parameters.AddWithValue("@propietarioId", propietarioId);
                if (!string.IsNullOrEmpty(estado))
                    cmd.Parameters.AddWithValue("@estado", estado);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    lista.Add(MapearInmueble(reader));
                }
                return lista;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return [];
            }
        }
        public int ContarInmuebles()
        {
            try
            {
                using var conn = Conexion.ObtenerConexion(_connectionString);
                using var cmd = new MySqlCommand("SELECT COUNT(*) FROM inmuebles WHERE activo = 1", conn);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 0;
            }
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
            using var conn = Conexion.ObtenerConexion(_connectionString);
            var query = "UPDATE inmuebles SET estado = @estado WHERE id_inmueble = @idInmueble";
            using var command = new MySqlCommand(query, conn);
            command.Parameters.AddWithValue("@estado", nuevoEstado);
            command.Parameters.AddWithValue("@idInmueble", idInmueble);
            command.ExecuteNonQuery();
        }

        public List<Inmueble> BuscarDisponibles(int? idTipo, string uso, int? ambientes, decimal? precioMax, DateTime? fechaInicio, DateTime? fechaFin)
        {
            var lista = new List<Inmueble>();
            try
            {
                using var conn = Conexion.ObtenerConexion(_connectionString);
                var query = @"SELECT * FROM inmuebles WHERE activo = 1"
                    + (idTipo == null ? "" : " AND id_tipo = @idTipo")
                    + (string.IsNullOrEmpty(uso) ? "" : " AND uso = @uso")
                    + (ambientes == null ? "" : " AND cantidad_ambientes = @ambientes")
                    + (precioMax == null ? "" : " AND precio <= @precioMax");

                // Si las fechas están presentes, filtrar inmuebles ocupados en ese rango
                if (fechaInicio != null && fechaFin != null)
                {
                    query += @" AND NOT EXISTS (
                                SELECT 1 FROM contratos c
                                WHERE c.id_inmueble = inmuebles.id_inmueble
                                  AND NOT (c.fecha_fin_original < @fechaInicio OR c.fecha_inicio > @fechaFin)
                            )";
                }

                using var command = new MySqlCommand(query, conn);
                if (idTipo != null)
                    command.Parameters.AddWithValue("@idTipo", idTipo);
                if (!string.IsNullOrEmpty(uso))
                    command.Parameters.AddWithValue("@uso", uso);
                if (ambientes != null)
                    command.Parameters.AddWithValue("@ambientes", ambientes);
                if (precioMax != null)
                    command.Parameters.AddWithValue("@precioMax", precioMax);
                if (fechaInicio != null && fechaFin != null)
                {
                    command.Parameters.AddWithValue("@fechaInicio", fechaInicio.Value.ToString("yyyy-MM-dd"));
                    command.Parameters.AddWithValue("@fechaFin", fechaFin.Value.ToString("yyyy-MM-dd"));
                }
                // Imprimir la query y los valores de los parámetros
                Console.WriteLine($"BuscarDisponibles SQL: {query}");
                foreach (MySqlParameter p in command.Parameters)
                {
                    Console.WriteLine($"  Param: {p.ParameterName} = {p.Value}");
                }
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    lista.Add(MapearInmueble(reader));
                }
            }
            catch (Exception ex)
            {
                // Loguear el error para depuración
                Console.WriteLine($"Error en BuscarDisponibles: {ex.Message}");
            }
            return lista;
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
