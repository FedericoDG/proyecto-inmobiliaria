using System.Data;
using MySql.Data.MySqlClient;
using inmobiliaria.Models;
using inmobiliaria.Data;

namespace inmobiliaria.Repositories
{
  public class ContratoDao(string connectionString)
  {
    private readonly string _connectionString = connectionString;

    public int ContarContratosVigentesPorFechas(DateTime desde, DateTime hasta)
    {
      try
      {
        using var conn = Conexion.ObtenerConexion(_connectionString);
        var cmd = new MySqlCommand("SELECT COUNT(*) FROM contratos WHERE estado = 'vigente' AND fecha_inicio >= @desde AND fecha_fin_original <= @hasta", conn);
        cmd.Parameters.AddWithValue("@desde", desde);
        cmd.Parameters.AddWithValue("@hasta", hasta);
        return Convert.ToInt32(cmd.ExecuteScalar());
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return 0;
      }
    }

    public int ContarContratosPorVencimiento(DateTime fechaLimite)
    {
      try
      {
        using var conn = Conexion.ObtenerConexion(_connectionString);
        var cmd = new MySqlCommand("SELECT COUNT(*) FROM contratos WHERE estado = 'vigente' AND fecha_fin_original >= @hoy AND fecha_fin_original <= @fechaLimite", conn);
        cmd.Parameters.AddWithValue("@hoy", DateTime.Today);
        cmd.Parameters.AddWithValue("@fechaLimite", fechaLimite);
        return Convert.ToInt32(cmd.ExecuteScalar());
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return 0;
      }
    }

    public int ContarContratosPorInmueble(int inmuebleId)
    {
      try
      {
        using var conn = Conexion.ObtenerConexion(_connectionString);
        var cmd = new MySqlCommand("SELECT COUNT(*) FROM contratos WHERE id_inmueble = @inmuebleId", conn);
        cmd.Parameters.AddWithValue("@inmuebleId", inmuebleId);
        return Convert.ToInt32(cmd.ExecuteScalar());
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return 0;
      }
    }

    public int ContarContratos()
    {
      try
      {
        using var conn = Conexion.ObtenerConexion(_connectionString);
        var cmd = new MySqlCommand("SELECT COUNT(*) FROM contratos", conn);
        return Convert.ToInt32(cmd.ExecuteScalar());
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return 0;
      }
    }

    public List<Contrato> ObtenerPaginadosVigentesPorFechas(int page, int pageSize, DateTime desde, DateTime hasta)
    {
      try
      {
        var lista = new List<Contrato>();
        using var conn = Conexion.ObtenerConexion(_connectionString);
        var cmd = new MySqlCommand(@"SELECT * FROM contratos WHERE estado = 'vigente' AND fecha_inicio >= @desde AND fecha_fin_original <= @hasta LIMIT @limit OFFSET @offset", conn);
        cmd.Parameters.AddWithValue("@desde", desde);
        cmd.Parameters.AddWithValue("@hasta", hasta);
        cmd.Parameters.AddWithValue("@limit", pageSize);
        cmd.Parameters.AddWithValue("@offset", (page - 1) * pageSize);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
          lista.Add(MapearContrato(reader));
        }
        return lista;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return [];
      }
    }

