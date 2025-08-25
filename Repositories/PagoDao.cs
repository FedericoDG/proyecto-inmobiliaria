using System.Data;
using MySql.Data.MySqlClient;
using inmobiliaria.Models;
using inmobiliaria.Data;

namespace inmobiliaria.Repositories
{
  public class PagoDao(string connectionString)
  {
    private readonly string _connectionString = connectionString;
    public List<Pago> ObtenerPaginados(int page, int pageSize)
    {
      var lista = new List<Pago>();
      using var conn = Conexion.ObtenerConexion(_connectionString);
      var cmd = new MySqlCommand(@"SELECT * FROM pagos LIMIT @limit OFFSET @offset", conn);
      cmd.Parameters.AddWithValue("@limit", pageSize);
      cmd.Parameters.AddWithValue("@offset", (page - 1) * pageSize);
      using var reader = cmd.ExecuteReader();
      while (reader.Read())
      {
        lista.Add(MapearPago(reader));
      }
      return lista;
    }
    public int ContarPagos()
    {
      using var conn = Conexion.ObtenerConexion(_connectionString);
      var cmd = new MySqlCommand("SELECT COUNT(*) FROM pagos", conn);
      return Convert.ToInt32(cmd.ExecuteScalar());
    }
    public List<Pago> ObtenerTodos()
    {
      var lista = new List<Pago>();
      using var conn = Conexion.ObtenerConexion(_connectionString);
      var cmd = new MySqlCommand("SELECT * FROM pagos", conn);
      using var reader = cmd.ExecuteReader();
      while (reader.Read())
      {
        lista.Add(MapearPago(reader));
      }
      return lista;
    }
    public Pago? ObtenerPorId(int id)
    {
      using var conn = Conexion.ObtenerConexion(_connectionString);
      var cmd = new MySqlCommand("SELECT * FROM pagos WHERE id_pago = @id", conn);
      cmd.Parameters.AddWithValue("@id", id);
      using var reader = cmd.ExecuteReader();
      if (reader.Read())
      {
        return MapearPago(reader);
      }
      return null;
    }
    public bool CrearPago(Pago pago)
    {
      using var conn = Conexion.ObtenerConexion(_connectionString);
      var cmd = new MySqlCommand(@"INSERT INTO pagos (id_contrato, numero_pago, fecha_pago, detalle, importe, estado, id_usuario_creador, id_usuario_anulador) VALUES (@id_contrato, @numero_pago, @fecha_pago, @detalle, @importe, @estado, @id_usuario_creador, @id_usuario_anulador)", conn);
      cmd.Parameters.AddWithValue("@id_contrato", pago.IdContrato);
      cmd.Parameters.AddWithValue("@numero_pago", pago.NumeroPago);
      cmd.Parameters.AddWithValue("@fecha_pago", pago.FechaPago);
      cmd.Parameters.AddWithValue("@detalle", pago.Detalle ?? (object)DBNull.Value);
      cmd.Parameters.AddWithValue("@importe", pago.Importe);
      cmd.Parameters.AddWithValue("@estado", pago.Estado ?? (object)DBNull.Value);
      cmd.Parameters.AddWithValue("@id_usuario_creador", pago.IdUsuarioCreador);
      cmd.Parameters.AddWithValue("@id_usuario_anulador", pago.IdUsuarioAnulador ?? (object)DBNull.Value);
      return cmd.ExecuteNonQuery() > 0;
    }
    public bool ActualizarPago(Pago pago)
    {
      using var conn = Conexion.ObtenerConexion(_connectionString);
      var cmd = new MySqlCommand(@"UPDATE pagos SET id_contrato = @id_contrato, numero_pago = @numero_pago, fecha_pago = @fecha_pago, detalle = @detalle, importe = @importe, estado = @estado, id_usuario_creador = @id_usuario_creador, id_usuario_anulador = @id_usuario_anulador WHERE id_pago = @id_pago", conn);
      cmd.Parameters.AddWithValue("@id_pago", pago.IdPago);
      cmd.Parameters.AddWithValue("@id_contrato", pago.IdContrato);
      cmd.Parameters.AddWithValue("@numero_pago", pago.NumeroPago);
      cmd.Parameters.AddWithValue("@fecha_pago", pago.FechaPago);
      cmd.Parameters.AddWithValue("@detalle", pago.Detalle ?? (object)DBNull.Value);
      cmd.Parameters.AddWithValue("@importe", pago.Importe);
      cmd.Parameters.AddWithValue("@estado", pago.Estado ?? (object)DBNull.Value);
      cmd.Parameters.AddWithValue("@id_usuario_creador", pago.IdUsuarioCreador);
      cmd.Parameters.AddWithValue("@id_usuario_anulador", pago.IdUsuarioAnulador ?? (object)DBNull.Value);
      return cmd.ExecuteNonQuery() > 0;
    }
    public bool EliminarPago(int id)
    {
      using var conn = Conexion.ObtenerConexion(_connectionString);
      var cmd = new MySqlCommand("DELETE FROM pagos WHERE id_pago = @id", conn);
      cmd.Parameters.AddWithValue("@id", id);
      return cmd.ExecuteNonQuery() > 0;
    }
    public void ActualizarEstado(int idPago, int IdUsuarioAnulador, string nuevoEstado)
    {
      using var conn = Conexion.ObtenerConexion(_connectionString);
      var cmd = new MySqlCommand("UPDATE pagos SET estado = @estado, id_usuario_anulador = @idUsuarioAnulador WHERE id_pago = @idPago", conn);
      cmd.Parameters.AddWithValue("@estado", nuevoEstado);
      cmd.Parameters.AddWithValue("@idPago", idPago);
      cmd.Parameters.AddWithValue("@idUsuarioAnulador", IdUsuarioAnulador);
      cmd.ExecuteNonQuery();
    }
    private static Pago MapearPago(IDataRecord reader)
    {
      return new Pago
      {
        IdPago = reader.GetInt32(reader.GetOrdinal("id_pago")),
        IdContrato = reader.GetInt32(reader.GetOrdinal("id_contrato")),
        NumeroPago = reader.GetInt32(reader.GetOrdinal("numero_pago")),
        FechaPago = reader.GetDateTime(reader.GetOrdinal("fecha_pago")),
        Detalle = reader.IsDBNull(reader.GetOrdinal("detalle")) ? null : reader.GetString(reader.GetOrdinal("detalle")),
        Importe = reader.GetDecimal(reader.GetOrdinal("importe")),
        Estado = reader.IsDBNull(reader.GetOrdinal("estado")) ? null : reader.GetString(reader.GetOrdinal("estado")),
        IdUsuarioCreador = reader.GetInt32(reader.GetOrdinal("id_usuario_creador")),
        IdUsuarioAnulador = reader.IsDBNull(reader.GetOrdinal("id_usuario_anulador")) ? null : reader.GetInt32(reader.GetOrdinal("id_usuario_anulador"))
      };
    }
  }
}
