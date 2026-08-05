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

        private readonly string[] prendasSuperiores =
        {
            "Camisa Blanca",
            "Playera Blanca"
        };

        private readonly string[] prendasInferiores =
        {
            "Pantalón",
            "Falda",
            "Jumper",
            "Short/Falda"
        };

        private void CargarUniformes()
        {
            using var db = new AppDbContext();

            inventarioUniformes = db.UniformesCanje
                .OrderBy(u => u.Tipo)
                .ThenBy(u => u.Color)
                .ThenBy(u => u.Talla)
                .ToList();

            var superiores = inventarioUniformes
                .Where(u => prendasSuperiores.Contains(u.Tipo))
                .Select(u => u.Tipo)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            var inferiores = inventarioUniformes
                .Where(u => prendasInferiores.Contains(u.Tipo))
                .Select(u => u.Tipo)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            cmbC1SuperiorTipo.ItemsSource = superiores;
            cmbC1InferiorTipo.ItemsSource = inferiores;
            cmbC2SuperiorTipo.ItemsSource = superiores;
            cmbC2InferiorTipo.ItemsSource = inferiores;
        }

        private bool ValidarFormulario()
        {
            if (!ValidarPrenda(cmbC1SuperiorTipo, cmbC1SuperiorColor, cmbC1SuperiorTalla, "la prenda superior del conjunto 1"))
            {
                return false;
            }

            if (!ValidarPrenda(cmbC1InferiorTipo, cmbC1InferiorColor, cmbC1InferiorTalla, "la prenda inferior del conjunto 1"))
            {
                return false;
            }

            if (cantidadConjuntos == 2)
            {
                if (!ValidarPrenda(cmbC2SuperiorTipo, cmbC2SuperiorColor, cmbC2SuperiorTalla, "la prenda superior del conjunto 2"))
                {
                    return false;
                }

                if (!ValidarPrenda(cmbC2InferiorTipo, cmbC2InferiorColor, cmbC2InferiorTalla, "la prenda inferior del conjunto 2"))
                {
                    return false;
                }
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

        private void BtnRegistrarCanje_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarFormulario())
                return;

            var prendasSeleccionadas =
                new List<(UniformeCanje Prenda, int Conjunto)>();

            var prenda = ObtenerUniforme(cmbC1SuperiorTipo, cmbC1SuperiorColor, cmbC1SuperiorTalla);

            if (prenda != null)
                prendasSeleccionadas.Add((prenda, 1));

            prenda = ObtenerUniforme(cmbC1InferiorTipo, cmbC1InferiorColor, cmbC1InferiorTalla);

            if (prenda != null)
                prendasSeleccionadas.Add((prenda, 1));

            if (cantidadConjuntos == 2)
            {
                prenda = ObtenerUniforme(cmbC2SuperiorTipo, cmbC2SuperiorColor, cmbC2SuperiorTalla);

                if (prenda != null)
                    prendasSeleccionadas.Add((prenda, 2));

                prenda = ObtenerUniforme(cmbC2InferiorTipo, cmbC2InferiorColor, cmbC2InferiorTalla);

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

            cmbColor.ItemsSource = null;
            cmbTalla.ItemsSource = null;
            cmbTalla.SelectedIndex = -1;

            lblExistencia.Text = "Disponibles: 0";

            if (string.IsNullOrWhiteSpace(tipo))
                return;

            var colores = inventarioUniformes
                .Where(u => 
                    string.Equals(
                        u.Tipo.Trim(),
                        tipo,
                        StringComparison.OrdinalIgnoreCase))
                .Select(u => u.Color.Trim())
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(c => c)
                .ToList();

            cmbColor.ItemsSource = colores;
            
            if (colores.Count == 1)
                cmbColor.SelectedIndex = 0;
        }

        private void cmbC1SuperiorTipo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CargarColores(cmbC1SuperiorTipo, cmbC1SuperiorColor, cmbC1SuperiorTalla, lblC1SuperiorExistencia);
        }

        private void cmbC1InferiorTipo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CargarColores(cmbC1InferiorTipo, cmbC1InferiorColor, cmbC1InferiorTalla, lblC1InferiorExistencia);
        }

        private void cmbC2SuperiorTipo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CargarColores(cmbC2SuperiorTipo, cmbC2SuperiorColor, cmbC2SuperiorTalla, lblC2SuperiorExistencia);
        }

        private void cmbC2InferiorTipo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CargarColores(cmbC2InferiorTipo, cmbC2InferiorColor, cmbC2InferiorTalla, lblC2InferiorExistencia);
        }

        private int ObtenerOrdenTalla(string talla)
        {
            return talla.ToUpperInvariant() switch
            {
                "4" => 4,
                "6" => 6,
                "8" => 8,
                "10" => 10,
                "12" => 12,
                "14" => 14,
                "16" => 16,
                "18" => 18,
                "18/CH" => 18,
                "20" => 20,
                "22" => 22,
                "24" => 24,
                "26" => 26,
                "30" => 30,
                "32" => 32,
                "34" => 34,
                "36" => 36,
                "M" => 100,
                "G" => 110,
                _ => 999
            };
        }

        private void CargarTallas(ComboBox cmbTipo, ComboBox cmbColor, ComboBox cmbTalla, TextBlock lblExistencia)
        {
            string tipo = cmbTipo.SelectedItem?.ToString() ?? "";
            string color = cmbColor.SelectedItem?.ToString() ?? "";

            cmbTalla.ItemsSource = null;
            cmbTalla.SelectedIndex = -1;
            lblExistencia.Text = "Disponibles: 0";

            if (string.IsNullOrWhiteSpace(tipo) ||
                string.IsNullOrWhiteSpace(color))
            {
                return;
            }

            var tallas = inventarioUniformes
                .Where(u =>
                    string.Equals(
                        u.Tipo.Trim(),
                        tipo,
                        StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(
                        u.Color.Trim(),
                        color,
                        StringComparison.OrdinalIgnoreCase))
                .Select(u => u.Talla.Trim())
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(t => ObtenerOrdenTalla(t))
                .ThenBy(t => t)
                .ToList();

            cmbTalla.ItemsSource = tallas;
        }

        private void cmbC1SuperiorColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CargarTallas(cmbC1SuperiorTipo, cmbC1SuperiorColor, cmbC1SuperiorTalla, lblC1SuperiorExistencia);
        }

        private void cmbC1InferiorColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CargarTallas(cmbC1InferiorTipo, cmbC1InferiorColor, cmbC1InferiorTalla, lblC1InferiorExistencia);
        }

        private void cmbC2SuperiorColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CargarTallas(cmbC2SuperiorTipo, cmbC2SuperiorColor, cmbC2SuperiorTalla, lblC2SuperiorExistencia);
        }

        private void cmbC2InferiorColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CargarTallas(cmbC2InferiorTipo, cmbC2InferiorColor, cmbC2InferiorTalla, lblC2InferiorExistencia);
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

        private void cmbC1SuperiorTalla_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MostrarExistencia(cmbC1SuperiorTipo, cmbC1SuperiorColor, cmbC1SuperiorTalla, lblC1SuperiorExistencia);
        }

        private void cmbC1InferiorTalla_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MostrarExistencia(cmbC1InferiorTipo, cmbC1InferiorColor, cmbC1InferiorTalla, lblC1InferiorExistencia);
        }

        private void cmbC2SuperiorTalla_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MostrarExistencia(cmbC2SuperiorTipo, cmbC2SuperiorColor, cmbC2SuperiorTalla, lblC2SuperiorExistencia);
        }

        private void cmbC2InferiorTalla_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MostrarExistencia(cmbC2InferiorTipo, cmbC2InferiorColor, cmbC2InferiorTalla,lblC2InferiorExistencia);
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