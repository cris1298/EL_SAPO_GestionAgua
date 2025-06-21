using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL_SAPO_GestionAgua.Models
{
    public class Pago
    {
        public int Id { get; set; }
        public int NumeroRecibo { get; set; }
        public string DNICliente { get; set; }
        public DateTime FechaPago { get; set; }
        public decimal MontoPagado { get; set; }
        public string MetodoPago { get; set; }
    }
}
