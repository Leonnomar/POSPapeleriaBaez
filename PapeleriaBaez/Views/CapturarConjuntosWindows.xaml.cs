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
using Microsoft.EntityFrameworkCore;
using PapeleriaBaez.Data;
using PapeleriaBaez.Models;

namespace PapeleriaBaez.Views
{
    /// <summary>
    /// Lógica de interacción para CapturarConjuntosWindows.xaml
    /// </summary>
    public partial class CapturarConjuntosWindows : Window
    {
        private readonly int cantidadConjuntos;

        private List<UniformeCanje> inventarioUniformes = new();
        public CapturarConjuntosWindows(int cantidadConjuntos)
        {
            InitializeComponent();

            this.cantidadConjuntos = cantidadConjuntos;

            grpConjunto2.Visibility =
                cantidadConjuntos == 2
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            CargarUniformes();
        }

        private void CargarUniformes()
        {
            using var db = new AppDbContext();

            inventarioUniformes = db.UniformesCanje
                .OrderBy(u => u.Tipo)
                .ThenBy(u => u.Color)
                .ThenBy(u => u.Talla)
                .ToList();

            var tipos = inventarioUniformes
                .Select(u => u.Tipo)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            cmbC1P1Tipo.ItemsSource = tipos;
            cmbC1P2Tipo.ItemsSource = tipos;
            cmbC2P1Tipo.ItemsSource = tipos;
            cmbC2P2Tipo.ItemsSource = tipos;
        }

