using EL_SAPO_GestionAgua.Data;
using EL_SAPO_GestionAgua.Models;
using EL_SAPO_GestionAgua.Services;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace EL_SAPO_GestionAgua.Forms
{
    public partial class GenerarFacturaForm : Form
    {
        private TextBox txtDNI, txtLecturaAnterior, txtLecturaActual, txtPeriodo, txtObservaciones;
        private DateTimePicker dtpVencimiento;
        private CheckBox chkReposicion;
        private Label lblConsumo, lblCosto;
        private Button btnBuscar, btnGenerar;

        private bool esPrimeraFactura = true; // ← se usa en todo el form

        public GenerarFacturaForm()
        {
            InitializeComponent();
            InitUI();
        }

        private void InitUI()
        {
            this.Text = "Generar Factura";
            this.Width = 500;
            this.Height = 600;

            int top = 20;
            int left = 20;

            this.Controls.Add(new Label() { Text = "DNI Cliente:", Top = top, Left = left });
            txtDNI = new TextBox() { Top = top + 20, Left = left, Width = 150 };
            btnBuscar = new Button() { Text = "Buscar", Top = top + 20, Left = left + 170 };
            btnBuscar.Click += BtnBuscar_Click;
            this.Controls.Add(txtDNI);
            this.Controls.Add(btnBuscar);

            top += 60;
            this.Controls.Add(new Label() { Text = "Lectura Anterior:", Top = top, Left = left });
            txtLecturaAnterior = new TextBox() { Top = top + 20, Left = left, Width = 100 };
            txtLecturaAnterior.ReadOnly = true;
            this.Controls.Add(txtLecturaAnterior);

            this.Controls.Add(new Label() { Text = "Lectura Actual:", Top = top, Left = left + 150 });
            txtLecturaActual = new TextBox() { Top = top + 20, Left = left + 150, Width = 100 };
            txtLecturaActual.TextChanged += TxtLecturaActual_TextChanged;
            this.Controls.Add(txtLecturaActual);

            top += 60;
            lblConsumo = new Label() { Text = "Consumo: 0 m³", Top = top, Left = left, Width = 200 };
            lblCosto = new Label() { Text = "Costo: S/ 0.00", Top = top, Left = left + 200, Width = 200 };
            this.Controls.Add(lblConsumo);
            this.Controls.Add(lblCosto);

            top += 40;
            this.Controls.Add(new Label() { Text = "Periodo (ej. Junio 2025):", Top = top, Left = left });
            txtPeriodo = new TextBox() { Top = top + 20, Left = left, Width = 200 };
            this.Controls.Add(txtPeriodo);

            top += 60;
            this.Controls.Add(new Label() { Text = "Fecha de Vencimiento:", Top = top, Left = left });
            dtpVencimiento = new DateTimePicker() { Top = top + 20, Left = left, Width = 200 };
            this.Controls.Add(dtpVencimiento);

            top += 60;
            this.Controls.Add(new Label() { Text = "Observaciones:", Top = top, Left = left });
            txtObservaciones = new TextBox() { Top = top + 20, Left = left, Width = 300 };
            this.Controls.Add(txtObservaciones);

            top += 60;
            chkReposicion = new CheckBox() { Text = "Aplicar cargo por reposición (S/150.00)", Top = top, Left = left, Width = 300 };
            this.Controls.Add(chkReposicion);

            top += 60;
            btnGenerar = new Button() { Text = "Generar Factura", Top = top, Left = left };
            btnGenerar.Click += BtnGenerar_Click;
            this.Controls.Add(btnGenerar);
        }

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            var cliente = UsuarioService.BuscarPorDNI(txtDNI.Text.Trim());
            if (cliente == null)
            {
                MessageBox.Show("Cliente no encontrado.");
                return;
            }

            MessageBox.Show($"Cliente encontrado: {cliente.Nombres} {cliente.Apellidos}");

            var facturasPrevias = FacturaRepository.ObtenerPorDNI(cliente.DNI);
            esPrimeraFactura = facturasPrevias.Count == 0;

            if (esPrimeraFactura)
            {
                txtLecturaAnterior.ReadOnly = false;
                txtLecturaAnterior.Text = ""; // permitir ingreso manual
            }
            else
            {
                txtLecturaAnterior.ReadOnly = true;
                txtLecturaAnterior.Text = cliente.UltimaLectura.ToString(); // autocompletado
            }
        }

        private void TxtLecturaActual_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(txtLecturaAnterior.Text, out int anterior) &&
                int.TryParse(txtLecturaActual.Text, out int actual))
            {
                int consumo = actual - anterior;
                decimal costo = FacturacionService.CalcularCosto(consumo, false, chkReposicion.Checked);

                lblConsumo.Text = $"Consumo: {consumo} m³";
                lblCosto.Text = $"Costo: S/ {costo:N2}";
            }
        }

        private void BtnGenerar_Click(object sender, EventArgs e)
        {
            var cliente = UsuarioService.BuscarPorDNI(txtDNI.Text.Trim());
            if (cliente == null)
            {
                MessageBox.Show("Cliente no encontrado.");
                return;
            }

            if (!int.TryParse(txtLecturaAnterior.Text, out int anterior) ||
                !int.TryParse(txtLecturaActual.Text, out int actual))
            {
                MessageBox.Show("Lecturas inválidas.");
                return;
            }

            if (actual < anterior)
            {
                MessageBox.Show("La lectura actual no puede ser menor que la anterior.");
                return;
            }

            var factura = FacturacionService.GenerarFactura(new Factura
            {
                DNICliente = txtDNI.Text.Trim(),
                LecturaAnterior = anterior,
                LecturaActual = actual,
                Periodo = txtPeriodo.Text,
                FechaEmision = DateTime.Now,
                FechaVencimiento = dtpVencimiento.Value,
                Observaciones = txtObservaciones.Text,
                AplicarReposicion = chkReposicion.Checked
            });

            // Solo actualiza última lectura si NO es la primera factura
            if (!esPrimeraFactura)
            {
                UsuarioService.ActualizarLectura(factura.DNICliente, factura.LecturaActual);
            }

            MessageBox.Show($"Factura generada:\nTotal: S/ {factura.TotalDeuda:N2}");
        }
    }
}
