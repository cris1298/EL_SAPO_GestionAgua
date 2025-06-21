using EL_SAPO_GestionAgua.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EL_SAPO_GestionAgua.Data
{
    public static class FacturaRepository
    {
        // Lista simulando la base de datos
        public static List<Factura> Facturas = new List<Factura>();

        public static void AgregarFactura(Factura factura)
        {
            Facturas.Add(factura);
        }

        public static List<Factura> ObtenerTodas()
        {
            return Facturas.ToList();
        }

        public static List<Factura> ObtenerPorDNI(string dni)
        {
            return Facturas.Where(f => f.DNICliente == dni).ToList();
        }

        public static List<Factura> ObtenerPendientesPorDNI(string dni)
        {
            return Facturas.Where(f => f.DNICliente == dni && f.Estado == "Pendiente").ToList();
        }

        public static Factura ObtenerPorNumero(int numeroRecibo)
        {
            return Facturas.FirstOrDefault(f => f.NumeroRecibo == numeroRecibo);
        }

        public static void MarcarComoPagada(int numeroRecibo)
        {
            var factura = ObtenerPorNumero(numeroRecibo);
            if (factura != null)
            {
                factura.Estado = "Pagado";
            }
        }
    }
}
