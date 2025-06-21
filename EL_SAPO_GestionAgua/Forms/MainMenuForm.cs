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
    public partial class MainMenuForm : Form
    {
        public MainMenuForm()
        {
            InitializeComponent();
            InitializeMenu();
        }
        private void InitializeMenu()
        {
            this.Text = "EL SAPO - Sistema de Gestión";
            this.Width = 400;
            this.Height = 450;
            this.StartPosition = FormStartPosition.CenterScreen;

            Label titleLabel = new Label();
            titleLabel.Text = "Menú Principal";
            titleLabel.Font = new System.Drawing.Font("Segoe UI", 14, System.Drawing.FontStyle.Bold);
            titleLabel.AutoSize = true;
            titleLabel.Top = 20;
            titleLabel.Left = 120;
            this.Controls.Add(titleLabel);

            string[] opciones = {
                "Registro de Usuario",
                "Búsqueda del Cliente",
                "Generación de Facturas",
                "Gestión de Pagos",
                "Reporte de Deudores",
                "Salir"
            };

            for (int i = 0; i < opciones.Length; i++)
            {
                Button btn = new Button();
                btn.Text = opciones[i];
                btn.Width = 220;
                btn.Height = 40;
                btn.Top = 70 + (i * 50);
                btn.Left = 80;
                btn.Tag = opciones[i];
                btn.Click += MenuButton_Click;
                this.Controls.Add(btn);
            }
        }

        private void MenuButton_Click(object sender, EventArgs e)
        {
            string opcion = (string)((Button)sender).Tag;

            Form form = null;

            switch (opcion)
            {
                case "Registro de Usuario":
                    form = new RegistroUsuarioForm();
                    break;
                case "Búsqueda del Cliente":
                    form = new BuscarClienteForm();
                    break;
                case "Generación de Facturas":
                    form = new GenerarFacturaForm();
                    break;
                case "Gestión de Pagos":
                    form = new GestionPagosForm();
                    break;
                case "Reporte de Deudores":
                    form = new ReporteDeudoresForm();
                    break;
                case "Salir":
                    Application.Exit();
                    return;
            }

            if (form != null)
            {
                form.ShowDialog();
            }
        }
    }
}
