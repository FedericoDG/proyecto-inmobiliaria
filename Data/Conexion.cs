using MySql.Data.MySqlClient;

namespace inmobiliaria.Data
{
  public static class Conexion
  {
    public static MySqlConnection ObtenerConexion(string connectionString)
    {
      var conn = new MySqlConnection(connectionString);
      conn.Open();
      return conn;
    }
  }
}
