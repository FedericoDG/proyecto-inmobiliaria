using System.Data;
using MySql.Data.MySqlClient;
using inmobiliaria.Models;
using inmobiliaria.Data;

namespace inmobiliaria.Repositories
{
  public class PagoDao
  {

    public List<dynamic> BuscarContratosVigentesPorDni(string dni)
    {
      var resultados = new List<dynamic>();
      try
      {
        using var conn = Conexion.ObtenerConexion(_connectionString);
        // Buscar el inquilino por DNI
        var cmdInq = new MySqlCommand("SELECT id_inquilino FROM inquilinos WHERE dni LIKE @dni", conn);
        cmdInq.Parameters.AddWithValue("@dni", "%" + dni + "%");
        var idInquilino = cmdInq.ExecuteScalar();

        if (idInquilino == null) return resultados;

        // Buscar contratos vigentes para ese inquilino
        var cmd = new MySqlCommand(@"SELECT c.id_contrato, i.direccion, c.fecha_fin_original FROM contratos c INNER JOIN inmuebles i ON c.id_inmueble = i.id_inmueble WHERE c.estado = 'vigente' AND c.id_inquilino = @idInquilino", conn);
        cmd.Parameters.AddWithValue("@idInquilino", idInquilino);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
          resultados.Add(new
          {
            idContrato = reader.GetInt32("id_contrato"),
            direccion = reader.GetString("direccion"),
            fechaFinOriginal = reader.GetDateTime("fecha_fin_original").ToString("yyyy-MM-dd")
          });
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
      }
      return resultados;
    }
    private readonly string _connectionString;
    public PagoDao(string connectionString)
    {
      _connectionString = connectionString;
    }

    public List<Pago> ObtenerPaginadosPorEstado(int page, int pageSize, string? estado)
    {
      try
      {
        var lista = new List<Pago>();
        using var conn = Conexion.ObtenerConexion(_connectionString);
        var query = "SELECT * FROM pagos";
        if (!string.IsNullOrEmpty(estado))
        {
          query += " WHERE estado = @estado";
        }
        query += " LIMIT @limit OFFSET @offset";
        var cmd = new MySqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@limit", pageSize);
        cmd.Parameters.AddWithValue("@offset", (page - 1) * pageSize);
        if (!string.IsNullOrEmpty(estado))
        {
          cmd.Parameters.AddWithValue("@estado", estado);
        }
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
          lista.Add(MapearPago(reader));
        }
        return lista;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return new List<Pago>();
      }
    }

    public int ContarPagosPorEstado(string? estado)
    {
      try
      {
        using var conn = Conexion.ObtenerConexion(_connectionString);
        var query = "SELECT COUNT(*) FROM pagos";
        if (!string.IsNullOrEmpty(estado))
        {
          query += " WHERE estado = @estado";
        }
        var cmd = new MySqlCommand(query, conn);
        if (!string.IsNullOrEmpty(estado))
        {
          cmd.Parameters.AddWithValue("@estado", estado);
        }
        return Convert.ToInt32(cmd.ExecuteScalar());
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return 0;
      }
    }

