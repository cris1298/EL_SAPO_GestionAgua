using EL_SAPO_GestionAgua.Data;
using EL_SAPO_GestionAgua.Models;
using EL_SAPO_GestionAgua.Services;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace EL_SAPO_GestionAgua.Forms
{
    public partial class GestionPagosForm : Form
    {
        private TextBox txtDNI;
        private ListBox lstFacturas;
        private Button btnBuscar, btnPagar, btnImprimirRecibo;
        private Label lblSeleccionado, lblTotalDeuda, lblClienteInfo;
        private GroupBox grpBusqueda, grpFacturas, grpPago;
        private Panel pnlResumen;
        private List<Factura> facturasPendientes = new List<Factura>();
        private Pago ultimoPagoRealizado = null;

        public GestionPagosForm()
        {
            InitializeComponent();
            InitUI();
        }

        private void InitUI()
        {
            // Configuración principal del formulario
            this.Text = "💰 Gestión de Pagos";
            this.Size = new Size(650, 750); // Aumentado para el nuevo botón
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(236, 240, 241);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, FontStyle.Regular);

            // Panel principal con padding
            Panel mainPanel = new Panel()
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                BackColor = Color.Transparent
            };
            this.Controls.Add(mainPanel);

            int currentY = 0;
            const int groupSpacing = 20;

            // Título principal
            Label titleLabel = new Label()
            {
                Text = "GESTIÓN DE PAGOS",
                Font = new System.Drawing.Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                AutoSize = true,
                Location = new Point(0, currentY)
            };
            mainPanel.Controls.Add(titleLabel);

            Label subtitleLabel = new Label()
            {
                Text = "Consulta y registra pagos de facturas pendientes",
                Font = new System.Drawing.Font("Segoe UI", 10F, FontStyle.Regular),
                ForeColor = Color.FromArgb(127, 140, 141),
                AutoSize = true,
                Location = new Point(0, currentY + 25)
            };
            mainPanel.Controls.Add(subtitleLabel);
            currentY += 60;

            // Grupo: Búsqueda de Cliente
            grpBusqueda = CreateStyledGroupBox("🔍 Búsqueda de Cliente", currentY, 590);
            mainPanel.Controls.Add(grpBusqueda);

            // DNI y búsqueda
            Label lblDNI = CreateStyledLabel("DNI del Cliente:", 20, 25);
            grpBusqueda.Controls.Add(lblDNI);

            txtDNI = CreateStyledTextBox(20, 45, 200);
            txtDNI.Font = new System.Drawing.Font("Consolas", 11F, FontStyle.Bold);
            txtDNI.TextAlign = HorizontalAlignment.Center;
            grpBusqueda.Controls.Add(txtDNI);

            btnBuscar = CreateStyledButton("🔍 Buscar Deudas", 235, 44, 140, 30);
            btnBuscar.BackColor = Color.FromArgb(41, 128, 185);
            btnBuscar.Click += BtnBuscar_Click;
            grpBusqueda.Controls.Add(btnBuscar);

            // Información del cliente
            lblClienteInfo = new Label()
            {
                Text = "Ingrese un DNI para consultar facturas pendientes",
                Location = new Point(20, 85),
                Size = new Size(550, 20),
                ForeColor = Color.FromArgb(127, 140, 141),
                Font = new System.Drawing.Font("Segoe UI", 9F, FontStyle.Italic)
            };
            grpBusqueda.Controls.Add(lblClienteInfo);

            currentY += grpBusqueda.Height + groupSpacing;

            // Grupo: Facturas Pendientes
            grpFacturas = CreateStyledGroupBox("📋 Facturas Pendientes", currentY, 590);
            mainPanel.Controls.Add(grpFacturas);

            // Lista de facturas con estilo personalizado
            lstFacturas = new ListBox()
            {
                Location = new Point(20, 25),
                Size = new Size(550, 200),
                Font = new System.Drawing.Font("Consolas", 10F),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                SelectionMode = SelectionMode.One,
                ItemHeight = 25
            };
            lstFacturas.DrawMode = DrawMode.OwnerDrawFixed;
            lstFacturas.DrawItem += LstFacturas_DrawItem;
            lstFacturas.SelectedIndexChanged += LstFacturas_SelectedIndexChanged;
            grpFacturas.Controls.Add(lstFacturas);

            currentY += grpFacturas.Height + groupSpacing;

            // Panel de resumen
            pnlResumen = new Panel()
            {
                Location = new Point(0, currentY),
                Size = new Size(590, 80),
                BackColor = Color.FromArgb(52, 152, 219),
                Visible = false
            };
            pnlResumen.Paint += PnlResumen_Paint;
            mainPanel.Controls.Add(pnlResumen);

            lblSeleccionado = new Label()
            {
                Text = "Ninguna factura seleccionada",
                Location = new Point(20, 15),
                Size = new Size(550, 20),
                ForeColor = Color.White,
                Font = new System.Drawing.Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = Color.Transparent
            };
            pnlResumen.Controls.Add(lblSeleccionado);

            lblTotalDeuda = new Label()
            {
                Text = "Total: S/ 0.00",
                Location = new Point(20, 45),
                Size = new Size(200, 25),
                ForeColor = Color.White,
                Font = new System.Drawing.Font("Segoe UI", 14F, FontStyle.Bold),
                BackColor = Color.Transparent
            };
            pnlResumen.Controls.Add(lblTotalDeuda);

            currentY += pnlResumen.Height + groupSpacing;

            // Grupo: Registro de Pago (AUMENTADO LA ALTURA)
            grpPago = CreateStyledGroupBox("💳 Registro de Pago", currentY, 590);
            grpPago.Height = 160; // Aumentado para acomodar el nuevo botón
            mainPanel.Controls.Add(grpPago);

            // Instrucciones
            Label lblInstrucciones = new Label()
            {
                Text = "Seleccione una factura de la lista para proceder con el pago",
                Location = new Point(20, 25),
                Size = new Size(550, 20),
                ForeColor = Color.FromArgb(127, 140, 141),
                Font = new System.Drawing.Font("Segoe UI", 9F, FontStyle.Italic)
            };
            grpPago.Controls.Add(lblInstrucciones);

            // Botón de pago
            btnPagar = new Button()
            {
                Text = "💰 REGISTRAR PAGO",
                Location = new Point(20, 55),
                Size = new Size(550, 45),
                Font = new System.Drawing.Font("Segoe UI", 12F, FontStyle.Bold),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false,
                Enabled = false
            };
            btnPagar.FlatAppearance.BorderSize = 0;
            btnPagar.FlatAppearance.MouseOverBackColor = Color.FromArgb(39, 174, 96);
            btnPagar.Click += BtnPagar_Click;
            grpPago.Controls.Add(btnPagar);

            // NUEVO: Botón de imprimir recibo
            btnImprimirRecibo = new Button()
            {
                Text = "🖨️ IMPRIMIR RECIBO DE PAGO",
                Location = new Point(20, 110),
                Size = new Size(550, 35),
                Font = new System.Drawing.Font("Segoe UI", 11F, FontStyle.Bold),
                BackColor = Color.FromArgb(155, 89, 182),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false,
                Enabled = false
            };
            btnImprimirRecibo.FlatAppearance.BorderSize = 0;
            btnImprimirRecibo.FlatAppearance.MouseOverBackColor = Color.FromArgb(142, 68, 173);
            btnImprimirRecibo.Click += BtnImprimirRecibo_Click;
            grpPago.Controls.Add(btnImprimirRecibo);
        }

        private GroupBox CreateStyledGroupBox(string text, int y, int width)
        {
            int height = 120;
            if (text.Contains("Facturas")) height = 240;
            if (text.Contains("Pago")) height = 160; // Aumentado para el nuevo botón

            return new GroupBox()
            {
                Text = text,
                Location = new Point(0, y),
                Size = new Size(width, height),
                Font = new System.Drawing.Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                BackColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
        }

        private Label CreateStyledLabel(string text, int x, int y)
        {
            return new Label()
            {
                Text = text,
                Location = new Point(x, y),
                AutoSize = true,
                Font = new System.Drawing.Font("Segoe UI", 9F, FontStyle.Regular),
                ForeColor = Color.FromArgb(52, 73, 94)
            };
        }

        private TextBox CreateStyledTextBox(int x, int y, int width)
        {
            return new TextBox()
            {
                Location = new Point(x, y),
                Size = new Size(width, 30),
                Font = new System.Drawing.Font("Segoe UI", 10F),
                BorderStyle = BorderStyle.FixedSingle
            };
        }

        private Button CreateStyledButton(string text, int x, int y, int width, int height)
        {
            return new Button()
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(width, height),
                Font = new System.Drawing.Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false
            };
        }

        private void LstFacturas_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            e.DrawBackground();

            // Colores alternados
            Color backColor = e.Index % 2 == 0 ? Color.FromArgb(248, 249, 250) : Color.White;
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                backColor = Color.FromArgb(52, 152, 219);
            }

            using (SolidBrush backBrush = new SolidBrush(backColor))
            {
                e.Graphics.FillRectangle(backBrush, e.Bounds);
            }

            // Texto
            Color textColor = ((e.State & DrawItemState.Selected) == DrawItemState.Selected) ? Color.White : Color.FromArgb(44, 62, 80);
            using (SolidBrush textBrush = new SolidBrush(textColor))
            {
                string text = lstFacturas.Items[e.Index].ToString();
                e.Graphics.DrawString(text, e.Font, textBrush, e.Bounds.X + 10, e.Bounds.Y + 5);
            }

            // Línea separadora
            if (e.Index < lstFacturas.Items.Count - 1)
            {
                using (Pen pen = new Pen(Color.FromArgb(220, 221, 222)))
                {
                    e.Graphics.DrawLine(pen, e.Bounds.X, e.Bounds.Bottom - 1, e.Bounds.Right, e.Bounds.Bottom - 1);
                }
            }

            e.DrawFocusRectangle();
        }

        private void PnlResumen_Paint(object sender, PaintEventArgs e)
        {
            // Borde redondeado y sombra
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(52, 152, 219)))
            {
                e.Graphics.FillRectangle(brush, pnlResumen.ClientRectangle);
            }

            using (Pen pen = new Pen(Color.FromArgb(41, 128, 185), 2))
            {
                e.Graphics.DrawRectangle(pen, 0, 0, pnlResumen.Width - 1, pnlResumen.Height - 1);
            }
        }

        // ===== LÓGICA ORIGINAL CON MODIFICACIONES PARA PDF =====

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            string dni = txtDNI.Text.Trim();

            if (string.IsNullOrEmpty(dni))
            {
                MessageBox.Show("Por favor ingrese un DNI válido.", "Datos Incompletos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            lstFacturas.Items.Clear();
            facturasPendientes = FacturaRepository.ObtenerPendientesPorDNI(dni);

            foreach (var factura in facturasPendientes)
            {
                lstFacturas.Items.Add($"Recibo {factura.NumeroRecibo} - S/ {factura.TotalDeuda:N2} - {factura.Periodo}");
            }

            if (facturasPendientes.Count == 0)
            {
                lblClienteInfo.Text = "❌ No hay facturas pendientes para este cliente";
                lblClienteInfo.ForeColor = Color.FromArgb(231, 76, 60);
                pnlResumen.Visible = false;
                btnPagar.Enabled = false;
                // Solo deshabilitar si no hay pago reciente
                if (ultimoPagoRealizado == null)
                    btnImprimirRecibo.Enabled = false;
                MessageBox.Show("No hay facturas pendientes.", "Sin Resultados", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                lblClienteInfo.Text = $"✅ Se encontraron {facturasPendientes.Count} factura(s) pendiente(s)";
                lblClienteInfo.ForeColor = Color.FromArgb(39, 174, 96);

                // Calcular total de deuda
                decimal totalDeuda = facturasPendientes.Sum(f => f.TotalDeuda);
                lblTotalDeuda.Text = $"Deuda Total: S/ {totalDeuda:N2}";
            }
        }

        private void LstFacturas_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstFacturas.SelectedIndex >= 0)
            {
                var facturaSeleccionada = facturasPendientes[lstFacturas.SelectedIndex];
                lblSeleccionado.Text = $"Factura Seleccionada: Recibo {facturaSeleccionada.NumeroRecibo} - {facturaSeleccionada.Periodo}";
                lblTotalDeuda.Text = $"Monto a Pagar: S/ {facturaSeleccionada.TotalDeuda:N2}";
                pnlResumen.Visible = true;
                btnPagar.Enabled = true;

                // Cambiar color del panel según el monto
                if (facturaSeleccionada.TotalDeuda > 100)
                {
                    pnlResumen.BackColor = Color.FromArgb(231, 76, 60);
                }
                else if (facturaSeleccionada.TotalDeuda > 50)
                {
                    pnlResumen.BackColor = Color.FromArgb(243, 156, 18);
                }
                else
                {
                    pnlResumen.BackColor = Color.FromArgb(46, 204, 113);
                }

                pnlResumen.Invalidate();
            }
            else
            {
                lblSeleccionado.Text = "Ninguna factura seleccionada";
                pnlResumen.Visible = false;
                btnPagar.Enabled = false;
                // NO deshabilitar si hay un pago reciente
                if (ultimoPagoRealizado == null)
                    btnImprimirRecibo.Enabled = false;
            }
        }

        private void BtnPagar_Click(object sender, EventArgs e)
        {
            if (lstFacturas.SelectedIndex == -1)
            {
                MessageBox.Show("Selecciona una factura para pagar.", "Selección Requerida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var facturaSeleccionada = facturasPendientes[lstFacturas.SelectedIndex];

            if (facturaSeleccionada.Estado == "Pagado")
            {
                MessageBox.Show("Esta factura ya ha sido pagada.", "Factura Pagada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Confirmación de pago
            DialogResult result = MessageBox.Show(
                $"¿Confirma el pago de la factura?\n\nRecibo: {facturaSeleccionada.NumeroRecibo}\nMonto: S/ {facturaSeleccionada.TotalDeuda:N2}\nPeríodo: {facturaSeleccionada.Periodo}",
                "Confirmar Pago",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                // MODIFICADO: Guardar el pago realizado para poder imprimirlo
                ultimoPagoRealizado = PagoService.RegistrarPago(facturaSeleccionada, "Efectivo");

                MessageBox.Show($"✅ Pago registrado exitosamente\n\nRecibo: {facturaSeleccionada.NumeroRecibo}\nTotal Pagado: S/ {facturaSeleccionada.TotalDeuda:N2}", "Pago Registrado", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Habilitar el botón de imprimir después del pago
                btnImprimirRecibo.Enabled = true;

                // Refrescar lista pero mantener el botón habilitado
                string dniActual = txtDNI.Text;
                BtnBuscar_Click(null, null);
                btnImprimirRecibo.Enabled = true; // Asegurar que quede habilitado
            }
        }

        // ===== NUEVA FUNCIONALIDAD: IMPRESIÓN DE RECIBO PDF =====

        private void BtnImprimirRecibo_Click(object sender, EventArgs e)
        {
            if (ultimoPagoRealizado == null)
            {
                MessageBox.Show("No hay ningún pago reciente para imprimir.", "Sin Pago", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Mostrar diálogo para guardar el archivo
                SaveFileDialog saveDialog = new SaveFileDialog()
                {
                    Filter = "Archivos PDF (*.pdf)|*.pdf",
                    FileName = $"Recibo_Pago_{ultimoPagoRealizado.NumeroRecibo}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf",
                    Title = "Guardar Recibo de Pago"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    GenerarReciboPDF(ultimoPagoRealizado, saveDialog.FileName);

                    DialogResult abrirArchivo = MessageBox.Show(
                        "✅ Recibo generado exitosamente.\n\n¿Desea abrir el archivo PDF?",
                        "Recibo Generado",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information
                    );

                    if (abrirArchivo == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(saveDialog.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar el recibo: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GenerarReciboPDF(Pago pago, string rutaArchivo)
        {
            // Crear documento PDF
            Document document = new Document(PageSize.A4, 50, 50, 50, 50);
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(rutaArchivo, FileMode.Create));

            document.Open();

            // Fuentes
            iTextSharp.text.Font fontTitulo = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, BaseColor.BLUE);
            iTextSharp.text.Font fontSubtitulo = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, BaseColor.BLACK);
            iTextSharp.text.Font fontNormal = FontFactory.GetFont(FontFactory.HELVETICA, 12, BaseColor.BLACK);
            iTextSharp.text.Font fontPequeño = FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.GRAY);

            // Encabezado de la empresa
            Paragraph empresa = new Paragraph("EL SAPO - GESTIÓN DE AGUA", fontTitulo);
            empresa.Alignment = Element.ALIGN_CENTER;
            document.Add(empresa);

            Paragraph direccion = new Paragraph("Sistema de Gestión de Servicios de Agua\nAv. Principal 123, Ciudad\nTeléfono: (01) 123-4567", fontPequeño);
            direccion.Alignment = Element.ALIGN_CENTER;
            direccion.SpacingAfter = 20;
            document.Add(direccion);

            // Línea separadora - CORREGIDO
            iTextSharp.text.pdf.draw.LineSeparator linea = new iTextSharp.text.pdf.draw.LineSeparator();
            document.Add(new iTextSharp.text.Chunk(linea));
            document.Add(iTextSharp.text.Chunk.NEWLINE);

            // Título del recibo
            Paragraph tituloRecibo = new Paragraph("RECIBO DE PAGO", fontSubtitulo);
            tituloRecibo.Alignment = Element.ALIGN_CENTER;
            tituloRecibo.SpacingAfter = 20;
            document.Add(tituloRecibo);

            // Información del recibo
            PdfPTable tablaInfo = new PdfPTable(2);
            tablaInfo.WidthPercentage = 100;
            tablaInfo.SetWidths(new float[] { 30f, 70f });

            // Obtener información del cliente (buscar en facturas)
            var factura = FacturaRepository.ObtenerPorNumero(pago.NumeroRecibo);
            string nombreCliente = "Cliente No Encontrado";
            string periodo = "N/A";

            if (factura != null)
            {
                // Aquí deberías obtener el nombre del cliente desde ClienteRepository
                // nombreCliente = ClienteRepository.ObtenerPorDNI(factura.DNICliente)?.Nombre ?? "Cliente No Encontrado";
                nombreCliente = $"Cliente DNI: {factura.DNICliente}"; // Temporal
                periodo = factura.Periodo;
            }

            AgregarFilaTabla(tablaInfo, "N° Recibo:", pago.NumeroRecibo.ToString(), fontNormal);
            AgregarFilaTabla(tablaInfo, "Fecha de Pago:", pago.FechaPago.ToString("dd/MM/yyyy HH:mm"), fontNormal);
            AgregarFilaTabla(tablaInfo, "Cliente:", nombreCliente, fontNormal);
            AgregarFilaTabla(tablaInfo, "DNI:", pago.DNICliente, fontNormal);
            AgregarFilaTabla(tablaInfo, "Período:", periodo, fontNormal);
            AgregarFilaTabla(tablaInfo, "Método de Pago:", pago.MetodoPago, fontNormal);

            document.Add(tablaInfo);
            document.Add(iTextSharp.text.Chunk.NEWLINE);

            // Monto pagado (destacado)
            PdfPTable tablaMonto = new PdfPTable(1);
            tablaMonto.WidthPercentage = 100;

            PdfPCell celdaMonto = new PdfPCell(new Phrase($"MONTO PAGADO: S/ {pago.MontoPagado:N2}", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16, BaseColor.GREEN)));
            celdaMonto.HorizontalAlignment = Element.ALIGN_CENTER;
            celdaMonto.Padding = 15;
            celdaMonto.BackgroundColor = new BaseColor(240, 248, 255);
            tablaMonto.AddCell(celdaMonto);

            document.Add(tablaMonto);
            document.Add(iTextSharp.text.Chunk.NEWLINE);

            // Nota al pie - CORREGIDO: cambiamos ALIGN_JUSTIFY por ALIGN_LEFT
            Paragraph nota = new Paragraph("IMPORTANTE: Conserve este recibo como comprobante de pago. " +
                "Este documento es válido como constancia de cancelación del servicio de agua.", fontPequeño);
            nota.Alignment = Element.ALIGN_LEFT; // Cambiado de ALIGN_JUSTIFY
            nota.SpacingBefore = 30;
            document.Add(nota);

            // Firma y fecha de generación
            Paragraph fechaGeneracion = new Paragraph($"Documento generado el: {DateTime.Now:dd/MM/yyyy HH:mm:ss}", fontPequeño);
            fechaGeneracion.Alignment = Element.ALIGN_RIGHT;
            fechaGeneracion.SpacingBefore = 40;
            document.Add(fechaGeneracion);

            // Código de verificación (simulado)
            string codigoVerificacion = $"VER-{pago.Id:D6}-{DateTime.Now:yyyyMMdd}";
            Paragraph verificacion = new Paragraph($"Código de verificación: {codigoVerificacion}", fontPequeño);
            verificacion.Alignment = Element.ALIGN_RIGHT;
            document.Add(verificacion);

            document.Close();
        }

        private void AgregarFilaTabla(PdfPTable tabla, string etiqueta, string valor, iTextSharp.text.Font fuente)
        {
            PdfPCell celdaEtiqueta = new PdfPCell(new Phrase(etiqueta, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12)));
            celdaEtiqueta.Border = iTextSharp.text.Rectangle.NO_BORDER; // Especificamos el namespace completo
            celdaEtiqueta.PaddingBottom = 8;

            PdfPCell celdaValor = new PdfPCell(new Phrase(valor, fuente));
            celdaValor.Border = iTextSharp.text.Rectangle.NO_BORDER; // Especificamos el namespace completo
            celdaValor.PaddingBottom = 8;

            tabla.AddCell(celdaEtiqueta);
            tabla.AddCell(celdaValor);
        }
    }
}