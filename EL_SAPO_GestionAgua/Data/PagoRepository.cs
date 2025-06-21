using EL_SAPO_GestionAgua.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL_SAPO_GestionAgua.Data
{
    public static class PagoRepository
    {
        private static List<Pago> pagos = new List<Pago>();
        private static int nextId = 1;

        public static void AgregarPago(Pago pago)
        {
            pago.Id = nextId++;
            pagos.Add(pago);
        }

        public static List<Pago> ObtenerPagos()
        {
            return pagos;
        }
    }
}
