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
        public RegistroUsuarioForm()
        {
            InitializeComponent();
            InitializeForm();
        }
        private void InitializeForm()
        {
            this.Text = "Registro de Usuario";
            this.Width = 400;
            this.Height = 500;
            this.StartPosition = FormStartPosition.CenterScreen;

            Label lblDNI = new Label() { Text = "DNI:", Top = 20, Left = 20 };
            TextBox txtDNI = new TextBox() { Name = "txtDNI", Top = 40, Left = 20, Width = 300 };

            Label lblNombres = new Label() { Text = "Nombres:", Top = 70, Left = 20 };
            TextBox txtNombres = new TextBox() { Name = "txtNombres", Top = 90, Left = 20, Width = 300 };

            Label lblApellidos = new Label() { Text = "Apellidos:", Top = 120, Left = 20 };
            TextBox txtApellidos = new TextBox() { Name = "txtApellidos", Top = 140, Left = 20, Width = 300 };

            Label lblDireccion = new Label() { Text = "Dirección:", Top = 170, Left = 20 };
            TextBox txtDireccion = new TextBox() { Name = "txtDireccion", Top = 190, Left = 20, Width = 300 };

            Label lblTelefono = new Label() { Text = "Teléfono:", Top = 220, Left = 20 };
            TextBox txtTelefono = new TextBox() { Name = "txtTelefono", Top = 240, Left = 20, Width = 300 };

            Label lblEmail = new Label() { Text = "Email:", Top = 270, Left = 20 };
            TextBox txtEmail = new TextBox() { Name = "txtEmail", Top = 290, Left = 20, Width = 300 };

            Label lblTipo = new Label() { Text = "Tipo de Registro:", Top = 320, Left = 20 };
            ComboBox cmbTipo = new ComboBox() { Name = "cmbTipo", Top = 340, Left = 20, Width = 300 };
            cmbTipo.Items.AddRange(new string[] { "Nuevo Usuario", "Hijo de Usuario Antiguo", "Usuario Antiguo" });

            Button btnGuardar = new Button() { Text = "Registrar Usuario", Top = 400, Left = 100, Width = 180 };
            btnGuardar.Click += (s, e) =>
            {
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
            };

            this.Controls.AddRange(new Control[] {
                lblDNI, txtDNI, lblNombres, txtNombres,
                lblApellidos, txtApellidos, lblDireccion, txtDireccion,
                lblTelefono, txtTelefono, lblEmail, txtEmail,
                lblTipo, cmbTipo, btnGuardar
            });
        }
    }
}
