using EL_SAPO_GestionAgua.Data;
using EL_SAPO_GestionAgua.Models;
using System;

namespace EL_SAPO_GestionAgua.Services
{
    public static class UsuarioService
    {
        /// <summary>
        /// Registra un nuevo usuario y devuelve un mensaje con el monto a pagar.
        /// </summary>
        public static string RegistrarUsuario(Usuario usuario)
        {
            decimal pago = 0;

            switch (usuario.TipoRegistro)
            {
                case "Nuevo Usuario":
                    pago = 2050m;
                    break;
                case "Hijo de Usuario Antiguo":
                    pago = 1050m;
                    break;
                case "Usuario Antiguo":
                    pago = 0m;
                    break;
                default:
                    return "Tipo de registro inválido.";
            }

            // Guarda el usuario (simulado en memoria o con BD real)
            UsuarioRepository.AgregarUsuario(usuario);

            return $"Usuario registrado correctamente.\nMonto a pagar: S/ {pago:N2}";
        }

        /// <summary>
        /// Busca un usuario por su DNI.
        /// </summary>
        public static Usuario BuscarPorDNI(string dni)
        {
            return UsuarioRepository.BuscarPorDNI(dni);
        }

        /// <summary>
        /// Actualiza la última lectura de un cliente registrado.
        /// </summary>
        public static void ActualizarLectura(string dni, int lecturaActual)
        {
            var usuario = UsuarioRepository.BuscarPorDNI(dni);
            if (usuario != null)
            {
                usuario.UltimaLectura = lecturaActual;
                UsuarioRepository.ActualizarUsuario(usuario); // ← Asegúrate de tener este método en tu repositorio
            }
        }
    }
}
