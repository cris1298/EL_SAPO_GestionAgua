using EL_SAPO_GestionAgua.Data;
using EL_SAPO_GestionAgua.Models;
using EL_SAPO_GestionAgua.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EL_SAPO_GestionAgua.Forms
{
    public partial class GestionPagosForm : Form
    {
        private TextBox txtDNI;
        private ListBox lstFacturas;
        private Button btnBuscar, btnPagar;
        private Label lblSeleccionado, lblTotalDeuda, lblClienteInfo;
        private GroupBox grpBusqueda, grpFacturas, grpPago;
        private Panel pnlResumen;
        private List<Factura> facturasPendientes = new List<Factura>();

        public GestionPagosForm()
        {
            InitializeComponent();
            InitUI();
        }

        private void InitUI()
        {
            // Configuración principal del formulario
            this.Text = "💰 Gestión de Pagos";
            this.Size = new Size(650, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(236, 240, 241);
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
                Text = "GESTIÓN DE PAGOS",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                AutoSize = true,
                Location = new Point(0, currentY)
            };
            mainPanel.Controls.Add(titleLabel);

            Label subtitleLabel = new Label()
            {
                Text = "Consulta y registra pagos de facturas pendientes",
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
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
            txtDNI.Font = new Font("Consolas", 11F, FontStyle.Bold);
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
                Font = new Font("Segoe UI", 9F, FontStyle.Italic)
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
                Font = new Font("Consolas", 10F),
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
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = Color.Transparent
            };
            pnlResumen.Controls.Add(lblSeleccionado);

            lblTotalDeuda = new Label()
            {
                Text = "Total: S/ 0.00",
                Location = new Point(20, 45),
                Size = new Size(200, 25),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                BackColor = Color.Transparent
            };
            pnlResumen.Controls.Add(lblTotalDeuda);

            currentY += pnlResumen.Height + groupSpacing;

            // Grupo: Registro de Pago
            grpPago = CreateStyledGroupBox("💳 Registro de Pago", currentY, 590);
            mainPanel.Controls.Add(grpPago);

            // Instrucciones
            Label lblInstrucciones = new Label()
            {
                Text = "Seleccione una factura de la lista para proceder con el pago",
                Location = new Point(20, 25),
                Size = new Size(550, 20),
                ForeColor = Color.FromArgb(127, 140, 141),
                Font = new Font("Segoe UI", 9F, FontStyle.Italic)
            };
            grpPago.Controls.Add(lblInstrucciones);

            // Botón de pago
            btnPagar = new Button()
            {
                Text = "💰 REGISTRAR PAGO",
                Location = new Point(20, 55),
                Size = new Size(550, 45),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
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
        }

        private GroupBox CreateStyledGroupBox(string text, int y, int width)
        {
            int height = 120;
            if (text.Contains("Facturas")) height = 240;
            if (text.Contains("Pago")) height = 120;

            return new GroupBox()
            {
                Text = text,
                Location = new Point(0, y),
                Size = new Size(width, height),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
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
                Size = new Size(width, 30),
                Font = new Font("Segoe UI", 10F),
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
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
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

        // ===== LÓGICA ORIGINAL SIN MODIFICACIONES =====

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
                PagoService.RegistrarPago(facturaSeleccionada, "Efectivo");
                MessageBox.Show($"✅ Pago registrado exitosamente\n\nRecibo: {facturaSeleccionada.NumeroRecibo}\nTotal Pagado: S/ {facturaSeleccionada.TotalDeuda:N2}", "Pago Registrado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                BtnBuscar_Click(null, null); // Refrescar lista
            }
        }
    }
}