using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DocumentFormat.OpenXml.Office2019.Excel.RichData2;
using PapeleriaBaez.Data;
using PapeleriaBaez.Models;

namespace PapeleriaBaez.Views
{
    /// <summary>
    /// Lógica de interacción para NuevoServicioWindow.xaml
    /// </summary>
    public partial class NuevoServicioWindow : Window
    {
        public NuevoServicioWindow()
        {
            InitializeComponent();

            CargarCategorias();
        }

        private void CargarCategorias()
        {
            cmbCategoria.ItemsSource = new[]
            {
                "Copias e impresiones",
                "Acabados",
                "Forrado",
                "Documentos",
                "Manualidades",
                "Otros"
            };

            cmbCategoria.SelectedIndex = 0;
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            string nombre = txtNombre.Text.Trim();
            string categoria = cmbCategoria.SelectedItem?.ToString() ?? "";
            string descripcion = txtDescripcion.Text.Trim();

            if (string.IsNullOrWhiteSpace(nombre))
            {
                MessageBox.Show(
                    "Ingresa el nombre del servicio.",
                    "Dato requerido",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                txtNombre.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(categoria))
            {
                MessageBox.Show(
                    "Selecciona una categoría.",
                    "Dato requerido",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                cmbCategoria.Focus();
                return;
            }

            if (!decimal.TryParse(txtPrecioBase.Text, out decimal precioBase))
            {
                MessageBox.Show(
                    "Ingresa un precio válido.",
                    "Precio inválido",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                txtPrecioBase.Focus();
                return;
            }

            if (precioBase < 0)
            {
                MessageBox.Show(
                    "El precio no puede ser negativo.",
                    "Precio inválido",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                txtPrecioBase.Focus();
                return;
            }

            using var db = new AppDbContext();

            bool existe = db.Servicios.Any(s =>
                s.Nombre.ToLower() == nombre.ToLower());

            if (existe)
            {
                MessageBox.Show(
                    "Ya existe un servicio con ese nombre.",
                    "Servicio duplicado",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            var servicio = new Servicio
            {
                Nombre = nombre,
                Categoria = categoria,
                Descripcion = descripcion,
                PrecioBase = precioBase,
                PermitePrecioManual = chkPrecioManual.IsChecked == true,
                Activo = true
            };

            db.Servicios.Add(servicio);
            db.SaveChanges();

            MessageBox.Show(
                "El servicio fue registrado correctamente.",
                "Servicio guardado",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            DialogResult = true;
            Close();
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
