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
using System.Windows.Navigation;
using System.Windows.Shapes;
using PapeleriaBaez.Data;
using PapeleriaBaez.Models;

namespace PapeleriaBaez.Views
{
    /// <summary>
    /// Lógica de interacción para ServiciosView.xaml
    /// </summary>
    public partial class ServiciosView : UserControl
    {
        public ServiciosView()
        {
            InitializeComponent();

            CargarServicios();
        }

        private void CargarServicios()
        {
            using var db = new AppDbContext();

            var servicios = db.Servicios
                .OrderBy(s => s.Categoria)
                .ThenBy(s => s.Nombre)
                .ToList();

            dgServicios.ItemsSource = servicios;
        }

        private void TxtBuscarServicio_TextChanged(object sender, TextChangedEventArgs e)
        {
            string texto = txtBuscarServicio.Text.Trim().ToLower();

            using var db = new AppDbContext();

            var servicios = db.Servicios
                .AsEnumerable()
                .Where(s =>
                    string.IsNullOrWhiteSpace(texto) ||
                    s.Nombre.ToLower().Contains(texto) ||
                    s.Categoria.ToLower().Contains(texto) ||
                    s.Descripcion.ToLower().Contains(texto))
                .OrderBy(s => s.Categoria)
                .ThenBy(s => s.Nombre)
                .ToList();

            dgServicios.ItemsSource = servicios;
        }

        private void BtnNuevoServicio_Click(object sender, RoutedEventArgs e)
        {
            var ventana = new NuevoServicioWindow();

            bool? resultado = ventana.ShowDialog();

            if (resultado == true)
            {
                CargarServicios();
            }
        }

        private void BtnModificarServicio_Click(object sender, RoutedEventArgs e)
        {
            if (dgServicios.SelectedItem is not Servicio servicio)
            {
                MessageBox.Show(
                    "Selecciona un servicio para modificar.",
                    "Servicio no seleccionado",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            MessageBox.Show(
                $"Aquí moidficaremos el servicio:\n\n{servicio.Nombre}",
                "Modificar servicio",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void BtnDesactivarServicio_Click(object sender, RoutedEventArgs e)
        {
            if (dgServicios.SelectedItem is not Servicio servicio)
            {
                MessageBox.Show(
                    "Selecciona un servicio para desactivar.",
                    "Servicio no seleccionado",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (!servicio.Activo)
            {
                MessageBox.Show(
                    "Este servicio ya está desactivado.",
                    "Servicio inactivo",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                return;
            }

            var respuesta = MessageBox.Show(
                $"¿Seguro que deseas desactivar este servicio?\n\n" +
                $"{servicio.Nombre}",
                "Desactivar servicio",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (respuesta != MessageBoxResult.Yes)
                return;

            using var db = new AppDbContext();

            var servicioDb = db.Servicios
                .FirstOrDefault(s => s.Id == servicio.Id);

            if (servicioDb == null)
                return;

            servicioDb.Activo = false;

            db.SaveChanges();

            MessageBox.Show(
                "El servicio fue desactivado correctamente.",
                "Servicio desactivado",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            CargarServicios();
        }
    }
}
