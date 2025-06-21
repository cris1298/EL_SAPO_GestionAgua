using EL_SAPO_GestionAgua.Models;
using EL_SAPO_GestionAgua.Services;
using EL_SAPO_GestionAgua.Utils;
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
    public partial class ReporteDeudoresForm : Form
    {
        private TextBox txtFiltro;
        private Button btnBuscar, btnExportarPDF, btnExportarExcel;
        private DataGridView dgvReporte;
        private List<Factura> deudores;
        public ReporteDeudoresForm()
        {
            InitializeComponent();
            InitUI();
        }

        private void InitUI()
        {
            this.Text = "Reporte de Deudores";
            this.Width = 900;
            this.Height = 600;

            Label lblFiltro = new Label() { Text = "Buscar (DNI / Nombre):", Top = 20, Left = 20 };
            txtFiltro = new TextBox() { Top = 40, Left = 20, Width = 300 };

            btnBuscar = new Button() { Text = "Buscar", Top = 40, Left = 330 };
            btnBuscar.Click += BtnBuscar_Click;

            btnExportarPDF = new Button() { Text = "Exportar PDF", Top = 40, Left = 420 };
            btnExportarExcel = new Button() { Text = "Exportar Excel", Top = 40, Left = 540 };

            btnExportarPDF.Click += (s, e) => PDFGenerator.ExportarReporteDeudores(deudores);
            btnExportarExcel.Click += (s, e) => ExcelExporter.ExportarReporteDeudores(deudores);

            dgvReporte = new DataGridView()
            {
                Top = 80,
                Left = 20,
                Width = 840,
                Height = 440,
                ReadOnly = true,
                AutoGenerateColumns = false
            };

            dgvReporte.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "DNI", DataPropertyName = "DNICliente" });
            dgvReporte.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Cliente", DataPropertyName = "NombreCompleto" });
            dgvReporte.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Dirección", DataPropertyName = "Direccion" });
            dgvReporte.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Periodo", DataPropertyName = "Periodo" });
            dgvReporte.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Deuda", DataPropertyName = "TotalDeuda" });
            dgvReporte.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Estado", DataPropertyName = "Estado" });
            dgvReporte.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Vencimiento", DataPropertyName = "FechaVencimiento" });

            this.Controls.Add(lblFiltro);
            this.Controls.Add(txtFiltro);
            this.Controls.Add(btnBuscar);
            this.Controls.Add(btnExportarPDF);
            this.Controls.Add(btnExportarExcel);
            this.Controls.Add(dgvReporte);
        }

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            string filtro = txtFiltro.Text.Trim();
            deudores = ReporteService.ObtenerDeudores(filtro);
            dgvReporte.DataSource = deudores;
        }
    }
}