        private bool ValidarFormulario()
        {
            if (!ValidarPrenda(cmbC1P1Tipo, cmbC1P1Color, cmbC1P1Talla, "la primera prenda del conjunto 1"))
            {
                return false;
            }

            if (!ValidarPrenda(cmbC1P2Tipo, cmbC1P2Color, cmbC1P2Talla, "la segunda prenda del conjunto 1"))
            {
                return false;
            }

            if (cantidadConjuntos == 2)
            {
                if (!ValidarPrenda(cmbC2P1Tipo, cmbC2P1Color, cmbC2P1Talla, "la primera prenda del conjunto 2"))
                {
                    return false;
                }

                if (!ValidarPrenda(cmbC2P2Tipo, cmbC2P2Color, cmbC2P2Talla, "la segunda prenda del conjunto 2"))
                {
                    return false;
                }
            }

            if (SonLaMismaPrenda(
                cmbC1P1Tipo, cmbC1P1Color, cmbC1P1Talla,
                cmbC1P2Tipo, cmbC1P2Color, cmbC1P2Talla))
            {
                MessageBox.Show(
                    "Las dos prendas del conjunto son iguales.",
                    "Canje de uniformes",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return false;
            }

            if (cantidadConjuntos == 2 &&
                SonLaMismaPrenda(
                    cmbC2P1Tipo, cmbC2P1Color, cmbC2P1Talla,
                    cmbC2P2Tipo, cmbC2P2Color, cmbC2P2Talla))
            {
                MessageBox.Show(
                    "Las dos prendas del conjunto 2 son iguales.",
                    "Canje de uniformes",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return false;
            }

            return true;
        }

        private bool ValidarPrenda(ComboBox cmbTipo, ComboBox cmbColor, ComboBox cmbTalla, string descripcion)
        {
            if (cmbTipo.SelectedItem == null)
            {
                MessageBox.Show(
                    $"Seleccione el tipo de {descripcion}.",
                    "Canje de uniformes",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                cmbTipo.Focus();
                return false;
            }

            if (cmbColor.SelectedItem == null)
            {
                MessageBox.Show(
                    $"Seleccione el color de {descripcion}.",
                    "Canje de uniformes",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                cmbColor.Focus();
                return false;
            }

            if (cmbTalla.SelectedItem == null)
            {
                MessageBox.Show(
                    $"Seleccione la talla de {descripcion}.",
                    "Canje de uniformes",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                cmbTalla.Focus();
                return false;
            }

            return true;
        }

        private bool SonLaMismaPrenda(ComboBox tipo1, ComboBox color1, ComboBox talla1, ComboBox tipo2, ComboBox color2, ComboBox talla2)
        {
            return tipo1.SelectedItem?.ToString() ==
                   tipo2.SelectedItem?.ToString()
                && color1.SelectedItem?.ToString() ==
                   color2.SelectedItem?.ToString()
                && talla1.SelectedItem?.ToString() ==
                   talla2.SelectedItem?.ToString();
        }

        private void BtnRegistrarCanje_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarFormulario())
                return;

            var prendasSeleccionadas =
                new List<(UniformeCanje Prenda, int Conjunto)>();

            var prenda = ObtenerUniforme(cmbC1P1Tipo, cmbC1P1Color, cmbC1P1Talla);

            if (prenda != null)
                prendasSeleccionadas.Add((prenda, 1));

            prenda = ObtenerUniforme(cmbC1P2Tipo, cmbC1P2Color, cmbC1P2Talla);

            if (prenda != null)
                prendasSeleccionadas.Add((prenda, 1));

            if (cantidadConjuntos == 2)
            {
                prenda = ObtenerUniforme(cmbC2P1Tipo, cmbC2P1Color, cmbC2P1Talla);

                if (prenda != null)
                    prendasSeleccionadas.Add((prenda, 2));

                prenda = ObtenerUniforme(cmbC2P2Tipo, cmbC2P2Color, cmbC2P2Talla);

                if (prenda != null)
                    prendasSeleccionadas.Add((prenda, 2));
            }

            using var db = new AppDbContext();
            using var transaccion = db.Database.BeginTransaction();

            try
            {
                var canje = new CanjeUniforme
                {
                    Fecha = DateTime.Now,
                    CantidadConjuntos = cantidadConjuntos
                };

                db.CanjeUniformes.Add(canje);
                db.SaveChanges();

                var pendientes = new List<string>();

                foreach (var seleccion in prendasSeleccionadas)
                {
                    var uniforme = db.UniformesCanje.Find(seleccion.Prenda.Id);

                    if (uniforme == null)
                    {
                        continue;
                    }

                    bool pendiente = uniforme.Existencia <= 0;

                    if (!pendiente)
                    {
                        uniforme.Existencia--;
                        uniforme.Entregados++;
                    }
                    else
                    {
                        pendientes.Add(
                            $"Conjunto {seleccion.Conjunto}: " +
                            $"{uniforme.Tipo}, " +
                            $"{uniforme.Color}, talla {uniforme.Talla}");
                    }

                    db.DetalleCanjeUniformes.Add(
                        new DetalleCanjeUniforme
                        {
                            CanjeUniformeId = canje.Id,
                            UniformeCanjeId = uniforme.Id,
                            NumeroConjunto = seleccion.Conjunto,
                            Cantidad = 1,
                            Pendiente = pendiente
                        });
                }

                db.SaveChanges();
                transaccion.Commit();

                CargarUniformes();

                if (pendientes.Any())
                {
                    MessageBox.Show(
                        "Canje registrado. \n\n" +
                        "Se generaron vales pendientes para:\n\n" +
                        string.Join("\n", pendientes),
                        "Canje registrado con pendientes",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }
                else
                {
                    MessageBox.Show(
                        "Canje de uniformes registrado correctamente.",
                        "Canjes",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                transaccion.Rollback();

                MessageBox.Show(
                    ex.InnerException?.Message ?? ex.Message,
                    "Error al registrar el canje",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void CargarColores(ComboBox cmbTipo, ComboBox cmbColor, ComboBox cmbTalla, TextBlock lblExistencia)
        {
            string tipo = cmbTipo.SelectedItem?.ToString() ?? "";

            var colores = inventarioUniformes
                .Where(u => u.Tipo == tipo)
                .Select(u => u.Color)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            cmbColor.ItemsSource = colores;
            cmbColor.SelectedIndex = colores.Count == 1 ? 0 : -1;

            cmbTalla.ItemsSource = null;
            lblExistencia.Text = "Disponibles: 0";
        }

        private void cmbC1P1Tipo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CargarColores(cmbC1P1Tipo, cmbC1P1Color, cmbC1P1Talla, lblC1P1Existencia);
        }

        private void cmbC1P2Tipo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CargarColores(cmbC1P2Tipo, cmbC1P2Color, cmbC1P2Talla, lblC1P2Existencia);
        }

        private void cmbC2P1Tipo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CargarColores(cmbC2P1Tipo, cmbC2P1Color, cmbC2P1Talla, lblC2P1Existencia);
        }

        private void cmbC2P2Tipo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CargarColores(cmbC2P2Tipo, cmbC2P2Color, cmbC2P2Talla, lblC2P2Existencia);
        }

        private void CargarTallas(ComboBox cmbTipo, ComboBox cmbColor, ComboBox cmbTalla, TextBlock lblExistencia)
        {
            string tipo = cmbTipo.SelectedItem?.ToString() ?? "";
            string color = cmbColor.SelectedItem?.ToString() ?? "";

            var tallas = inventarioUniformes
                .Where(u =>
                    u.Tipo == tipo &&
                    u.Color == color)
                .Select(u => u.Talla)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            cmbTalla.ItemsSource = tallas;
            cmbTalla.SelectedIndex = -1;

            lblExistencia.Text = "Disponibles: 0";
        }

        private void cmbC1P1Color_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CargarTallas(cmbC1P1Tipo, cmbC1P1Color, cmbC1P1Talla, lblC1P1Existencia);
        }

        private void cmbC1P2Color_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CargarTallas(cmbC1P2Tipo, cmbC1P2Color, cmbC1P2Talla, lblC1P2Existencia);
        }

        private void cmbC2P1Color_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CargarTallas(cmbC2P1Tipo, cmbC2P1Color, cmbC2P1Talla, lblC2P1Existencia);
        }

        private void cmbC2P2Color_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CargarTallas(cmbC2P2Tipo, cmbC2P2Color, cmbC2P2Talla, lblC2P2Existencia);
        }

        private void MostrarExistencia(ComboBox cmbTipo, ComboBox cmbColor, ComboBox cmbTalla, TextBlock lblExistencia)
        {
            string tipo = cmbTipo.SelectedItem?.ToString() ?? "";
            string color = cmbColor.SelectedItem?.ToString() ?? "";
            string talla = cmbTalla.SelectedItem?.ToString() ?? "";

            var uniforme = inventarioUniformes.FirstOrDefault(u =>
                u.Tipo == tipo &&
                u.Color == color &&
                u.Talla == talla);

            lblExistencia.Text =
                uniforme == null
                    ? "Disponibles: 0"
                    : $"Disponibles: {uniforme.Existencia}";
        }

        private void cmbC1P1Talla_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MostrarExistencia(cmbC1P1Tipo, cmbC1P1Color, cmbC1P1Talla, lblC1P1Existencia);
        }

        private void cmbC1P2Talla_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MostrarExistencia(cmbC1P2Tipo, cmbC1P2Color, cmbC1P2Talla, lblC1P2Existencia);
        }

        private void cmbC2P1Talla_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MostrarExistencia(cmbC2P1Tipo, cmbC2P1Color, cmbC2P1Talla, lblC2P1Existencia);
        }

        private void cmbC2P2Talla_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MostrarExistencia(cmbC2P2Tipo, cmbC2P2Color, cmbC2P2Talla,lblC2P2Existencia);
        }

        private UniformeCanje? ObtenerUniforme(ComboBox cmbTipo, ComboBox cmbColor,  ComboBox cmbTalla)
        {
            string tipo = cmbTipo.SelectedItem?.ToString() ?? "";
            string color = cmbColor.SelectedItem?.ToString() ?? "";
            string talla = cmbTalla.SelectedItem?.ToString() ?? "";

            return inventarioUniformes.FirstOrDefault(u =>
                u.Tipo == tipo &&
                u.Color == color &&
                u.Talla == talla);
        }
    }
}
