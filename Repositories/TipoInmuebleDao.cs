using System.Data;
using MySql.Data.MySqlClient;
using inmobiliaria.Models;
using inmobiliaria.Data;

namespace inmobiliaria.Repositories
{
  public class TipoInmuebleDao(string connectionString)
  {
    private readonly string _connectionString = connectionString;

    public List<TipoInmueble> ObtenerTodos()
    {
      var lista = new List<TipoInmueble>();
      using var conn = Conexion.ObtenerConexion(_connectionString);
      using var cmd = new MySqlCommand("SELECT * FROM tipos_inmueble WHERE activo = 1", conn);
      using var reader = cmd.ExecuteReader();
      while (reader.Read())
      {
        lista.Add(MapearTipo(reader));
      }
      return lista;
    }

    public TipoInmueble? ObtenerPorId(int id)
    {
      using var conn = Conexion.ObtenerConexion(_connectionString);
      using var cmd = new MySqlCommand("SELECT * FROM tipos_inmueble WHERE id_tipo = @id", conn);
      cmd.Parameters.AddWithValue("@id", id);
      using var reader = cmd.ExecuteReader();
      if (reader.Read())
      {
        return MapearTipo(reader);
      }
      return null;
    }

    public bool CrearTipo(TipoInmueble tipo)
    {
      using var conn = Conexion.ObtenerConexion(_connectionString);
      using var cmd = new MySqlCommand(@"INSERT INTO tipos_inmueble (nombre, descripcion) VALUES (@nombre, @descripcion)", conn);
      cmd.Parameters.AddWithValue("@nombre", tipo.Nombre);
      cmd.Parameters.AddWithValue("@descripcion", tipo.Descripcion ?? (object)DBNull.Value);
      return cmd.ExecuteNonQuery() > 0;
    }

    public bool ActualizarTipo(TipoInmueble tipo)
    {
      using var conn = Conexion.ObtenerConexion(_connectionString);
      using var cmd = new MySqlCommand(@"UPDATE tipos_inmueble SET nombre = @nombre, descripcion = @descripcion WHERE id_tipo = @id", conn);
      cmd.Parameters.AddWithValue("@id", tipo.IdTipo);
      cmd.Parameters.AddWithValue("@nombre", tipo.Nombre);
      cmd.Parameters.AddWithValue("@descripcion", tipo.Descripcion ?? (object)DBNull.Value);
      return cmd.ExecuteNonQuery() > 0;
    }

    public bool EliminarTipo(int id)
    {
      using var conn = Conexion.ObtenerConexion(_connectionString);
      using var cmd = new MySqlCommand("UPDATE tipos_inmueble SET activo = 0 WHERE id_tipo = @id", conn);
      cmd.Parameters.AddWithValue("@id", id);
      return cmd.ExecuteNonQuery() > 0;
    }

    private static TipoInmueble MapearTipo(IDataRecord reader)
    {
      return new TipoInmueble
      {
        IdTipo = reader.GetInt32(reader.GetOrdinal("id_tipo")),
        Nombre = reader.GetString(reader.GetOrdinal("nombre")),
        Descripcion = reader.IsDBNull(reader.GetOrdinal("descripcion")) ? null : reader.GetString(reader.GetOrdinal("descripcion"))
      };
    }
  }
}
