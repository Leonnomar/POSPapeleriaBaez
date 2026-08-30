using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
using Microsoft.EntityFrameworkCore;
using PapeleriaBaez.Data;
using PapeleriaBaez.Models;

namespace PapeleriaBaez.Views
{
    /// <summary>
    /// Lógica de interacción para CapturarCambiosCanjeWindow.xaml
    /// </summary>
    public partial class CapturarCambiosCanjeWindow : Window
    {

        private readonly int cantidadCambios;

        public CapturarCambiosCanjeWindow(int cantidad)
        {
            InitializeComponent();

            cantidadCambios = cantidad;

            grpCambio2.Visibility =
                cantidad == 2
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            CargarCombosIniciales();
        }

        private void CargarCombosIniciales()
        {
            var articulos = new[]
            {
                "Uniformes",
                "Tenis"
            };

            var tiposCambio = new[]
            {
                "CambioTalla",
                "MismaTalla"
            };

            cmbC1DevArticulo.ItemsSource = articulos;
            cmbC1TipoCambio.ItemsSource = tiposCambio;

            cmbC2DevArticulo.ItemsSource = articulos;
            cmbC2TipoCambio.ItemsSource = tiposCambio;
        }

        private void cmbC1DevArticulo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CargarArticuloDevuelto(cmbC1DevArticulo, cmbC1DevTipo, cmbC1DevColor, cmbC1DevTalla, cmbC1EntTipo, cmbC1EntColor, cmbC1EntTalla, lblC1Disponibles);
        }

