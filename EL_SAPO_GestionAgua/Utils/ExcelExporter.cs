using EL_SAPO_GestionAgua.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EL_SAPO_GestionAgua.Utils
{
    public static class ExcelExporter
    {
        public static void ExportarReporteDeudores(List<Factura> facturas)
        {
            string ruta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "ReporteDeudores.csv");
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("DNI,Cliente,Dirección,Periodo,Deuda,Estado,Fecha de Vencimiento");

            foreach (var f in facturas)
            {
                sb.AppendLine($"{f.DNICliente},{f.NombreCompleto},{f.Direccion},{f.Periodo},{f.TotalDeuda},{f.Estado},{f.FechaVencimiento:dd/MM/yyyy}");
            }

            File.WriteAllText(ruta, sb.ToString());
            MessageBox.Show("Reporte exportado como CSV (Excel) en el Escritorio.");
        }
    }
}
