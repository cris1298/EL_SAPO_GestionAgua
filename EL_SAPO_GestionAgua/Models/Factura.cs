using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL_SAPO_GestionAgua.Models
{
    public class Factura
    {
        public int NumeroRecibo { get; set; }
        public string DNICliente { get; set; }
        public string Periodo { get; set; }
        public int Consumo { get; set; }
        public decimal TotalDeuda { get; set; }
        public string Estado { get; set; }
        public int LecturaAnterior { get; set; }
        public int LecturaActual { get; set; }
        public string Observaciones { get; set; }
        public bool AplicarReposicion { get; set; }
        public string NombreCompleto { get; set; }
        public string Direccion { get; set; }
        public DateTime FechaVencimiento { get; set; }

        public DateTime FechaEmision { get; set; }
    }
}

