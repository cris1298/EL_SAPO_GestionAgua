using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EL_SAPO_GestionAgua.Models;
using EL_SAPO_GestionAgua.Services;

namespace EL_SAPO_GestionAgua.Forms
{
    public partial class RegistroUsuarioForm : Form
    {
        private Panel headerPanel;
        private Panel formPanel;
        private Panel buttonPanel;

        public RegistroUsuarioForm()
        {
            InitializeComponent();
            InitializeForm();
        }

        private void InitializeForm()
        {
            this.Text = "Registro de Usuario - EL SAPO";
            this.Width = 520;
            this.Height = 680;
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
                Height = 80,
                BackColor = Color.FromArgb(40, 167, 69)
            };

            Label titleLabel = new Label()
            {
                Text = "👤 Registro de Nuevo Usuario",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Top = 25,
                Left = 50
            };

            Label subtitleLabel = new Label()
            {
                Text = "Complete todos los campos para registrar un nuevo usuario",
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                ForeColor = Color.FromArgb(220, 255, 220),
                AutoSize = true,
                Top = 55,
                Left = 50
            };

            headerPanel.Controls.Add(titleLabel);
            headerPanel.Controls.Add(subtitleLabel);

            // Panel del formulario
            formPanel = new Panel()
            {
                Top = 80,
                Left = 0,
                Width = this.ClientSize.Width,
                Height = 500,
                BackColor = Color.White,
                BorderStyle = BorderStyle.None
            };
            formPanel.Paint += (s, e) => {
                ControlPaint.DrawBorder(e.Graphics, formPanel.ClientRectangle,
                    Color.FromArgb(220, 220, 220), ButtonBorderStyle.Solid);
            };

            // Crear los campos del formulario
            CreateFormField("🆔 DNI:", "txtDNI", 30, formPanel);
            CreateFormField("👤 Nombres:", "txtNombres", 100, formPanel);
            CreateFormField("👥 Apellidos:", "txtApellidos", 170, formPanel);
            CreateFormField("🏠 Dirección:", "txtDireccion", 240, formPanel);
            CreateFormField("📞 Teléfono:", "txtTelefono", 310, formPanel);
            CreateFormField("📧 Email:", "txtEmail", 380, formPanel);

            // Campo especial para el ComboBox
            Label lblTipo = new Label()
            {
                Text = "📋 Tipo de Registro:",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 51, 51),
                AutoSize = true,
                Top = 450,
                Left = 40
            };

            ComboBox cmbTipo = new ComboBox()
            {
                Name = "cmbTipo",
                Top = 475,
                Left = 40,
                Width = 420,
                Height = 35,
                Font = new Font("Segoe UI", 10F),
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White
            };
            cmbTipo.Items.AddRange(new string[] { "Nuevo Usuario", "Hijo de Usuario Antiguo", "Usuario Antiguo" });

            formPanel.Controls.Add(lblTipo);
            formPanel.Controls.Add(cmbTipo);

            // Panel de botones
            buttonPanel = new Panel()
            {
                Top = 580,
                Left = 0,
                Width = this.ClientSize.Width,
                Height = 80,
                BackColor = Color.FromArgb(248, 249, 250)
            };

