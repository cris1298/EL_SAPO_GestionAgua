using EL_SAPO_GestionAgua.Models;
using System.Collections.Generic;
using System.Linq;

namespace EL_SAPO_GestionAgua.Data
{
    public static class UsuarioRepository
    {
        // Base de datos en memoria
        private static List<Usuario> Usuarios = new List<Usuario>();

        public static void AgregarUsuario(Usuario usuario)
        {
            Usuarios.Add(usuario);
        }

        public static Usuario BuscarPorDNI(string dni)
        {
            return Usuarios.FirstOrDefault(u => u.DNI == dni);
        }

        /// <summary>
        /// Reemplaza por completo un usuario existente con nuevos datos.
        /// </summary>
        public static void ActualizarUsuario(Usuario usuarioActualizado)
        {
            var index = Usuarios.FindIndex(u => u.DNI == usuarioActualizado.DNI);
            if (index != -1)
            {
                Usuarios[index] = usuarioActualizado;
            }
        }

        /// <summary>
        /// Actualiza solo la última lectura del usuario (opcional, también se puede usar desde UsuarioService).
        /// </summary>
        public static void ActualizarLectura(string dni, int lecturaActual)
        {
            var usuario = BuscarPorDNI(dni);
            if (usuario != null)
            {
                usuario.UltimaLectura = lecturaActual;
            }
        }

        /// <summary>
        /// Devuelve todos los usuarios (por si necesitas mostrarlos).
        /// </summary>
        public static List<Usuario> ObtenerTodos()
        {
            return Usuarios;
        }
    }
}
