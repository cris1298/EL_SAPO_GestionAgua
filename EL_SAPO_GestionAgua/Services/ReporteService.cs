using EL_SAPO_GestionAgua.Data;
using EL_SAPO_GestionAgua.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL_SAPO_GestionAgua.Services
{
    public static class ReporteService
    {
        public static List<Factura> ObtenerDeudores(string filtro)
        {
            var todos = FacturaRepository.ObtenerTodas();
            var conDeuda = todos
                .Where(f => f.Estado == "Mora" || f.Estado == "Pendiente" || f.Estado == "Servicio Cortado")
                .ToList();

            if (!string.IsNullOrEmpty(filtro))
            {
                filtro = filtro.ToLower();
                conDeuda = conDeuda.Where(f =>
                    f.DNICliente.Contains(filtro) ||
                    f.NombreCompleto.ToLower().Contains(filtro)).ToList();
            }

            return conDeuda;
        }
    }
}
