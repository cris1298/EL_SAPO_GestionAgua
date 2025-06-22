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
        private Panel panelHeader;
        private Panel panelBusqueda;
        private Panel panelExportacion;
        private Label lblTitulo;

        public ReporteDeudoresForm()
        {
            InitializeComponent();
            InitUI();
        }

        private void InitUI()
        {
            this.Text = "Reporte de Deudores - Sistema de Gestión de Agua";
            this.Width = 1000;
            this.Height = 700;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 244, 248);
            this.Font = new Font("Segoe UI", 9F, FontStyle.Regular);

            // Panel Header con título
            panelHeader = new Panel()
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.FromArgb(34, 139, 174),
                Padding = new Padding(20, 15, 20, 15)
            };

            lblTitulo = new Label()
            {
                Text = "📊 REPORTE DE DEUDORES",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 20)
            };

            panelHeader.Controls.Add(lblTitulo);

            // Panel de Búsqueda
            panelBusqueda = new Panel()
            {
                Top = 80,
                Left = 20,
                Width = 940,
                Height = 80,
                BackColor = Color.White,
                BorderStyle = BorderStyle.None
            };

            // Agregar sombra visual al panel
            panelBusqueda.Paint += (s, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, panelBusqueda.ClientRectangle,
                    Color.FromArgb(220, 220, 220), ButtonBorderStyle.Solid);
            };

            Label lblFiltro = new Label()
            {
                Text = "🔍 Buscar por DNI o Nombre:",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Top = 15,
                Left = 20,
                AutoSize = true
            };

            txtFiltro = new TextBox()
            {
                Top = 40,
                Left = 20,
                Width = 350,
                Height = 25,
                Font = new Font("Segoe UI", 10F),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(250, 250, 250)
            };

            btnBuscar = new Button()
            {
                Text = "🔍 Buscar",
                Top = 39,
                Left = 385,
                Width = 100,
                Height = 30,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                BackColor = Color.FromArgb(46, 125, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnBuscar.FlatAppearance.BorderSize = 0;
            btnBuscar.Click += BtnBuscar_Click;

            panelBusqueda.Controls.AddRange(new Control[] { lblFiltro, txtFiltro, btnBuscar });

            // Panel de Exportación
            panelExportacion = new Panel()
            {
                Top = 170,
                Left = 20,
                Width = 940,
                Height = 60,
                BackColor = Color.White,
                BorderStyle = BorderStyle.None
            };

            panelExportacion.Paint += (s, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, panelExportacion.ClientRectangle,
                    Color.FromArgb(220, 220, 220), ButtonBorderStyle.Solid);
            };

            Label lblExportar = new Label()
            {
                Text = "📤 Opciones de Exportación:",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Top = 10,
                Left = 20,
                AutoSize = true
            };

            btnExportarPDF = new Button()
            {
                Text = "📄 Exportar PDF",
                Top = 30,
                Left = 20,
                Width = 140,
                Height = 25,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                BackColor = Color.FromArgb(211, 47, 47),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnExportarPDF.FlatAppearance.BorderSize = 0;
            btnExportarPDF.Click += (s, e) => PDFGenerator.ExportarReporteDeudores(deudores);

            btnExportarExcel = new Button()
            {
                Text = "📊 Exportar Excel",
                Top = 30,
                Left = 170,
                Width = 140,
                Height = 25,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                BackColor = Color.FromArgb(56, 142, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnExportarExcel.FlatAppearance.BorderSize = 0;
            btnExportarExcel.Click += (s, e) => ExcelExporter.ExportarReporteDeudores(deudores);

            panelExportacion.Controls.AddRange(new Control[] { lblExportar, btnExportarPDF, btnExportarExcel });

            // DataGridView mejorado
            dgvReporte = new DataGridView()
            {
                Top = 240,
                Left = 20,
                Width = 940,
                Height = 380,
                ReadOnly = true,
                AutoGenerateColumns = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 9F),
                GridColor = Color.FromArgb(230, 230, 230)
            };

            // Estilo del header
            dgvReporte.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle()
            {
                BackColor = Color.FromArgb(52, 73, 94),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                SelectionBackColor = Color.FromArgb(52, 73, 94),
                Padding = new Padding(5)
            };

            dgvReporte.ColumnHeadersHeight = 35;

            // Estilo de las filas
            dgvReporte.DefaultCellStyle = new DataGridViewCellStyle()
            {
                BackColor = Color.White,
                ForeColor = Color.FromArgb(52, 73, 94),
                SelectionBackColor = Color.FromArgb(232, 245, 252),
                SelectionForeColor = Color.FromArgb(52, 73, 94),
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                Padding = new Padding(5)
            };

            dgvReporte.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle()
            {
                BackColor = Color.FromArgb(248, 249, 250),
                ForeColor = Color.FromArgb(52, 73, 94),
                SelectionBackColor = Color.FromArgb(232, 245, 252),
                SelectionForeColor = Color.FromArgb(52, 73, 94),
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                Padding = new Padding(5)
            };

            dgvReporte.RowTemplate.Height = 30;

            // Configuración de columnas
            dgvReporte.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "DNI",
                DataPropertyName = "DNICliente",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle() { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            dgvReporte.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "Cliente",
                DataPropertyName = "NombreCompleto",
                Width = 180,
                DefaultCellStyle = new DataGridViewCellStyle() { Font = new Font("Segoe UI", 9F, FontStyle.Bold) }
            });

            dgvReporte.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "Dirección",
                DataPropertyName = "Direccion",
                Width = 200
            });

            dgvReporte.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "Periodo",
                DataPropertyName = "Periodo",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle() { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            dgvReporte.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "Deuda",
                DataPropertyName = "TotalDeuda",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle()
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Format = "C2",
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(211, 47, 47)
                }
            });

            dgvReporte.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "Estado",
                DataPropertyName = "Estado",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle() { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            dgvReporte.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "Vencimiento",
                DataPropertyName = "FechaVencimiento",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle()
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    Format = "dd/MM/yyyy"
                }
            });

            // Agregar efecto hover a los botones
            AddHoverEffect(btnBuscar, Color.FromArgb(46, 125, 50), Color.FromArgb(67, 160, 71));
            AddHoverEffect(btnExportarPDF, Color.FromArgb(211, 47, 47), Color.FromArgb(244, 67, 54));
            AddHoverEffect(btnExportarExcel, Color.FromArgb(56, 142, 60), Color.FromArgb(76, 175, 80));

            // Mostrar todos los deudores al cargar el formulario
            deudores = ReporteService.ObtenerDeudores("");  // o null si el método lo permite
            dgvReporte.DataSource = deudores;

            // Agregar controles al formulario
            this.Controls.AddRange(new Control[] {
                panelHeader,
                panelBusqueda,
                panelExportacion,
                dgvReporte
            });
        }

        private void AddHoverEffect(Button button, Color normalColor, Color hoverColor)
        {
            button.MouseEnter += (s, e) => button.BackColor = hoverColor;
            button.MouseLeave += (s, e) => button.BackColor = normalColor;
        }

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            string filtro = txtFiltro.Text.Trim();
            if (string.IsNullOrEmpty(filtro))
            {
                // Mostrar todos si no hay filtro
                deudores = ReporteService.ObtenerDeudores("");
            }
            else
            {
                // Filtrar por DNI o nombre
                deudores = ReporteService.ObtenerDeudores(filtro);
            }
            dgvReporte.DataSource = null; // Forzar refresh
            dgvReporte.DataSource = deudores;
        }
    }
}