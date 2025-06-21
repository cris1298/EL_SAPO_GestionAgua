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
        public BuscarClienteForm()
        {
            InitializeComponent();
            InitializeForm();
        }
        private void InitializeForm()
        {
            this.Text = "Buscar Cliente";
            this.Width = 800;
            this.Height = 600;

            Label lblBuscar = new Label() { Text = "DNI del Cliente:", Top = 20, Left = 20 };
            txtDNI = new TextBox() { Top = 40, Left = 20, Width = 200 };

            Button btnBuscar = new Button() { Text = "Buscar", Top = 40, Left = 240 };
            btnBuscar.Click += BtnBuscar_Click;

            lblInfo = new Label() { Top = 80, Left = 20, Width = 700, Height = 60 };

            dgvHistorial = new DataGridView()
            {
                Top = 150,
                Left = 20,
                Width = 740,
                Height = 380,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            this.Controls.Add(lblBuscar);
            this.Controls.Add(txtDNI);
            this.Controls.Add(btnBuscar);
            this.Controls.Add(lblInfo);
            this.Controls.Add(dgvHistorial);
        }

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            string dni = txtDNI.Text.Trim();

            var cliente = UsuarioService.BuscarPorDNI(dni);
            if (cliente == null)
            {
                MessageBox.Show("Cliente no encontrado.");
                return;
            }

            lblInfo.Text = $"Nombre: {cliente.Nombres} {cliente.Apellidos}\nDirección: {cliente.Direccion} - Tel: {cliente.Telefono}";

            var historial = FacturacionService.ObtenerHistorialPorDNI(dni);

            dgvHistorial.DataSource = historial;
        }
        public void RefrescarHistorial()
        {
            string dni = txtDNI.Text.Trim();
            var historial = FacturacionService.ObtenerHistorialPorDNI(dni);
            dgvHistorial.DataSource = null; // Importante para refrescar
            dgvHistorial.DataSource = historial;
        }

    }
}
