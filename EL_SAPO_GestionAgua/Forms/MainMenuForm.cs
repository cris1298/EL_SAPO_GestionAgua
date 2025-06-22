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
        private Panel headerPanel;
        private Panel menuPanel;
        private Panel footerPanel;

        public MainMenuForm()
        {
            InitializeComponent();
            InitializeMenu();
        }

        private void InitializeMenu()
        {
            this.Text = "EL SAPO - Sistema de Gestión de Agua";
            this.Width = 520;
            this.Height = 650;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(240, 248, 255);

            // Panel de encabezado
            headerPanel = new Panel()
            {
                Top = 0,
                Left = 0,
                Width = this.ClientSize.Width,
                Height = 130,
                BackColor = Color.FromArgb(0, 123, 255)
            };

            // Logo/Icono principal
            Label logoLabel = new Label()
            {
                Text = "🐸",
                Font = new Font("Segoe UI Emoji", 36F, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Top = 25,
                Left = 60
            };

            // Título principal
            Label titleLabel = new Label()
            {
                Text = "EL SAPO",
                Font = new Font("Segoe UI", 26F, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Top = 25,
                Left = 170
            };

            // Subtítulo
            Label subtitleLabel = new Label()
            {
                Text = "Sistema de Gestión de Agua",
                Font = new Font("Segoe UI", 12F, FontStyle.Regular),
                ForeColor = Color.FromArgb(220, 230, 255),
                AutoSize = true,
                Top = 65,
                Left = 170
            };

            // Fecha y hora
            Label dateTimeLabel = new Label()
            {
                Text = DateTime.Now.ToString("dd/MM/yyyy - HH:mm"),
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                ForeColor = Color.FromArgb(200, 220, 255),
                AutoSize = true,
                Top = 95,
                Left = 170
            };

            headerPanel.Controls.Add(logoLabel);
            headerPanel.Controls.Add(titleLabel);
            headerPanel.Controls.Add(subtitleLabel);
            headerPanel.Controls.Add(dateTimeLabel);

            // Panel del menú
            menuPanel = new Panel()
            {
                Top = 130,
                Left = 0,
                Width = this.ClientSize.Width,
                Height = 450,
                BackColor = Color.FromArgb(240, 248, 255)
            };

            Label menuTitleLabel = new Label()
            {
                Text = "📋 Menú Principal",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 51, 51),
                AutoSize = true,
                Top = 20,
                Left = 60
            };

            menuPanel.Controls.Add(menuTitleLabel);

            // Opciones del menú con iconos
            var opciones = new Dictionary<string, string>
            {
                { "👤 Registro de Usuario", "Registro de Usuario" },
                { "🔍 Búsqueda del Cliente", "Búsqueda del Cliente" },
                { "📄 Generación de Facturas", "Generación de Facturas" },
                { "💰 Gestión de Pagos", "Gestión de Pagos" },
                { "📊 Reporte de Deudores", "Reporte de Deudores" },
                { "🚪 Salir", "Salir" }
            };

            // Colores para cada botón
            Color[] buttonColors = {
                Color.FromArgb(40, 167, 69),   // Verde - Registro
                Color.FromArgb(0, 123, 255),   // Azul - Búsqueda
                Color.FromArgb(255, 193, 7),   // Amarillo - Facturas
                Color.FromArgb(23, 162, 184),  // Cian - Pagos
                Color.FromArgb(220, 53, 69),   // Rojo - Reportes
                Color.FromArgb(108, 117, 125)  // Gris - Salir
            };

            int index = 0;
            foreach (var opcion in opciones)
            {
                Button btn = new Button()
                {
                    Text = opcion.Key,
                    Width = 320,
                    Height = 48,
                    Top = 65 + (index * 58),
                    Left = 100,
                    Tag = opcion.Value,
                    BackColor = buttonColors[index],
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                    Cursor = Cursors.Hand,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Padding = new Padding(15, 0, 0, 0)
                };

                btn.FlatAppearance.BorderSize = 0;
                btn.FlatAppearance.MouseOverBackColor = ChangeColorBrightness(buttonColors[index], -0.1f);
                btn.FlatAppearance.MouseDownBackColor = ChangeColorBrightness(buttonColors[index], -0.2f);

                // Efecto de sombra
                btn.Paint += (s, e) => {
                    var rect = new Rectangle(3, 3, btn.Width - 3, btn.Height - 3);
                    using (var brush = new SolidBrush(Color.FromArgb(50, 0, 0, 0)))
                    {
                        e.Graphics.FillRectangle(brush, rect);
                    }
                };

                btn.Click += MenuButton_Click;

                // Animación hover
                btn.MouseEnter += (s, e) => {
                    btn.Left = 105;
                };
                btn.MouseLeave += (s, e) => {
                    btn.Left = 100;
                };

                menuPanel.Controls.Add(btn);
                index++;
            }

            // Panel de pie
            footerPanel = new Panel()
            {
                Height = 40,
                BackColor = Color.FromArgb(248, 249, 250),
                Dock = DockStyle.Bottom
            };

            Label footerLabel = new Label()
            {
                Text = "© 2025 EL SAPO - Todos los derechos reservados",
                Font = new Font("Segoe UI", 8F, FontStyle.Regular),
                ForeColor = Color.FromArgb(108, 117, 125),
                AutoSize = true,
                Top = 12,
                Left = 20
            };

            Label versionLabel = new Label()
            {
                Text = "v1.0",
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                ForeColor = Color.FromArgb(108, 117, 125),
                AutoSize = true,
                Top = 12,
                Left = 440
            };

            footerPanel.Controls.Add(footerLabel);
            footerPanel.Controls.Add(versionLabel);

            // Agregar paneles al formulario
            this.Controls.Add(headerPanel);
            this.Controls.Add(menuPanel);
            this.Controls.Add(footerPanel);

            // Timer para actualizar fecha y hora
            Timer timer = new Timer();
            timer.Interval = 60000; // 1 minuto
            timer.Tick += (s, e) => {
                dateTimeLabel.Text = DateTime.Now.ToString("dd/MM/yyyy - HH:mm");
            };
            timer.Start();
        }

        private Color ChangeColorBrightness(Color color, float correctionFactor)
        {
            float red = color.R;
            float green = color.G;
            float blue = color.B;

            if (correctionFactor < 0)
            {
                correctionFactor = 1 + correctionFactor;
                red *= correctionFactor;
                green *= correctionFactor;
                blue *= correctionFactor;
            }
            else
            {
                red = (255 - red) * correctionFactor + red;
                green = (255 - green) * correctionFactor + green;
                blue = (255 - blue) * correctionFactor + blue;
            }

            return Color.FromArgb(color.A, (int)red, (int)green, (int)blue);
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