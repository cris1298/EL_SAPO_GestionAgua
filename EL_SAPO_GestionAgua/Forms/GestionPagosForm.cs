using EL_SAPO_GestionAgua.Data;
using EL_SAPO_GestionAgua.Models;
using EL_SAPO_GestionAgua.Services;
using System;
using System.Collections.Generic;
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
        private Label lblSeleccionado;
        private List<Factura> facturasPendientes = new List<Factura>();

        public GestionPagosForm()
        {
            InitializeComponent();
            InitUI();
        }

        private void InitUI()
        {
            this.Text = "Gestión de Pagos";
            this.Width = 500;
            this.Height = 500;

            int top = 20;
            int left = 20;

            this.Controls.Add(new Label() { Text = "DNI Cliente:", Top = top, Left = left });
            txtDNI = new TextBox() { Top = top + 20, Left = left, Width = 200 };
            btnBuscar = new Button() { Text = "Buscar Deudas", Top = top + 20, Left = left + 210 };
            btnBuscar.Click += BtnBuscar_Click;

            top += 60;
            lstFacturas = new ListBox() { Top = top, Left = left, Width = 450, Height = 200 };
            lstFacturas.SelectedIndexChanged += LstFacturas_SelectedIndexChanged;

            top += 220;
            lblSeleccionado = new Label() { Text = "Factura Seleccionada: Ninguna", Top = top, Left = left, Width = 400 };
            btnPagar = new Button() { Text = "Registrar Pago", Top = top + 40, Left = left };
            btnPagar.Click += BtnPagar_Click;

            this.Controls.Add(txtDNI);
            this.Controls.Add(btnBuscar);
            this.Controls.Add(lstFacturas);
            this.Controls.Add(lblSeleccionado);
            this.Controls.Add(btnPagar);
        }

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            string dni = txtDNI.Text.Trim();
            lstFacturas.Items.Clear();
            facturasPendientes = FacturaRepository.ObtenerPendientesPorDNI(dni);

            foreach (var factura in facturasPendientes)
            {
                lstFacturas.Items.Add($"Recibo {factura.NumeroRecibo} - S/ {factura.TotalDeuda:N2} - {factura.Periodo}");
            }

            if (facturasPendientes.Count == 0)
                MessageBox.Show("No hay facturas pendientes.");
        }

        private void LstFacturas_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblSeleccionado.Text = "Factura Seleccionada: " + lstFacturas.SelectedItem?.ToString();
        }

        private void BtnPagar_Click(object sender, EventArgs e)
        {
            if (lstFacturas.SelectedIndex == -1)
            {
                MessageBox.Show("Selecciona una factura para pagar.");
                return;
            }

            var facturaSeleccionada = facturasPendientes[lstFacturas.SelectedIndex];

            if (facturaSeleccionada.Estado == "Pagado")
            {
                MessageBox.Show("Esta factura ya ha sido pagada.");
                return;
            }

            PagoService.RegistrarPago(facturaSeleccionada, "Efectivo");
            MessageBox.Show($"Pago registrado para Recibo {facturaSeleccionada.NumeroRecibo} - Total: S/ {facturaSeleccionada.TotalDeuda:N2}");

            BtnBuscar_Click(null, null); // Refrescar lista
        }
    }
}