            Button btnGuardar = new Button()
            {
                Text = "💾 Registrar Usuario",
                Top = 20,
                Left = 120,
                Width = 260,
                Height = 45,
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnGuardar.FlatAppearance.BorderSize = 0;
            btnGuardar.FlatAppearance.MouseOverBackColor = Color.FromArgb(33, 136, 56);
            btnGuardar.FlatAppearance.MouseDownBackColor = Color.FromArgb(25, 103, 42);

            // Efecto de sombra para el botón
            btnGuardar.Paint += (s, e) => {
                var rect = new Rectangle(3, 3, btnGuardar.Width - 3, btnGuardar.Height - 3);
                using (var brush = new SolidBrush(Color.FromArgb(50, 0, 0, 0)))
                {
                    e.Graphics.FillRectangle(brush, rect);
                }
            };

            btnGuardar.Click += (s, e) =>
            {
                // Obtener referencias a los controles
                var txtDNI = formPanel.Controls.Find("txtDNI", false)[0] as TextBox;
                var txtNombres = formPanel.Controls.Find("txtNombres", false)[0] as TextBox;
                var txtApellidos = formPanel.Controls.Find("txtApellidos", false)[0] as TextBox;
                var txtDireccion = formPanel.Controls.Find("txtDireccion", false)[0] as TextBox;
                var txtTelefono = formPanel.Controls.Find("txtTelefono", false)[0] as TextBox;
                var txtEmail = formPanel.Controls.Find("txtEmail", false)[0] as TextBox;

                // Validación básica
                if (string.IsNullOrWhiteSpace(txtDNI.Text) ||
                    string.IsNullOrWhiteSpace(txtNombres.Text) ||
                    string.IsNullOrWhiteSpace(txtApellidos.Text) ||
                    cmbTipo.SelectedItem == null)
                {
                    MessageBox.Show("Por favor, complete todos los campos obligatorios (DNI, Nombres, Apellidos y Tipo de Registro).",
                        "Campos requeridos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var usuario = new Usuario()
                {
                    DNI = txtDNI.Text,
                    Nombres = txtNombres.Text,
                    Apellidos = txtApellidos.Text,
                    Direccion = txtDireccion.Text,
                    Telefono = txtTelefono.Text,
                    Email = txtEmail.Text,
                    TipoRegistro = cmbTipo.SelectedItem?.ToString()
                };

                var resultado = UsuarioService.RegistrarUsuario(usuario);
                MessageBox.Show(resultado, "Registro", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Limpiar formulario después del registro exitoso
                if (resultado.Contains("exitosamente") || resultado.Contains("éxito"))
                {
                    LimpiarFormulario();
                }
            };

            buttonPanel.Controls.Add(btnGuardar);

            // Agregar paneles al formulario
            this.Controls.Add(headerPanel);
            this.Controls.Add(formPanel);
            this.Controls.Add(buttonPanel);
        }

        private void CreateFormField(string labelText, string textBoxName, int topPosition, Panel parentPanel)
        {
            Label label = new Label()
            {
                Text = labelText,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 51, 51),
                AutoSize = true,
                Top = topPosition,
                Left = 40
            };

            TextBox textBox = new TextBox()
            {
                Name = textBoxName,
                Top = topPosition + 25,
                Left = 40,
                Width = 420,
                Height = 30,
                Font = new Font("Segoe UI", 10F),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };

            // Efecto focus en los TextBox
            textBox.Enter += (s, e) => {
                textBox.BackColor = Color.FromArgb(255, 248, 220);
                textBox.BorderStyle = BorderStyle.FixedSingle;
            };

            textBox.Leave += (s, e) => {
                textBox.BackColor = Color.White;
            };

            // Validaciones específicas
            if (textBoxName == "txtDNI")
            {
                textBox.KeyPress += (s, e) => {
                    if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                        e.Handled = true;
                };
                textBox.MaxLength = 8;
            }
            else if (textBoxName == "txtTelefono")
            {
                textBox.KeyPress += (s, e) => {
                    if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != '-' && e.KeyChar != ' ')
                        e.Handled = true;
                };
                textBox.MaxLength = 15;
            }
            else if (textBoxName == "txtEmail")
            {
                textBox.Leave += (s, e) => {
                    if (!string.IsNullOrEmpty(textBox.Text) && !IsValidEmail(textBox.Text))
                    {
                        MessageBox.Show("Por favor, ingrese un email válido.", "Email inválido",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        textBox.Focus();
                    }
                };
            }

            parentPanel.Controls.Add(label);
            parentPanel.Controls.Add(textBox);
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void LimpiarFormulario()
        {
            foreach (Control control in formPanel.Controls)
            {
                if (control is TextBox textBox)
                {
                    textBox.Clear();
                }
                else if (control is ComboBox comboBox)
                {
                    comboBox.SelectedIndex = -1;
                }
            }

            // Enfocar el primer campo
            var primerTextBox = formPanel.Controls.Find("txtDNI", false)[0] as TextBox;
            primerTextBox?.Focus();
        }
    }
}