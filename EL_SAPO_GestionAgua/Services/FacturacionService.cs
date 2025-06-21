using EL_SAPO_GestionAgua.Data;
using EL_SAPO_GestionAgua.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL_SAPO_GestionAgua.Services
{
    public static class FacturacionService
    {
        public static List<object> ObtenerHistorialPorDNI(string dni)
        {
            var facturas = FacturaRepository.ObtenerPorDNI(dni);
            var resultado = new List<object>();

            foreach (var f in facturas)
            {
                string estado;
                var hoy = DateTime.Now;

                if (f.TotalDeuda <= 0)
                    estado = "Pagado";
                else if (hoy > f.FechaVencimiento)
                    estado = "Mora";
                else
                    estado = "Pendiente";

                resultado.Add(new
                {
                    f.NumeroRecibo,
                    f.Periodo,
                    f.Consumo,
                    f.TotalDeuda,
                    Estado = estado,
                    Fecha = f.FechaEmision.ToShortDateString()
                });
            }

            return resultado;
        }
        public static decimal CalcularCosto(int consumo, bool conMora, bool conReposicion)
        {
            decimal costo = 0;

            if (!conMora)
            {
                if (consumo <= 15)
                    costo = consumo * 0.50m;
                else
                    costo = 15 * 0.50m + (consumo - 15) * 1.50m;
            }
            else
            {
                costo = consumo * 2.00m * 2; // doble consumo, tarifa de mora
            }

            costo += 1.00m; // mantenimiento fijo

            if (conReposicion)
                costo += 150.00m;

            return costo;
        }

        public static Factura GenerarFactura(Factura datos)
        {
            int consumo = datos.LecturaActual - datos.LecturaAnterior;
            bool conMora = DateTime.Now > datos.FechaVencimiento;
            decimal costo = CalcularCosto(consumo, conMora, datos.AplicarReposicion);

            var factura = new Factura
            {
                NumeroRecibo = new Random().Next(1000, 9999), // Simula autogenerado
                DNICliente = datos.DNICliente,
                Periodo = datos.Periodo,
                LecturaAnterior = datos.LecturaAnterior,
                LecturaActual = datos.LecturaActual,
                Consumo = consumo,
                TotalDeuda = costo,
                Estado = "Pendiente",
                FechaEmision = DateTime.Now,
                FechaVencimiento = datos.FechaVencimiento,
                Observaciones = datos.Observaciones,
                AplicarReposicion = datos.AplicarReposicion
            };

            FacturaRepository.AgregarFactura(factura);
            return factura;
        }

    }

}