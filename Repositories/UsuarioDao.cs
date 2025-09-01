using System;
using System.Data;
using MySql.Data.MySqlClient;
using inmobiliaria.Data;
using inmobiliaria.Models;
using Microsoft.AspNetCore.Identity;

namespace inmobiliaria.Repositories
{
    public class UsuarioDao(string connectionString)
    {
        private readonly string _connectionString = connectionString;

        public Usuario? Login(string email, string contrasena)
        {
            try
            {
                using var conn = Conexion.ObtenerConexion(_connectionString);
                var cmd = new MySqlCommand(@"SELECT * FROM usuarios WHERE email = @email AND activo = TRUE", conn);
                cmd.Parameters.AddWithValue("@email", email);
                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    var usuario = MapearUsuario(reader);
                    var hasher = new PasswordHasher<Usuario>();
                    var resultado = hasher.VerifyHashedPassword(usuario, usuario.Contrasena, contrasena);
                    if (resultado == PasswordVerificationResult.Success)
                    {
                        return usuario;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        public bool CrearUsuario(Usuario usuario)
        {
            try
            {
                using var connection = Conexion.ObtenerConexion(_connectionString);
                var hasher = new PasswordHasher<Usuario>();
                usuario.Contrasena = hasher.HashPassword(usuario, usuario.Contrasena);
                using var command = new MySqlCommand(@"INSERT INTO usuarios 
        (email, contrasena, nombre, apellido, rol, avatar, activo) 
        VALUES (@email, @contrasena, @nombre, @apellido, @rol, @avatar, @activo)", connection);

                command.Parameters.AddWithValue("@email", usuario.Email);
                command.Parameters.AddWithValue("@contrasena", usuario.Contrasena);
                command.Parameters.AddWithValue("@nombre", usuario.Nombre ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@apellido", usuario.Apellido);
                command.Parameters.AddWithValue("@rol", usuario.Rol);
                command.Parameters.AddWithValue("@avatar", usuario.Avatar ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@activo", usuario.Activo);

                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public List<Usuario> ObtenerTodos()
        {
            try
            {
                var lista = new List<Usuario>();
                using var conn = Conexion.ObtenerConexion(_connectionString);
                var cmd = new MySqlCommand("SELECT * FROM usuarios", conn);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    lista.Add(MapearUsuario(reader));
                }
                return lista;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new List<Usuario>();
            }
        }

        public Usuario? ObtenerPorId(int id)
        {
            try
            {
                using var conn = Conexion.ObtenerConexion(_connectionString);
                var cmd = new MySqlCommand("SELECT * FROM usuarios WHERE id_usuario = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return MapearUsuario(reader);
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        public bool ActualizarUsuario(Usuario usuario)
        {
            using var conn = Conexion.ObtenerConexion(_connectionString);
            var cmd = new MySqlCommand(@"UPDATE usuarios SET email = @email, nombre = @nombre, apellido = @apellido, rol = @rol, avatar = @avatar, activo = @activo WHERE id_usuario = @id", conn);
            cmd.Parameters.AddWithValue("@id", usuario.IdUsuario);
            cmd.Parameters.AddWithValue("@email", usuario.Email);
            cmd.Parameters.AddWithValue("@nombre", usuario.Nombre ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@apellido", usuario.Apellido);
            cmd.Parameters.AddWithValue("@rol", usuario.Rol);
            cmd.Parameters.AddWithValue("@avatar", usuario.Avatar ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@activo", usuario.Activo);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool ActualizarContrasena(int idUsuario, string nuevaContrasena)
        {
            using var conn = Conexion.ObtenerConexion(_connectionString);
            var hasher = new PasswordHasher<Usuario>();
            var usuario = ObtenerPorId(idUsuario);
            if (usuario == null) return false;
            var hash = hasher.HashPassword(usuario, nuevaContrasena);
            var cmd = new MySqlCommand("UPDATE usuarios SET contrasena = @contrasena WHERE id_usuario = @id", conn);
            cmd.Parameters.AddWithValue("@id", idUsuario);
            cmd.Parameters.AddWithValue("@contrasena", hash);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool ActualizarAvatar(int idUsuario, string? avatarUrl)
        {
            using var conn = Conexion.ObtenerConexion(_connectionString);
            var cmd = new MySqlCommand("UPDATE usuarios SET avatar = @avatar WHERE id_usuario = @id", conn);
            cmd.Parameters.AddWithValue("@id", idUsuario);
            cmd.Parameters.AddWithValue("@avatar", avatarUrl ?? (object)DBNull.Value);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool EliminarUsuario(int id)
        {
            using var conn = Conexion.ObtenerConexion(_connectionString);
            var cmd = new MySqlCommand("UPDATE usuarios SET activo = 0 WHERE id_usuario = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            return cmd.ExecuteNonQuery() > 0;
        }

        private Usuario MapearUsuario(IDataRecord reader)
        {
            return new Usuario
            {
                IdUsuario = reader.GetInt32(reader.GetOrdinal("id_usuario")),
                Email = reader.GetString(reader.GetOrdinal("email")),
                Contrasena = reader.GetString(reader.GetOrdinal("contrasena")),
                Nombre = reader.GetString(reader.GetOrdinal("nombre")),
                Apellido = reader.GetString(reader.GetOrdinal("apellido")),
                Rol = reader.GetString(reader.GetOrdinal("rol")),
                Avatar = reader.IsDBNull(reader.GetOrdinal("avatar")) ? null : reader.GetString(reader.GetOrdinal("avatar")),
                FechaCreacion = reader.GetDateTime(reader.GetOrdinal("fecha_creacion")),
                FechaActualizacion = reader.GetDateTime(reader.GetOrdinal("fecha_actualizacion")),
                Activo = reader.GetBoolean(reader.GetOrdinal("activo"))
            };
        }
    }
}