        private void cmbC1DevTipo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CargarColoresDevuelto(cmbC1DevArticulo, cmbC1DevTipo, cmbC1DevColor, cmbC1DevTalla);
        }

        private void cmbC1DevColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CargarTallasDevuelto(cmbC1DevArticulo, cmbC1DevTipo, cmbC1DevColor, cmbC1DevTalla);
        }

        private void cmbC1EntTipo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CargarColoresEntregado(cmbC1DevArticulo, cmbC1EntTipo, cmbC1EntColor, cmbC1EntTalla, lblC1Disponibles);
        }

        private void cmbC1EntColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CargarTallasEntregado(cmbC1DevArticulo, cmbC1EntTipo, cmbC1EntColor, cmbC1EntTalla, lblC1Disponibles);
        }

        private void cmbC1EntTalla_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MostrarExistencia(cmbC1DevArticulo, cmbC1EntTipo, cmbC1EntColor, cmbC1EntTalla, lblC1Disponibles);
        }

        private void cmbC2DevArticulo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CargarArticuloDevuelto(cmbC2DevArticulo, cmbC2DevTipo, cmbC2DevColor, cmbC2DevTalla, cmbC2EntTipo, cmbC2EntColor, cmbC2EntTalla, lblC2Disponibles);
        }

        private void cmbC2DevTipo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CargarColoresDevuelto(cmbC2DevArticulo, cmbC2DevTipo, cmbC2DevColor, cmbC2DevTalla);
        }

        private void cmbC2DevColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CargarTallasDevuelto(cmbC2DevArticulo, cmbC2DevTipo, cmbC2DevColor, cmbC2DevTalla);
        }

        private void cmbC2EntTipo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CargarColoresEntregado(cmbC2DevArticulo, cmbC2EntTipo, cmbC2EntColor, cmbC2EntTalla, lblC2Disponibles);
        }

        private void cmbC2EntColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CargarTallasEntregado(cmbC2DevArticulo, cmbC2EntTipo, cmbC2EntColor, cmbC2EntTalla, lblC2Disponibles);
        }

        private void cmbC2EntTalla_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MostrarExistencia(cmbC2DevArticulo, cmbC2EntTipo, cmbC2EntColor, cmbC2EntTalla, lblC2Disponibles);
        }

        private void CargarArticuloDevuelto(ComboBox cmbArticulo, ComboBox cmbDevTipo, ComboBox cmbDevColor, ComboBox cmbDevTalla, ComboBox cmbEntTipo, ComboBox cmbEntColor, ComboBox cmbEntTalla, TextBlock lblDisponibles)
        {
            string articulo = cmbArticulo.SelectedItem?.ToString() ?? "";

            cmbDevTipo.ItemsSource = null;
            cmbDevColor.ItemsSource = null;
            cmbDevTalla.ItemsSource = null;

            cmbEntTipo.ItemsSource = null;
            cmbEntColor.ItemsSource = null;
            cmbEntTalla.ItemsSource = null;

            lblDisponibles.Text = "Disponibles: 0";

            using var db = new AppDbContext();

            if (articulo == "Uniforme")
            {
                var tipos = db.UniformesCanje
                    .Select(u => u.Tipo)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();

                cmbDevTipo.ItemsSource = tipos;
                cmbEntTipo.ItemsSource = tipos;
            }
            else if (articulo == "Tenis")
            {
                cmbDevTipo.ItemsSource = new[]
                {
                    "Tenis"
                };

                cmbEntTipo.ItemsSource = new[]
                {
                    "Tenis"
                };

                cmbDevTipo.SelectedIndex = 0;
                cmbEntTipo.SelectedIndex = 0;
            }
        }

        private void CargarColoresDevuelto(ComboBox cmbArticulo, ComboBox cmbTipo, ComboBox cmbColor, ComboBox cmbTalla)
        {
            string articulo = cmbArticulo.SelectedItem?.ToString() ?? "";

            cmbColor.ItemsSource = null;
            cmbTalla.ItemsSource = null;

            using var db = new AppDbContext();

            if (articulo == "Uniforme")
            {
                string tipo = cmbTipo.SelectedItem?.ToString() ?? "";

                cmbColor.ItemsSource = db.UniformesCanje
                    .Where(u => u.Tipo == tipo)
                    .Select(u => u.Color)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();
            }
            else if (articulo == "Tenis")
            {
                cmbColor.ItemsSource = new[]
                {
                    "-"
                };

                cmbColor.SelectedIndex = 0;

                cmbTalla.ItemsSource = db.TenisCanjes
                    .OrderBy(t => t.Talla)
                    .Select(t => t.Talla)
                    .ToList();
            }
        }

        private void CargarTallasDevuelto(ComboBox cmbArticulo, ComboBox cmbTipo, ComboBox cmbColor, ComboBox cmbTalla)
        {
            string articulo = cmbArticulo.SelectedItem?.ToString() ?? "";

            if (articulo != "Uniforme")
                return;

            string tipo = cmbTipo.SelectedItem?.ToString() ?? "";

            string color = cmbColor.SelectedItem?.ToString() ?? "";

            using var db = new AppDbContext();

            cmbTalla.ItemsSource = db.UniformesCanje
                .Where(u =>
                    u.Tipo == tipo &&
                    u.Color == color)
                .Select(u => u.Talla)
                .Distinct()
                .OrderBy(x => x)
                .ToList();
        }

        private void CargarColoresEntregado(ComboBox cmbArticulo, ComboBox cmbTipo, ComboBox cmbColor, ComboBox cmbTalla, TextBlock lblDisponibles)
        {
            string articulo = cmbArticulo.SelectedItem?.ToString() ?? "";

            cmbColor.ItemsSource = null;
            cmbTalla.ItemsSource = null;

            lblDisponibles.Text = "Disponibles: 0";

            using var db = new AppDbContext();

            if (articulo == "Uniforme")
            {
                string tipo = cmbTipo.SelectedItem?.ToString() ?? "";

                cmbColor.ItemsSource = db.UniformesCanje
                    .Where(u => u.Tipo == tipo)
                    .Select(u => u.Color)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();
            }
            else if (articulo == "Tenis")
            {
                cmbColor.ItemsSource = new[] { "-" };

                cmbColor.SelectedIndex = 0;

                cmbTalla.ItemsSource = db.TenisCanjes
                    .OrderBy(t => t.Talla)
                    .Select(t => t.Talla)
                    .ToList();
            }
        }

        private void CargarTallasEntregado(ComboBox cmbArticulo, ComboBox cmbTipo, ComboBox cmbColor, ComboBox cmbTalla, TextBlock lblDisponibles)
        {
            string articulo = cmbArticulo.SelectedItem?.ToString() ?? "";

            lblDisponibles.Text = "Disponibles: 0";

            if (articulo != "Uniforme")
                return;

            string tipo = cmbTipo.SelectedItem?.ToString() ?? "";

            string color = cmbColor.SelectedItem?.ToString() ?? "";

            using var db = new AppDbContext();

            cmbTalla.ItemsSource = db.UniformesCanje
                .Where(u =>
                    u.Tipo == tipo &&
                    u.Color == color)
                .Select(u => u.Talla)
                .Distinct()
                .OrderBy(x => x)
                .ToList();
        }

        private void MostrarExistencia(ComboBox cmbArticulo, ComboBox cmbTipo, ComboBox cmbColor, ComboBox cmbTalla, TextBlock lblDisponibles)
        {
            string articulo = cmbArticulo.SelectedItem?.ToString() ?? "";

            using var db = new AppDbContext();

            if (articulo == "Uniforme")
            {
                string tipo = cmbTipo.SelectedItem?.ToString() ?? "";

                string color = cmbColor.SelectedItem?.ToString() ?? "";

                string talla = cmbTalla.SelectedItem?.ToString() ?? "";

                var uniforme = db.UniformesCanje
                    .FirstOrDefault(u =>
                        u.Tipo == tipo &&
                        u.Color == color &&
                        u.Talla == talla);

                lblDisponibles.Text = $"Disponibles: {uniforme?.Existencia ?? 0}";
            }
            else if (articulo == "Tenis")
            {
                string talla = cmbTalla.SelectedItem?.ToString() ?? "";

                var tenis = db.TenisCanjes
                    .FirstOrDefault(t =>
                        t.Talla == talla);

                lblDisponibles.Text = $"Disponibles: {tenis?.Existencia ?? 0}";
            }
        }

        private void cmbC1TipoCambio_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AplicarMismaTalla(cmbC1TipoCambio, cmbC1DevArticulo, cmbC1DevTipo, cmbC1DevColor, cmbC1DevTalla, cmbC1EntTipo, cmbC1EntColor, cmbC1EntTalla);
        }

        private void cmbC2TipoCambio_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AplicarMismaTalla(cmbC2TipoCambio, cmbC2DevArticulo, cmbC2DevTipo, cmbC2DevColor, cmbC2DevTalla, cmbC2EntTipo, cmbC2EntColor, cmbC2EntTalla);
        }

        private void AplicarMismaTalla(ComboBox cmbTipoCambio, ComboBox cmbArticulo, ComboBox cmbDevTipo, ComboBox cmbDevColor, ComboBox cmbDevTalla, ComboBox cmbEntTipo, ComboBox cmbEntColor, ComboBox cmbEntTalla)
        {
            string tipoCambio = cmbArticulo.SelectedItem?.ToString() ?? "";

            if (tipoCambio != "MismaTalla")
                return;

            string articulo = cmbArticulo.SelectedItem?.ToString() ?? "";

            if (string.IsNullOrWhiteSpace(articulo))
                return;

            cmbEntTipo.SelectedItem = cmbDevTipo.SelectedItem;

            cmbEntColor.SelectedItem = cmbDevColor.SelectedItem;

            cmbEntTalla.SelectedItem = cmbDevTalla.SelectedItem;
        }


        private void BtnRegistrarCambios_Click(object sender, RoutedEventArgs e)
        {
            using var db = new AppDbContext();
            using var transaccion = db.Database.BeginTransaction();

            try
            {
                var devolucion = new DevolucionClienteCanje
                {
                    Fecha = DateTime.Now,
                    Observacion = txtObservacion.Text.Trim()
                };

                if (!ProcesarCambio(db, devolucion, cmbC1DevArticulo, cmbC1TipoCambio, cmbC1DevTipo, cmbC1DevColor, cmbC1DevTalla, cmbC1EntTipo, cmbC1EntColor, cmbC1EntTalla, 1))
                {
                    return;
                }

                if (cantidadCambios == 2)
                {
                    if (!ProcesarCambio(db, devolucion, cmbC2DevArticulo, cmbC2TipoCambio, cmbC2DevTipo, cmbC2DevColor, cmbC2DevTalla, cmbC2EntTipo, cmbC2EntColor, cmbC2EntTalla, 2))
                    {
                        return;
                    }
                }

                db.DevolucionesClienteCanje.Add(devolucion);

                db.SaveChanges();

                transaccion.Commit();

                MessageBox.Show(
                    cantidadCambios == 1
                        ? "Cambio registrado correctamente."
                        : "Los 2 cambios se registraron correctamente.",
                    "Devolución de cliente",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                transaccion.Rollback();

                MessageBox.Show(
                    ex.InnerException?.Message ?? ex.Message,
                    "Error al registrar los cambios",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private bool ProcesarCambio(AppDbContext db, DevolucionClienteCanje devolucion, ComboBox cmbArticulo, ComboBox cmbTipoCambio, ComboBox cmbDevTipo, ComboBox cmbDevColor, ComboBox cmbDevTalla, ComboBox cmbEntTipo, ComboBox cmbEntColor, ComboBox cmbEntTalla, int numeroCambio)
        {
            string articulo = cmbArticulo.SelectedItem?.ToString() ?? "";

            string tipoCambio = cmbTipoCambio.SelectedItem?.ToString() ?? "";

            if (string.IsNullOrWhiteSpace(articulo))
            {
                MessageBox.Show($"Seleccione el tipo de artículo del Cambio {numeroCambio}.");

                return false;
            }    

            if (string.IsNullOrWhiteSpace(tipoCambio))
            {
                MessageBox.Show($"Seleccione el tipo de cambio del Cambio {numeroCambio}.");

                return false;
            }

            if (articulo == "Uniforme")
            {
                return ProcesarCambioUniforme(db, devolucion, tipoCambio, cmbDevTipo, cmbDevColor, cmbDevTalla, cmbEntTipo, cmbEntColor, cmbEntTalla, numeroCambio);
            }

            if (articulo == "Tenis")
            {
                return ProcesarCambioTenis(db, devolucion, tipoCambio, cmbDevTalla, cmbEntTalla, numeroCambio);
            }

            MessageBox.Show($"El artículo del Cambio {numeroCambio} no es válido.");

            return false;
        }

        private bool ProcesarCambioUniforme(AppDbContext db, DevolucionClienteCanje devolucion, string tipoCambio, ComboBox cmbDevTipo, ComboBox cmbDevColor, ComboBox cmbDevTalla, ComboBox cmbEntTipo, ComboBox cmbEntColor, ComboBox cmbEntTalla, int numeroCambio)
        {
            string devTipo = cmbDevTipo.SelectedItem?.ToString() ?? "";

            string devColor = cmbDevColor.SelectedItem?.ToString() ?? "";

            string devTalla = cmbDevTalla.SelectedItem?.ToString() ?? "";

            string entTipo = cmbEntTipo.SelectedItem?.ToString() ?? "";

            string entColor = cmbEntColor.SelectedItem?.ToString() ?? "";

            string entTalla = cmbEntTalla.SelectedItem?.ToString() ?? "";

            if (string.IsNullOrWhiteSpace(devTipo) ||
                string.IsNullOrWhiteSpace(devColor) ||
                string.IsNullOrWhiteSpace(devTalla))
            {
                MessageBox.Show($"Complete el artículo que devuelve en el Cambio {numeroCambio}.");

                return false;
            }

            if (string.IsNullOrWhiteSpace(entTipo) ||
                string.IsNullOrWhiteSpace(entColor) ||
                string.IsNullOrWhiteSpace(entTalla))
            {
                MessageBox.Show($"Complete el artículo que se entrega en el Cambio {numeroCambio}.");

                return false;
            }

            var devuelto = db.UniformesCanje
                .FirstOrDefault(u =>
                    u.Tipo == devTipo &&
                    u.Color == devColor &&
                    u.Talla == devTalla);

            var entregado = db.UniformesCanje
                .FirstOrDefault(u =>
                    u.Tipo == entTipo &&
                    u.Color == entColor &&
                    u.Talla == entTalla);

            if (devuelto == null)
            {
                MessageBox.Show($"No se encontró el uniforme devuelto del Cambio {numeroCambio}.");

                return false;
            }

            if (entregado == null)
            {
                MessageBox.Show($"No se encontró el uniforme a entregar del Cambio {numeroCambio}.");

                return false;
            }

            devuelto.Existencia++;

            if (entregado.Existencia <= 0)
            {
                devuelto.Existencia--;

                MessageBox.Show(
                    $"No hay existencia disponible del uniforme " +
                    $"que se quiere entregar en el Cambio {numeroCambio}.");


                return false;
            }

            entregado.Existencia--;

            devolucion.Detalles.Add(
                new DetalleDevolucionClienteCanje
                {
                    TipoCambio = tipoCambio,
                    TipoArticulo = "Uniforme",

                    UniformeDevueltoId = devuelto.Id,
                    UniformeEntregadoId = entregado.Id
                });

            return true;
        }

        private bool ProcesarCambioTenis(AppDbContext db, DevolucionClienteCanje devolucion, string tipoCambio, ComboBox cmbDevTalla, ComboBox cmbEntTalla, int numeroCambio)
        {
            string devTalla = cmbDevTalla.SelectedItem?.ToString() ?? "";

            string entTalla = cmbEntTalla.SelectedItem?.ToString() ?? "";

            if (string.IsNullOrWhiteSpace(devTalla))
            {
                MessageBox.Show(
                    $"Seleccione la talla de tenis que devuelve " +
                    $"en el Cambio {numeroCambio}.");

                return false;
            }

            if (string.IsNullOrWhiteSpace(entTalla))
            {
                MessageBox.Show(
                    $"Seleccione la talla de tenis que se entrega " +
                    $"en el Cambio {numeroCambio}.");

                return false;
            }

            var devuelto = db.TenisCanjes
                .FirstOrDefault(t =>
                    t.Talla == devTalla);

            var entregado = db.TenisCanjes
                .FirstOrDefault(t =>
                    t.Talla == entTalla);

            if (devuelto == null)
            {
                MessageBox.Show($"No se encontró el tenis devuelto del Cambio {numeroCambio}.");

                return false;
            }

            if (entregado == null)
            {
                MessageBox.Show($"No se encontró el tenis a entregar del Cambio {numeroCambio}.");

                return false;
            }

            devuelto.Existencia++;

            if (entregado.Existencia <= 0)
            {
                devuelto.Existencia--;

                MessageBox.Show(
                    $"No hay existencia disponible de los tenis" +
                    $"que se quieren entregar en el Cambio {numeroCambio}.");

                return false;
            }

            entregado.Existencia--;

            devolucion.Detalles.Add(
                new DetalleDevolucionClienteCanje
                {
                    TipoCambio = tipoCambio,
                    TipoArticulo = "Tenis",

                    TenisDevueltoId = devuelto.Id,
                    TenisEntregadoId = entregado.Id
                });

            return true;
        }
    }
}
