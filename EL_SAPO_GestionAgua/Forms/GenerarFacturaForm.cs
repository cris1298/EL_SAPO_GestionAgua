using EL_SAPO_GestionAgua.Data;
using EL_SAPO_GestionAgua.Models;
using EL_SAPO_GestionAgua.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;


namespace EL_SAPO_GestionAgua.Forms
{
    public partial class GenerarFacturaForm : Form
    {
        private TextBox txtDNI, txtLecturaAnterior, txtLecturaActual, txtPeriodo, txtObservaciones, txtMantenimiento;
        private DateTimePicker dtpVencimiento;
        private CheckBox chkReposicion;
        private Label lblConsumo, lblCosto, lblClienteInfo, lblMantenimiento;
        private Button btnBuscar, btnGenerar, btnImprimir;
        private GroupBox grpCliente, grpLecturas, grpDetalles;
        private Panel pnlResultados;

        private bool esPrimeraFactura = true;
        private Factura ultimaFacturaGenerada = null;
        private Usuario clienteActual = null;

        public GenerarFacturaForm()
        {
            InitializeComponent();
            InitUI();
        }

        private void InitUI()
        {
            // Configuración principal del formulario
            this.Text = "🧾 Generar Factura de Agua";
            this.Size = new Size(580, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(240, 244, 248);
            this.Font = new Font("Segoe UI", 9F, FontStyle.Regular);

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
                Text = "GENERAR NUEVA FACTURA",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                AutoSize = true,
                Location = new Point(0, currentY)
            };
            mainPanel.Controls.Add(titleLabel);
            currentY += 45;

            // Grupo: Información del Cliente
            grpCliente = CreateStyledGroupBox("👤 Información del Cliente", currentY, 520);
            mainPanel.Controls.Add(grpCliente);

            // DNI y búsqueda
            Label lblDNI = CreateStyledLabel("DNI del Cliente:", 20, 25);
            grpCliente.Controls.Add(lblDNI);

            txtDNI = CreateStyledTextBox(20, 45, 180);
            txtDNI.Font = new Font("Consolas", 10F);
            grpCliente.Controls.Add(txtDNI);

            btnBuscar = CreateStyledButton("🔍 Buscar Cliente", 210, 44, 120, 25);
            btnBuscar.BackColor = Color.FromArgb(52, 152, 219);
            btnBuscar.Click += BtnBuscar_Click;
            grpCliente.Controls.Add(btnBuscar);

            // Información del cliente encontrado
            lblClienteInfo = new Label()
            {
                Text = "Ingrese un DNI y presione 'Buscar Cliente'",
                Location = new Point(20, 80),
                Size = new Size(480, 20),
                ForeColor = Color.FromArgb(127, 140, 141),
                Font = new Font("Segoe UI", 9F, FontStyle.Italic)
            };
            grpCliente.Controls.Add(lblClienteInfo);

            currentY += grpCliente.Height + groupSpacing;

            // Grupo: Lecturas del Medidor (ACTUALIZADO)
            grpLecturas = CreateStyledGroupBox("📊 Lecturas del Medidor", currentY, 520);
            mainPanel.Controls.Add(grpLecturas);

            // Lectura anterior
            Label lblLectAnterior = CreateStyledLabel("Lectura Anterior (m³):", 20, 25);
            grpLecturas.Controls.Add(lblLectAnterior);

            txtLecturaAnterior = CreateStyledTextBox(20, 45, 120);
            txtLecturaAnterior.ReadOnly = true;
            txtLecturaAnterior.BackColor = Color.FromArgb(236, 240, 241);
            txtLecturaAnterior.Font = new Font("Consolas", 10F, FontStyle.Bold);
            txtLecturaAnterior.TextAlign = HorizontalAlignment.Center;
            grpLecturas.Controls.Add(txtLecturaAnterior);

            // Lectura actual
            Label lblLectActual = CreateStyledLabel("Lectura Actual (m³):", 160, 25);
            grpLecturas.Controls.Add(lblLectActual);

            txtLecturaActual = CreateStyledTextBox(160, 45, 120);
            txtLecturaActual.Font = new Font("Consolas", 10F, FontStyle.Bold);
            txtLecturaActual.TextAlign = HorizontalAlignment.Center;
            txtLecturaActual.TextChanged += TxtLecturaActual_TextChanged;
            grpLecturas.Controls.Add(txtLecturaActual);

            // NUEVO: Campo de Mantenimiento
            lblMantenimiento = CreateStyledLabel("Mantenimiento (S/):", 300, 25);
            grpLecturas.Controls.Add(lblMantenimiento);

            txtMantenimiento = CreateStyledTextBox(300, 45, 100);
            txtMantenimiento.Text = "1.00";
            txtMantenimiento.Font = new Font("Consolas", 10F, FontStyle.Bold);
            txtMantenimiento.TextAlign = HorizontalAlignment.Center;
            txtMantenimiento.TextChanged += TxtMantenimiento_TextChanged;
            grpLecturas.Controls.Add(txtMantenimiento);

            // Panel de resultados
            pnlResultados = new Panel()
            {
                Location = new Point(20, 80),
                Size = new Size(480, 60),
                BackColor = Color.FromArgb(46, 204, 113),
                Visible = false
            };
            pnlResultados.Paint += (s, e) =>
            {
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(46, 204, 113)), pnlResultados.ClientRectangle);
                using (Pen pen = new Pen(Color.FromArgb(39, 174, 96), 2))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, pnlResultados.Width - 1, pnlResultados.Height - 1);
                }
            };
            grpLecturas.Controls.Add(pnlResultados);

            lblConsumo = new Label()
            {
                Text = "Consumo: 0 m³",
                Location = new Point(15, 8),
                Size = new Size(200, 20),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = Color.Transparent
            };
            pnlResultados.Controls.Add(lblConsumo);

            lblCosto = new Label()
            {
                Text = "Costo: S/ 0.00",
                Location = new Point(15, 32),
                Size = new Size(200, 20),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                BackColor = Color.Transparent
            };
            pnlResultados.Controls.Add(lblCosto);

            currentY += grpLecturas.Height + groupSpacing;

            // Grupo: Detalles de la Factura
            grpDetalles = CreateStyledGroupBox("📋 Detalles de la Factura", currentY, 520);
            mainPanel.Controls.Add(grpDetalles);

            // Período
            Label lblPeriodo = CreateStyledLabel("Período de Facturación:", 20, 25);
            grpDetalles.Controls.Add(lblPeriodo);

            txtPeriodo = CreateStyledTextBox(20, 45, 200);
            txtPeriodo.Text = "Ej: Junio 2025";
            txtPeriodo.ForeColor = Color.Gray;
            txtPeriodo.Enter += (s, e) => {
                if (txtPeriodo.Text == "Ej: Junio 2025" && txtPeriodo.ForeColor == Color.Gray)
                {
                    txtPeriodo.Text = "";
                    txtPeriodo.ForeColor = Color.Black;
                }
            };
            txtPeriodo.Leave += (s, e) => {
                if (string.IsNullOrWhiteSpace(txtPeriodo.Text))
                {
                    txtPeriodo.Text = "Ej: Junio 2025";
                    txtPeriodo.ForeColor = Color.Gray;
                }
            };
            grpDetalles.Controls.Add(txtPeriodo);

            // Fecha de vencimiento
            Label lblVencimiento = CreateStyledLabel("Fecha de Vencimiento:", 240, 25);
            grpDetalles.Controls.Add(lblVencimiento);

            dtpVencimiento = new DateTimePicker()
            {
                Location = new Point(240, 45),
                Size = new Size(200, 25),
                Format = DateTimePickerFormat.Short,
                Font = new Font("Segoe UI", 9F)
            };
            grpDetalles.Controls.Add(dtpVencimiento);

            // Observaciones
            Label lblObservaciones = CreateStyledLabel("Observaciones:", 20, 85);
            grpDetalles.Controls.Add(lblObservaciones);

            txtObservaciones = new TextBox()
            {
                Location = new Point(20, 105),
                Size = new Size(480, 60),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Segoe UI", 9F),
                BorderStyle = BorderStyle.FixedSingle
            };
            grpDetalles.Controls.Add(txtObservaciones);

            // Checkbox reposición
            chkReposicion = new CheckBox()
            {
                Text = "💰 Aplicar cargo por reposición de medidor (S/ 150.00)",
                Location = new Point(20, 180),
                Size = new Size(480, 25),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(192, 57, 43),
                UseVisualStyleBackColor = true
            };
            chkReposicion.CheckedChanged += (s, e) => RecalcularCosto();
            grpDetalles.Controls.Add(chkReposicion);

            currentY += grpDetalles.Height + groupSpacing;

            // Panel de botones
            Panel pnlBotones = new Panel()
            {
                Location = new Point(0, currentY),
                Size = new Size(520, 80),
                BackColor = Color.Transparent
            };
            mainPanel.Controls.Add(pnlBotones);

            // Botón generar factura
            btnGenerar = new Button()
            {
                Text = "🧾 GENERAR FACTURA",
                Location = new Point(0, 0),
                Size = new Size(520, 35),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false
            };
            btnGenerar.FlatAppearance.BorderSize = 0;
            btnGenerar.FlatAppearance.MouseOverBackColor = Color.FromArgb(192, 57, 43);
            btnGenerar.Click += BtnGenerar_Click;
            pnlBotones.Controls.Add(btnGenerar);

            // NUEVO: Botón imprimir factura
            btnImprimir = new Button()
            {
                Text = "🖨️ IMPRIMIR FACTURA PDF",
                Location = new Point(0, 40),
                Size = new Size(520, 35),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                BackColor = Color.FromArgb(39, 174, 96),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false,
                Enabled = false
            };
            btnImprimir.FlatAppearance.BorderSize = 0;
            btnImprimir.FlatAppearance.MouseOverBackColor = Color.FromArgb(46, 204, 113);
            btnImprimir.Click += BtnImprimir_Click;
            pnlBotones.Controls.Add(btnImprimir);
        }

        private GroupBox CreateStyledGroupBox(string text, int y, int width)
        {
            return new GroupBox()
            {
                Text = text,
                Location = new Point(0, y),
                Size = new Size(width, text.Contains("Lecturas") ? 160 : text.Contains("Detalles") ? 220 : 110),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
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
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                ForeColor = Color.FromArgb(52, 73, 94)
            };
        }

        private TextBox CreateStyledTextBox(int x, int y, int width)
        {
            return new TextBox()
            {
                Location = new Point(x, y),
                Size = new Size(width, 25),
                Font = new Font("Segoe UI", 9F),
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
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false
            };
        }

        // ===== MÉTODOS DE EVENTOS =====

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            var cliente = UsuarioService.BuscarPorDNI(txtDNI.Text.Trim());
            if (cliente == null)
            {
                lblClienteInfo.Text = "❌ Cliente no encontrado";
                lblClienteInfo.ForeColor = Color.FromArgb(231, 76, 60);
                MessageBox.Show("Cliente no encontrado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            clienteActual = cliente;
            lblClienteInfo.Text = $"✅ Cliente: {cliente.Nombres} {cliente.Apellidos}";
            lblClienteInfo.ForeColor = Color.FromArgb(39, 174, 96);
            MessageBox.Show($"Cliente encontrado: {cliente.Nombres} {cliente.Apellidos}", "Cliente Encontrado", MessageBoxButtons.OK, MessageBoxIcon.Information);

            var facturasPrevias = FacturaRepository.ObtenerPorDNI(cliente.DNI);
            esPrimeraFactura = facturasPrevias.Count == 0;

            if (esPrimeraFactura)
            {
                txtLecturaAnterior.ReadOnly = false;
                txtLecturaAnterior.BackColor = Color.White;
                txtLecturaAnterior.Text = "";
            }
            else
            {
                txtLecturaAnterior.ReadOnly = true;
                txtLecturaAnterior.BackColor = Color.FromArgb(236, 240, 241);
                txtLecturaAnterior.Text = cliente.UltimaLectura.ToString();
            }

            // Recalcular costo al cambiar cliente
            RecalcularCosto();
        }

        private void TxtLecturaActual_TextChanged(object sender, EventArgs e)
        {
            RecalcularCosto();
        }

        private void TxtMantenimiento_TextChanged(object sender, EventArgs e)
        {
            RecalcularCosto();
        }

        private void RecalcularCosto()
        {
            if (int.TryParse(txtLecturaAnterior.Text, out int anterior) &&
                int.TryParse(txtLecturaActual.Text, out int actual) &&
                decimal.TryParse(txtMantenimiento.Text, out decimal mantenimiento))
            {
                if (actual < anterior)
                {
                    pnlResultados.Visible = false;
                    return;
                }

                int consumo = actual - anterior;

                // Verificar si debe aplicar mora usando el nuevo método
                bool conMora = clienteActual != null ? FacturacionService.DebeAplicarMora(clienteActual.DNI) : false;

                decimal costo = FacturacionService.CalcularCosto(consumo, conMora, chkReposicion.Checked, mantenimiento);

                lblConsumo.Text = $"Consumo: {consumo} m³";

                string textoMora = conMora ? " (CON MORA)" : "";
                lblCosto.Text = $"Costo: S/ {costo:N2}{textoMora}";

                // Mostrar el panel de resultados
                pnlResultados.Visible = true;

                // Cambiar color según el consumo y mora
                if (conMora)
                {
                    pnlResultados.BackColor = Color.FromArgb(192, 57, 43); // Rojo por mora
                }
                else if (consumo > 50)
                {
                    pnlResultados.BackColor = Color.FromArgb(231, 76, 60); // Alto consumo
                }
                else if (consumo > 20)
                {
                    pnlResultados.BackColor = Color.FromArgb(243, 156, 18); // Consumo medio
                }
                else if (consumo <= 2)
                {
                    pnlResultados.BackColor = Color.FromArgb(52, 152, 219); // Consumo mínimo (solo mantenimiento)
                }
                else
                {
                    pnlResultados.BackColor = Color.FromArgb(46, 204, 113); // Consumo bajo
                }

                pnlResultados.Invalidate();
            }
            else
            {
                pnlResultados.Visible = false;
            }
        }

        private void BtnGenerar_Click(object sender, EventArgs e)
        {
            var cliente = UsuarioService.BuscarPorDNI(txtDNI.Text.Trim());
            if (cliente == null)
            {
                MessageBox.Show("Cliente no encontrado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(txtLecturaAnterior.Text, out int anterior) ||
                !int.TryParse(txtLecturaActual.Text, out int actual))
            {
                MessageBox.Show("Lecturas inválidas.", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (actual < anterior)
            {
                MessageBox.Show("La lectura actual no puede ser menor que la anterior.", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtMantenimiento.Text, out decimal mantenimiento) || mantenimiento < 0)
            {
                MessageBox.Show("El costo de mantenimiento debe ser un valor válido.", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Obtener el período, manejando el placeholder
            string periodo = txtPeriodo.Text == "Ej: Junio 2025" ? "" : txtPeriodo.Text;

            var factura = FacturacionService.GenerarFactura(new Factura
            {
                DNICliente = txtDNI.Text.Trim(),
                LecturaAnterior = anterior,
                LecturaActual = actual,
                Periodo = periodo,
                FechaEmision = DateTime.Now,
                FechaVencimiento = dtpVencimiento.Value,
                Observaciones = txtObservaciones.Text,
                AplicarReposicion = chkReposicion.Checked,
                NombreCompleto = $"{cliente.Nombres} {cliente.Apellidos}",
                Direccion = cliente.Direccion
            }, mantenimiento);

            // Solo actualiza última lectura si NO es la primera factura
            if (!esPrimeraFactura)
            {
                UsuarioService.ActualizarLectura(factura.DNICliente, factura.LecturaActual);
            }

            ultimaFacturaGenerada = factura;
            btnImprimir.Enabled = true;

            MessageBox.Show($"Factura generada exitosamente:\n\nRecibo N°: {factura.NumeroRecibo}\nTotal a Pagar: S/ {factura.TotalDeuda:N2}",
                "Factura Generada", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnImprimir_Click(object sender, EventArgs e)
        {
            if (ultimaFacturaGenerada == null || clienteActual == null)
            {
                MessageBox.Show("No hay ninguna factura para imprimir. Genere una factura primero.",
                    "Sin Factura", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show("¿Desea guardar la factura como PDF o enviarla directamente a la impresora?",
                "Opciones de Impresión", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Guardar como PDF
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "Archivos PDF (*.pdf)|*.pdf";
                saveDialog.FileName = $"Factura_{ultimaFacturaGenerada.NumeroRecibo}_{DateTime.Now:yyyyMMdd}.pdf";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    PDFService.GuardarComoPDF(ultimaFacturaGenerada, clienteActual, saveDialog.FileName);
                }
            }
            else if (result == DialogResult.No)
            {
                // Imprimir directamente
                PDFService.ImprimirFactura(ultimaFacturaGenerada, clienteActual);
            }
        }
    }
}