    public List<Contrato> ObtenerContratosPorVencimiento(int page, int pageSize, DateTime fechaLimite)
    {
      try
      {
        var lista = new List<Contrato>();
        using var conn = Conexion.ObtenerConexion(_connectionString);
        var cmd = new MySqlCommand(@"SELECT * FROM contratos WHERE estado = 'vigente' AND fecha_fin_original >= @hoy AND fecha_fin_original <= @fechaLimite LIMIT @limit OFFSET @offset", conn);
        cmd.Parameters.AddWithValue("@hoy", DateTime.Today);
        cmd.Parameters.AddWithValue("@fechaLimite", fechaLimite);
        cmd.Parameters.AddWithValue("@limit", pageSize);
        cmd.Parameters.AddWithValue("@offset", (page - 1) * pageSize);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
          lista.Add(MapearContrato(reader));
        }
        return lista;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return [];
      }
    }

    public List<Contrato> ObtenerPaginadosPorInmueble(int page, int pageSize, int? inmuebleId)
    {
      try
      {
        var lista = new List<Contrato>();
        using var conn = Conexion.ObtenerConexion(_connectionString);
        string sql = "SELECT * FROM contratos";
        if (inmuebleId != null)
          sql += " WHERE id_inmueble = @inmuebleId";
        sql += " LIMIT @limit OFFSET @offset";
        var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@limit", pageSize);
        cmd.Parameters.AddWithValue("@offset", (page - 1) * pageSize);
        if (inmuebleId != null)
          cmd.Parameters.AddWithValue("@inmuebleId", inmuebleId);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
          lista.Add(MapearContrato(reader));
        }
        return lista;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return [];
      }
    }

    public List<Contrato> ObtenerPaginados(int page, int pageSize)
    {
      try
      {
        var lista = new List<Contrato>();
        using var conn = Conexion.ObtenerConexion(_connectionString);
        var cmd = new MySqlCommand(@"SELECT * FROM contratos LIMIT @limit OFFSET @offset", conn);
        cmd.Parameters.AddWithValue("@limit", pageSize);
        cmd.Parameters.AddWithValue("@offset", (page - 1) * pageSize);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
          lista.Add(MapearContrato(reader));
        }
        return lista;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return new List<Contrato>();
      }
    }

    public Contrato? ObtenerPorId(int id)
    {
      try
      {
        using var conn = Conexion.ObtenerConexion(_connectionString);
        var cmd = new MySqlCommand("SELECT * FROM contratos WHERE id_contrato = @id", conn);
        cmd.Parameters.AddWithValue("@id", id);
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
          return MapearContrato(reader);
        }
        return null;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return null;
      }
    }

    public int CrearContrato(Contrato contrato)
    {
      try
      {
        using var conn = Conexion.ObtenerConexion(_connectionString);
        var cmd = new MySqlCommand(@"INSERT INTO contratos (id_inquilino, id_inmueble, id_usuario_creador, id_usuario_finalizador, fecha_inicio, fecha_fin_original, fecha_fin_anticipada, monto_mensual, estado, multa) VALUES (@id_inquilino, @id_inmueble, @id_usuario_creador, @id_usuario_finalizador, @fecha_inicio, @fecha_fin_original, @fecha_fin_anticipada, @monto_mensual, @estado, @multa)", conn);
        cmd.Parameters.AddWithValue("@id_inquilino", contrato.IdInquilino);
        cmd.Parameters.AddWithValue("@id_inmueble", contrato.IdInmueble);
        cmd.Parameters.AddWithValue("@id_usuario_creador", contrato.IdUsuarioCreador);
        cmd.Parameters.AddWithValue("@id_usuario_finalizador", contrato.IdUsuarioFinalizador ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@fecha_inicio", contrato.FechaInicio);
        cmd.Parameters.AddWithValue("@fecha_fin_original", contrato.FechaFinOriginal);
        cmd.Parameters.AddWithValue("@fecha_fin_anticipada", contrato.FechaFinAnticipada ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@monto_mensual", contrato.MontoMensual);
        cmd.Parameters.AddWithValue("@estado", contrato.Estado ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@multa", contrato.Multa ?? (object)DBNull.Value);
        cmd.ExecuteNonQuery();

        // 2. Obtener el ID generado
        var idCmd = new MySqlCommand("SELECT LAST_INSERT_ID();", conn);
        var id = Convert.ToInt32(idCmd.ExecuteScalar());
        return id;

      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return 0;
      }
    }

    public bool ActualizarContrato(Contrato contrato)
    {
      try
      {
        using var conn = Conexion.ObtenerConexion(_connectionString);
        var cmd = new MySqlCommand(@"UPDATE contratos SET id_inquilino = @id_inquilino, id_inmueble = @id_inmueble, id_usuario_creador = @id_usuario_creador, id_usuario_finalizador = @id_usuario_finalizador, fecha_inicio = @fecha_inicio, fecha_fin_original = @fecha_fin_original, fecha_fin_anticipada = @fecha_fin_anticipada, monto_mensual = @monto_mensual, estado = @estado, multa = @multa WHERE id_contrato = @id_contrato", conn);
        cmd.Parameters.AddWithValue("@id_contrato", contrato.IdContrato);
        cmd.Parameters.AddWithValue("@id_inquilino", contrato.IdInquilino);
        cmd.Parameters.AddWithValue("@id_inmueble", contrato.IdInmueble);
        cmd.Parameters.AddWithValue("@id_usuario_creador", contrato.IdUsuarioCreador);
        cmd.Parameters.AddWithValue("@id_usuario_finalizador", contrato.IdUsuarioFinalizador ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@fecha_inicio", contrato.FechaInicio);
        cmd.Parameters.AddWithValue("@fecha_fin_original", contrato.FechaFinOriginal);
        cmd.Parameters.AddWithValue("@fecha_fin_anticipada", contrato.FechaFinAnticipada ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@monto_mensual", contrato.MontoMensual);
        cmd.Parameters.AddWithValue("@estado", contrato.Estado ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@multa", contrato.Multa ?? (object)DBNull.Value);
        return cmd.ExecuteNonQuery() > 0;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return false;
      }
    }

    public bool RescindirContrato(Contrato contrato)
    {
      try
      {
        using var conn = Conexion.ObtenerConexion(_connectionString);
        var cmd = new MySqlCommand(@"UPDATE contratos SET fecha_fin_anticipada = @fecha_fin_anticipada, monto_mensual = @monto_mensual, multa = @multa, estado = @estado, id_usuario_finalizador = @id_usuario_finalizador WHERE id_contrato = @id_contrato", conn);
        cmd.Parameters.AddWithValue("@fecha_fin_anticipada", contrato.FechaFinAnticipada);
        cmd.Parameters.AddWithValue("@monto_mensual", contrato.MontoMensual);
        cmd.Parameters.AddWithValue("@multa", contrato.Multa ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@estado", contrato.Estado);
        cmd.Parameters.AddWithValue("@id_usuario_finalizador", contrato.IdUsuarioFinalizador ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@id_contrato", contrato.IdContrato);
        return cmd.ExecuteNonQuery() > 0;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return false;
      }
    }

    private static Contrato MapearContrato(IDataRecord reader)
    {
      return new Contrato
      {
        IdContrato = reader.GetInt32(reader.GetOrdinal("id_contrato")),
        IdInquilino = reader.GetInt32(reader.GetOrdinal("id_inquilino")),
        IdInmueble = reader.GetInt32(reader.GetOrdinal("id_inmueble")),
        IdUsuarioCreador = reader.GetInt32(reader.GetOrdinal("id_usuario_creador")),
        IdUsuarioFinalizador = reader.IsDBNull(reader.GetOrdinal("id_usuario_finalizador")) ? null : reader.GetInt32(reader.GetOrdinal("id_usuario_finalizador")),
        FechaInicio = reader.GetDateTime(reader.GetOrdinal("fecha_inicio")),
        FechaFinOriginal = reader.GetDateTime(reader.GetOrdinal("fecha_fin_original")),
        FechaFinAnticipada = reader.IsDBNull(reader.GetOrdinal("fecha_fin_anticipada")) ? null : reader.GetDateTime(reader.GetOrdinal("fecha_fin_anticipada")),
        MontoMensual = reader.GetDecimal(reader.GetOrdinal("monto_mensual")),
        Estado = reader.IsDBNull(reader.GetOrdinal("estado")) ? null : reader.GetString(reader.GetOrdinal("estado")),
        Multa = reader.IsDBNull(reader.GetOrdinal("multa")) ? null : reader.GetDecimal(reader.GetOrdinal("multa"))
      };
    }

    // Para obtener datos relacionados
    public List<dynamic> ObtenerInquilinos()
    {
      try
      {
        var lista = new List<dynamic>();
        using var conn = Conexion.ObtenerConexion(_connectionString);
        var cmd = new MySqlCommand("SELECT id_inquilino, nombre, apellido, dni FROM inquilinos WHERE activo = 1", conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
          lista.Add(new
          {
            IdInquilino = reader.GetInt32(reader.GetOrdinal("id_inquilino")),
            Nombre = reader.GetString(reader.GetOrdinal("nombre")),
            Apellido = reader.GetString(reader.GetOrdinal("apellido")),
            Dni = reader.GetString(reader.GetOrdinal("dni"))
          });
        }
        return lista;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return new List<dynamic>();
      }
    }
    public List<dynamic> ObtenerInmuebles()
    {
      try
      {
        var lista = new List<dynamic>();
        using var conn = Conexion.ObtenerConexion(_connectionString);
        var cmd = new MySqlCommand(@"SELECT i.id_inmueble, i.direccion, t.nombre AS tipo_nombre FROM inmuebles i JOIN tipos_inmueble t ON i.id_tipo = t.id_tipo WHERE i.activo = 1", conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
          lista.Add(new
          {
            IdInmueble = reader.GetInt32(reader.GetOrdinal("id_inmueble")),
            Direccion = reader.GetString(reader.GetOrdinal("direccion")),
            TipoNombre = reader.GetString(reader.GetOrdinal("tipo_nombre"))
          });
        }
        return lista;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return new List<dynamic>();
      }
    }
    public List<dynamic> ObtenerUsuarios()
    {
      try
      {
        var lista = new List<dynamic>();
        using var conn = Conexion.ObtenerConexion(_connectionString);
        var cmd = new MySqlCommand("SELECT id_usuario, nombre, apellido, email FROM usuarios WHERE activo = 1", conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
          lista.Add(new
          {
            IdUsuario = reader.GetInt32(reader.GetOrdinal("id_usuario")),
            Nombre = reader.GetString(reader.GetOrdinal("nombre")),
            Apellido = reader.GetString(reader.GetOrdinal("apellido")),
            Email = reader.GetString(reader.GetOrdinal("email"))
          });
        }
        return lista;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return new List<dynamic>();
      }
    }
  }
}
