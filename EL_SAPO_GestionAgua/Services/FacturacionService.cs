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

        public static decimal CalcularCosto(int consumo, bool conMora, bool conReposicion, decimal costoMantenimiento = 1.00m)
        {
            decimal costo = 0;

            // Si el consumo está entre 0 y 2 m³, solo se cobra mantenimiento
            if (consumo >= 0 && consumo <= 2)
            {
                costo = costoMantenimiento;
            }
            else
            {
                // Cálculo normal de consumo
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

                // Agregar mantenimiento
                costo += costoMantenimiento;
            }

            // Agregar costo de reposición si aplica
            if (conReposicion)
                costo += 150.00m;

            return costo;
        }

        public static bool DebeAplicarMora(string dniCliente)
        {
            var facturasPendientes = FacturaRepository.ObtenerPendientesPorDNI(dniCliente);

            // Si no hay facturas pendientes, no hay mora
            if (facturasPendientes.Count == 0)
                return false;

            // Buscar la factura pendiente más antigua
            var facturaVencidaMasAntigua = facturasPendientes
                .Where(f => f.FechaVencimiento < DateTime.Now)
                .OrderBy(f => f.FechaVencimiento)
                .FirstOrDefault();

            if (facturaVencidaMasAntigua == null)
                return false;

            // Verificar si han pasado 3 meses desde el vencimiento
            var fechaLimiteMora = facturaVencidaMasAntigua.FechaVencimiento.AddMonths(3);
            return DateTime.Now > fechaLimiteMora;
        }

        public static Factura GenerarFactura(Factura datos, decimal costoMantenimiento = 1.00m)
        {
            int consumo = datos.LecturaActual - datos.LecturaAnterior;

            // Verificar si debe aplicar mora (3 meses después del vencimiento)
            bool conMora = DebeAplicarMora(datos.DNICliente);

            decimal costo = CalcularCosto(consumo, conMora, datos.AplicarReposicion, costoMantenimiento);

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
                AplicarReposicion = datos.AplicarReposicion,
                NombreCompleto = datos.NombreCompleto,
                Direccion = datos.Direccion
            };

            FacturaRepository.AgregarFactura(factura);
            return factura;
        }
    }
}