    public int ContarPagos()
    {
      try
      {
        using var conn = Conexion.ObtenerConexion(_connectionString);
        var cmd = new MySqlCommand("SELECT COUNT(*) FROM pagos", conn);
        return Convert.ToInt32(cmd.ExecuteScalar());
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return 0;
      }
    }
    public List<Pago> ObtenerTodos()
    {
      try
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
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return [];
      }
    }
    public Pago? ObtenerPorId(int id)
    {
      try
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
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return null;
      }
    }
    public void CrearPagosPendientes(int idContrato, DateTime fechaInicio, int cantidadPagos, decimal montoMensual, int idUsuarioCreador)
    {
      try
      {
        using var conn = Conexion.ObtenerConexion(_connectionString);
        var fechaVencimiento = fechaInicio.AddDays(10); // Primer vencimiento 10 dias despues del inicio del contrato
        for (int i = 1; i <= cantidadPagos; i++)
        {
          var cmd = new MySqlCommand(@"INSERT INTO pagos (id_contrato, numero_pago, fecha_vencimiento, detalle, importe, estado, id_usuario_creador) VALUES (@id_contrato, @numero_pago, @fecha_vencimiento, @detalle, @importe, @estado, @id_usuario_creador)", conn);
          cmd.Parameters.AddWithValue("@id_contrato", idContrato);
          cmd.Parameters.AddWithValue("@numero_pago", i);
          cmd.Parameters.AddWithValue("@fecha_vencimiento", fechaVencimiento);
          cmd.Parameters.AddWithValue("@detalle", DBNull.Value);
          cmd.Parameters.AddWithValue("@importe", montoMensual);
          cmd.Parameters.AddWithValue("@estado", "pendiente");
          cmd.Parameters.AddWithValue("@id_usuario_creador", idUsuarioCreador);
          cmd.ExecuteNonQuery();

          // Para el siguiente vencimiento, avanzar exactamente un mes
          fechaVencimiento = fechaVencimiento.AddMonths(1);
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
      }
    }
    public bool CrearPago(Pago pago)
    {
      try
      {
        using var conn = Conexion.ObtenerConexion(_connectionString);
        var cmd = new MySqlCommand(@"INSERT INTO pagos (id_contrato, numero_pago, fecha_vencimiento, detalle, importe, estado, id_usuario_creador, id_usuario_anulador) VALUES (@id_contrato, @numero_pago, @fecha_vencimiento, @detalle, @importe, @estado, @id_usuario_creador, @id_usuario_anulador)", conn);
        cmd.Parameters.AddWithValue("@id_contrato", pago.IdContrato);
        cmd.Parameters.AddWithValue("@numero_pago", pago.NumeroPago);
        cmd.Parameters.AddWithValue("@fecha_vencimiento", pago.FechaVencimiento);
        cmd.Parameters.AddWithValue("@detalle", pago.Detalle ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@importe", pago.Importe);
        cmd.Parameters.AddWithValue("@estado", pago.Estado ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@id_usuario_creador", pago.IdUsuarioCreador);
        cmd.Parameters.AddWithValue("@id_usuario_anulador", pago.IdUsuarioAnulador ?? (object)DBNull.Value);
        return cmd.ExecuteNonQuery() > 0;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return false;
      }
    }
    public bool ActualizarPago(Pago pago)
    {
      try
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
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return false;
      }
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
    public void CancelarPagosPorContrato(int idContrato, DateTime fechaCancelacion)
    {
      using var conn = Conexion.ObtenerConexion(_connectionString);
      var cmd = new MySqlCommand("UPDATE pagos SET estado = 'anulado' WHERE id_contrato = @idContrato AND fecha_vencimiento >= @fechaVencimiento AND estado = 'pendiente'", conn);
      cmd.Parameters.AddWithValue("@idContrato", idContrato);
      cmd.Parameters.AddWithValue("@fechaCancelacion", fechaCancelacion);
      cmd.Parameters.AddWithValue("@fechaVencimiento", fechaCancelacion);
      cmd.ExecuteNonQuery();
    }
    private static Pago MapearPago(IDataRecord reader)
    {
      return new Pago
      {
        IdPago = reader.GetInt32(reader.GetOrdinal("id_pago")),
        IdContrato = reader.GetInt32(reader.GetOrdinal("id_contrato")),
        NumeroPago = reader.GetInt32(reader.GetOrdinal("numero_pago")),
        FechaVencimiento = reader.GetDateTime(reader.GetOrdinal("fecha_vencimiento")),
        FechaPago = reader.IsDBNull(reader.GetOrdinal("fecha_pago")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("fecha_pago")),
        Detalle = reader.IsDBNull(reader.GetOrdinal("detalle")) ? null : reader.GetString(reader.GetOrdinal("detalle")),
        Importe = reader.GetDecimal(reader.GetOrdinal("importe")),
        Estado = reader.IsDBNull(reader.GetOrdinal("estado")) ? null : reader.GetString(reader.GetOrdinal("estado")),
        IdUsuarioCreador = reader.GetInt32(reader.GetOrdinal("id_usuario_creador")),
        IdUsuarioAnulador = reader.IsDBNull(reader.GetOrdinal("id_usuario_anulador")) ? null : reader.GetInt32(reader.GetOrdinal("id_usuario_anulador"))
      };
    }
  }
}
