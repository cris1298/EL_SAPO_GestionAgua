using EL_SAPO_GestionAgua.Models;
using EL_SAPO_GestionAgua.Services;
using System;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Windows.Forms;

namespace EL_SAPO_GestionAgua.Services
{
    public static class PDFService
    {
        private static Factura facturaAImprimir;
        private static Usuario clienteAImprimir;

        public static void ImprimirFactura(Factura factura, Usuario cliente)
        {
            facturaAImprimir = factura;
            clienteAImprimir = cliente;

            PrintDocument printDoc = new PrintDocument();
            printDoc.PrintPage += PrintDoc_PrintPage;

            PrintDialog printDialog = new PrintDialog();
            printDialog.Document = printDoc;

            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    printDoc.Print();
                    MessageBox.Show("Factura impresa exitosamente.", "Impresión Completa",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al imprimir: {ex.Message}", "Error de Impresión",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public static void GuardarComoPDF(Factura factura, Usuario cliente, string rutaArchivo)
        {
            facturaAImprimir = factura;
            clienteAImprimir = cliente;

            try
            {
                PrintDocument printDoc = new PrintDocument();
                printDoc.PrintPage += PrintDoc_PrintPage;

                // Configurar para guardar como PDF (requiere Microsoft Print to PDF)
                printDoc.PrinterSettings.PrinterName = "Microsoft Print to PDF";
                printDoc.PrinterSettings.PrintToFile = true;
                printDoc.PrinterSettings.PrintFileName = rutaArchivo;

                printDoc.Print();

                MessageBox.Show($"Factura guardada como PDF en:\n{rutaArchivo}", "PDF Generado",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar PDF: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void PrintDoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Font fontTitulo = new Font("Arial", 16, FontStyle.Bold);
            Font fontSubtitulo = new Font("Arial", 12, FontStyle.Bold);
            Font fontNormal = new Font("Arial", 10, FontStyle.Regular);
            Font fontSmall = new Font("Arial", 8, FontStyle.Regular);

            Brush brushNegro = Brushes.Black;
            Brush brushAzul = new SolidBrush(Color.FromArgb(52, 152, 219));
            Brush brushVerde = new SolidBrush(Color.FromArgb(39, 174, 96));

            int y = 50;
            int margenIzq = 50;
            int margenDer = e.PageBounds.Width - 50;

            // Encabezado de la empresa
            g.DrawString("EL SAPO - GESTIÓN DE AGUA", fontTitulo, brushAzul, margenIzq, y);
            y += 25;
            g.DrawString("Sistema de Facturación de Servicios de Agua", fontNormal, brushNegro, margenIzq, y);
            y += 20;
            g.DrawString("RUC: 20123456789 | Teléfono: (044) 123-4567", fontSmall, brushNegro, margenIzq, y);
            y += 15;
            g.DrawString("Dirección: Av. Principal 123, Cajamarca", fontSmall, brushNegro, margenIzq, y);
            y += 30;

            // Línea separadora
            g.DrawLine(Pens.Black, margenIzq, y, margenDer, y);
            y += 20;

            // Título de la factura
            g.DrawString("FACTURA DE SERVICIO DE AGUA", fontSubtitulo, brushNegro, margenIzq, y);
            y += 30;

            // Información del recibo
            g.DrawString($"Recibo N°: {facturaAImprimir.NumeroRecibo}", fontSubtitulo, brushNegro, margenIzq, y);
            g.DrawString($"Fecha: {facturaAImprimir.FechaEmision:dd/MM/yyyy}", fontNormal, brushNegro, margenDer - 150, y);
            y += 25;

            // Información del cliente
            g.DrawString("DATOS DEL CLIENTE", fontSubtitulo, brushAzul, margenIzq, y);
            y += 25;
            g.DrawString($"Cliente: {clienteAImprimir.Nombres} {clienteAImprimir.Apellidos}", fontNormal, brushNegro, margenIzq, y);
            y += 20;
            g.DrawString($"DNI: {clienteAImprimir.DNI}", fontNormal, brushNegro, margenIzq, y);
            g.DrawString($"Teléfono: {clienteAImprimir.Telefono}", fontNormal, brushNegro, margenIzq + 200, y);
            y += 20;
            g.DrawString($"Dirección: {clienteAImprimir.Direccion}", fontNormal, brushNegro, margenIzq, y);
            y += 30;

            // Detalles del consumo
            g.DrawString("DETALLES DEL CONSUMO", fontSubtitulo, brushAzul, margenIzq, y);
            y += 25;
            g.DrawString($"Período: {facturaAImprimir.Periodo}", fontNormal, brushNegro, margenIzq, y);
            y += 20;
            g.DrawString($"Lectura Anterior: {facturaAImprimir.LecturaAnterior} m³", fontNormal, brushNegro, margenIzq, y);
            g.DrawString($"Lectura Actual: {facturaAImprimir.LecturaActual} m³", fontNormal, brushNegro, margenIzq + 200, y);
            y += 20;
            g.DrawString($"Consumo Total: {facturaAImprimir.Consumo} m³", fontSubtitulo, brushVerde, margenIzq, y);
            y += 30;

            // Desglose de costos
            g.DrawString("DESGLOSE DE COSTOS", fontSubtitulo, brushAzul, margenIzq, y);
            y += 25;

            decimal costoConsumo = 0;
            if (facturaAImprimir.Consumo <= 2)
            {
                g.DrawString("Consumo (0-2 m³): Solo mantenimiento", fontNormal, brushNegro, margenIzq, y);
                y += 20;
            }
            else
            {
                if (facturaAImprimir.Consumo <= 15)
                {
                    costoConsumo = facturaAImprimir.Consumo * 0.50m;
                    g.DrawString($"Consumo ({facturaAImprimir.Consumo} m³ x S/ 0.50): S/ {costoConsumo:N2}", fontNormal, brushNegro, margenIzq, y);
                }
                else
                {
                    decimal costo1 = 15 * 0.50m;
                    decimal costo2 = (facturaAImprimir.Consumo - 15) * 1.50m;
                    costoConsumo = costo1 + costo2;
                    g.DrawString($"Consumo básico (15 m³ x S/ 0.50): S/ {costo1:N2}", fontNormal, brushNegro, margenIzq, y);
                    y += 20;
                    g.DrawString($"Consumo adicional ({facturaAImprimir.Consumo - 15} m³ x S/ 1.50): S/ {costo2:N2}", fontNormal, brushNegro, margenIzq, y);
                }
                y += 20;
            }

            g.DrawString("Mantenimiento: S/ 1.00", fontNormal, brushNegro, margenIzq, y);
            y += 20;

            if (facturaAImprimir.AplicarReposicion)
            {
                g.DrawString("Reposición de medidor: S/ 150.00", fontNormal, brushNegro, margenIzq, y);
                y += 20;
            }

            // Línea separadora
            y += 10;
            g.DrawLine(Pens.Black, margenIzq, y, margenDer, y);
            y += 15;

            // Total
            g.DrawString($"TOTAL A PAGAR: S/ {facturaAImprimir.TotalDeuda:N2}", fontSubtitulo, brushVerde, margenIzq, y);
            y += 30;

            // Fecha de vencimiento
            g.DrawString($"Fecha de Vencimiento: {facturaAImprimir.FechaVencimiento:dd/MM/yyyy}", fontSubtitulo,
                facturaAImprimir.FechaVencimiento < DateTime.Now ? Brushes.Red : brushNegro, margenIzq, y);
            y += 30;

            // Observaciones
            if (!string.IsNullOrEmpty(facturaAImprimir.Observaciones))
            {
                g.DrawString("OBSERVACIONES:", fontSubtitulo, brushAzul, margenIzq, y);
                y += 20;
                g.DrawString(facturaAImprimir.Observaciones, fontNormal, brushNegro, margenIzq, y);
                y += 30;
            }

            // Pie de página
            y = e.PageBounds.Height - 100;
            g.DrawLine(Pens.Black, margenIzq, y, margenDer, y);
            y += 15;
            g.DrawString("¡Gracias por su puntualidad en el pago!", fontNormal, brushAzul, margenIzq, y);
            y += 15;
            g.DrawString($"Factura generada el {DateTime.Now:dd/MM/yyyy HH:mm}", fontSmall, brushNegro, margenIzq, y);
        }
    }
}