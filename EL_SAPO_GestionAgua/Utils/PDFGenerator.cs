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
    public static class PDFGenerator
    {
        public static void ExportarReporteDeudores(List<Factura> facturas)
        {
            string ruta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "ReporteDeudores.pdf");

            using (StreamWriter sw = new StreamWriter(ruta))
            {
                sw.WriteLine("REPORTE DE DEUDORES - EL SAPO");
                sw.WriteLine("Fecha: " + DateTime.Now.ToShortDateString());
                sw.WriteLine("-------------------------------------------------");

                foreach (var f in facturas)
                {
                    sw.WriteLine($"DNI: {f.DNICliente} | Cliente: {f.NombreCompleto} | Deuda: S/ {f.TotalDeuda:N2} | Estado: {f.Estado}");
                }
            }

            MessageBox.Show("PDF exportado en Escritorio.");
        }
    }
}
