using EL_SAPO_GestionAgua.Data;
using EL_SAPO_GestionAgua.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL_SAPO_GestionAgua.Services
{
    public class PagoService
    {
        // MÉTODO MODIFICADO: Ahora retorna el objeto Pago para poder usarlo en la impresión
        public static Pago RegistrarPago(Factura factura, string metodo)
        {
            var pago = new Pago
            {
                NumeroRecibo = factura.NumeroRecibo,
                DNICliente = factura.DNICliente,
                FechaPago = DateTime.Now,
                MontoPagado = factura.TotalDeuda,
                MetodoPago = metodo
            };

            PagoRepository.AgregarPago(pago);
            FacturaRepository.MarcarComoPagada(factura.NumeroRecibo);

            return pago; // NUEVO: Retorna el pago para poder imprimirlo
        }
    }
}