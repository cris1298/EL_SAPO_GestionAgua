using EL_SAPO_GestionAgua.Models;
using EL_SAPO_GestionAgua.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EL_SAPO_GestionAgua.Forms
{
    public partial class BuscarClienteForm : Form
    {
        private TextBox txtDNI;
        private DataGridView dgvHistorial;
        private Label lblInfo;
        private Panel panelBusqueda;
        private Panel panelInfo;
        private Panel panelHistorial;

        public BuscarClienteForm()
        {
            InitializeComponent();
            InitializeForm();
        }

        private void InitializeForm()
        {
            this.Text = "Buscar Cliente - Gestión de Agua";
            this.Width = 900;
            this.Height = 650;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 248, 255); // Azul muy claro
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Panel de búsqueda
            panelBusqueda = new Panel()
            {
                Top = 15,
                Left = 15,
                Width = 850,
                Height = 80,
                BackColor = Color.White,
                BorderStyle = BorderStyle.None
            };
            panelBusqueda.Paint += (s, e) => {
                ControlPaint.DrawBorder(e.Graphics, panelBusqueda.ClientRectangle,
                    Color.FromArgb(200, 200, 200), ButtonBorderStyle.Solid);
            };

            Label lblBuscar = new Label()
            {
                Text = "DNI del Cliente:",
                Top = 15,
                Left = 20,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 51, 51),
                AutoSize = true
            };

            txtDNI = new TextBox()
            {
                Top = 40,
                Left = 20,
                Width = 200,
                Height = 25,
                Font = new Font("Segoe UI", 10F),
                BorderStyle = BorderStyle.FixedSingle
            };

            Button btnBuscar = new Button()
            {
                Text = "🔍 Buscar",
                Top = 38,
                Left = 240,
                Width = 100,
                Height = 30,
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnBuscar.FlatAppearance.BorderSize = 0;
            btnBuscar.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 86, 179);
            btnBuscar.Click += BtnBuscar_Click;

            panelBusqueda.Controls.Add(lblBuscar);
            panelBusqueda.Controls.Add(txtDNI);
            panelBusqueda.Controls.Add(btnBuscar);

            // Panel de información del cliente
            panelInfo = new Panel()
            {
                Top = 110,
                Left = 15,
                Width = 850,
                Height = 100,
                BackColor = Color.White,
                BorderStyle = BorderStyle.None
            };
            panelInfo.Paint += (s, e) => {
                ControlPaint.DrawBorder(e.Graphics, panelInfo.ClientRectangle,
                    Color.FromArgb(200, 200, 200), ButtonBorderStyle.Solid);
            };

            Label lblTituloInfo = new Label()
            {
                Text = "📋 Información del Cliente",
                Top = 10,
                Left = 20,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 167, 69),
                AutoSize = true
            };

            lblInfo = new Label()
            {
                Top = 35,
                Left = 20,
                Width = 800,
                Height = 50,
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = Color.FromArgb(51, 51, 51),
                Text = "Ingrese un DNI y presione 'Buscar' para mostrar la información del cliente."
            };

            panelInfo.Controls.Add(lblTituloInfo);
            panelInfo.Controls.Add(lblInfo);

            // Panel del historial
            panelHistorial = new Panel()
            {
                Top = 225,
                Left = 15,
                Width = 850,
                Height = 370,
                BackColor = Color.White,
                BorderStyle = BorderStyle.None
            };
            panelHistorial.Paint += (s, e) => {
                ControlPaint.DrawBorder(e.Graphics, panelHistorial.ClientRectangle,
                    Color.FromArgb(200, 200, 200), ButtonBorderStyle.Solid);
            };

            Label lblTituloHistorial = new Label()
            {
                Text = "📊 Historial de Facturación",
                Top = 10,
                Left = 20,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 53, 69),
                AutoSize = true
            };

            dgvHistorial = new DataGridView()
            {
                Top = 40,
                Left = 20,
                Width = 810,
                Height = 315,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                Font = new Font("Segoe UI", 9F)
            };

            // Estilo del header del DataGridView
            dgvHistorial.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 58, 64);
            dgvHistorial.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvHistorial.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvHistorial.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(52, 58, 64);
            dgvHistorial.ColumnHeadersHeight = 35;

            // Estilo de las filas
            dgvHistorial.DefaultCellStyle.BackColor = Color.White;
            dgvHistorial.DefaultCellStyle.ForeColor = Color.FromArgb(51, 51, 51);
            dgvHistorial.DefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 244, 255);
            dgvHistorial.DefaultCellStyle.SelectionForeColor = Color.FromArgb(51, 51, 51);
            dgvHistorial.RowTemplate.Height = 30;

            // Filas alternadas
            dgvHistorial.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);

            panelHistorial.Controls.Add(lblTituloHistorial);
            panelHistorial.Controls.Add(dgvHistorial);

            // Agregar todos los paneles al formulario
            this.Controls.Add(panelBusqueda);
            this.Controls.Add(panelInfo);
            this.Controls.Add(panelHistorial);

            // Configurar el Enter en el TextBox para buscar
            txtDNI.KeyPress += (s, e) => {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    btnBuscar.PerformClick();
                    e.Handled = true;
                }
            };
        }

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            string dni = txtDNI.Text.Trim();

            if (string.IsNullOrEmpty(dni))
            {
                MessageBox.Show("Por favor, ingrese un DNI.", "Campo requerido",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDNI.Focus();
                return;
            }

            var cliente = UsuarioService.BuscarPorDNI(dni);
            if (cliente == null)
            {
                MessageBox.Show("Cliente no encontrado.", "Búsqueda sin resultados",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                lblInfo.Text = "Cliente no encontrado. Verifique el DNI ingresado.";
                lblInfo.ForeColor = Color.FromArgb(220, 53, 69);
                dgvHistorial.DataSource = null;
                return;
            }

            lblInfo.Text = $"Nombre: {cliente.Nombres} {cliente.Apellidos}\n" +
                          $"Dirección: {cliente.Direccion}\n" +
                          $"Teléfono: {cliente.Telefono}";
            lblInfo.ForeColor = Color.FromArgb(51, 51, 51);

            var historial = FacturacionService.ObtenerHistorialPorDNI(dni);
            dgvHistorial.DataSource = historial;

            if (historial == null || historial.Count == 0)
            {
                MessageBox.Show("No se encontró historial de facturación para este cliente.",
                    "Sin historial", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public void RefrescarHistorial()
        {
            string dni = txtDNI.Text.Trim();
            if (!string.IsNullOrEmpty(dni))
            {
                var historial = FacturacionService.ObtenerHistorialPorDNI(dni);
                dgvHistorial.DataSource = null; // Importante para refrescar
                dgvHistorial.DataSource = historial;
            }
        }
    }
}