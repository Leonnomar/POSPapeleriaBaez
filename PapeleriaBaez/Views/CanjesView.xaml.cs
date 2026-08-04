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
    /// Lógica de interacción para CanjesView.xaml
    /// </summary>
    public partial class CanjesView : UserControl
    {
        public CanjesView()
        {
            InitializeComponent();

            CargarCombos();
            CargarComboEntregaPaquetes();
            CargarPaquetes();
            CargarUniformes();
        }

        private void CargarCombos()
        {
            cmbPaquete.ItemsSource = new[] { 1, 2, 3, 4 };

            cmbTipoUniforme.ItemsSource = new[]
            {
                "Falda",
                "Camisa Blanca",
                "Playera Blanca",
                "Jumper",
                "Pantalón",
                "Short/Falda"
            };

            cmbColorUniforme.ItemsSource = null;
            cmbTallaUniforme.ItemsSource = null;
        }

        private void CargarComboEntregaPaquetes()
        {
            using var db = new AppDbContext();

            cmbEntregaPaquete.ItemsSource = db.PaquetesCanje
                .OrderBy(p => p.NumeroPaquete)
                .ToList();

            cmbEntregaPaquete.DisplayMemberPath = "NumeroPaquete";
        }

        private void CargarPaquetes()
        {
            using var db = new AppDbContext();

            var paquetes = db.PaquetesCanje
                .OrderBy(p => p.NumeroPaquete)
                .ToList();

            dgPaquetes.ItemsSource = paquetes;
        }

        private void BtnEntradaPaquete_Click(object sender, RoutedEventArgs e)
        {
            if (cmbPaquete.SelectedItem is not int numeroPaquete)
            {
                MessageBox.Show("Seleccione un paquete.");
                return;
            }

            if (!int.TryParse(txtCantidadPaquete.Text, out int cantidad) ||
                cantidad <= 0)
            {
                MessageBox.Show("Capture una cantidad válida");
                return;
            }

            using var db = new AppDbContext();

            var paquete = db.PaquetesCanje
                .FirstOrDefault(p => p.NumeroPaquete == numeroPaquete);

            if (paquete == null)
            {
                paquete = new PaqueteCanje
                {
                    NumeroPaquete = numeroPaquete,
                    Existencia = cantidad,
                    Entregados = 0
                };

                db.PaquetesCanje.Add(paquete);
            }
            else
            {
                paquete.Existencia += cantidad;
            }

            db.SaveChanges();

            txtCantidadPaquete.Clear();
            cmbPaquete.SelectedIndex = -1;

            CargarPaquetes();

            MessageBox.Show("Entrada de paquete registrada correctamente.");
        }

        private void CargarUniformes()
        {
            using var db = new AppDbContext();

            var uniformes = db.UniformesCanje
                .OrderBy(u => u.Tipo)
                .ThenBy(u => u.Color)
                .ThenBy(u => u.Talla)
                .ToList();

            dgUniformes.ItemsSource = uniformes;
        }

        private void BtnEntradaUniforme_Click(object sender, RoutedEventArgs e)
        {
            string tipo = cmbTipoUniforme.SelectedItem?.ToString() ?? "";
            string color = cmbColorUniforme.SelectedItem?.ToString() ?? "";
            string talla = cmbTallaUniforme.SelectedItem?.ToString() ?? "";

            if (string.IsNullOrWhiteSpace(tipo))
            {
                MessageBox.Show("Seleccione el tipo de uniforme.");
                return;
            }

            if (string.IsNullOrWhiteSpace(color))
            {
                MessageBox.Show("Seleccione el color.");
                return;
            }

            if (string.IsNullOrWhiteSpace(talla))
            {
                MessageBox.Show("Seleccione la talla.");
                return;
            }

            if (!int.TryParse(txtCantidadUniforme.Text, out int cantidad) ||
                cantidad <= 0)
            {
                MessageBox.Show("Capture una cantidad válida.");
                return;
            }

            using var db = new AppDbContext();

            var uniforme = db.UniformesCanje
                .FirstOrDefault(u =>
                    u.Tipo == tipo &&
                    u.Color == color &&
                    u.Talla == talla);

            if (uniforme == null)
            {
                uniforme = new UniformeCanje
                {
                    Tipo = tipo,
                    Color = color,
                    Talla = talla,
                    Existencia = cantidad,
                    Entregados = 0
                };

                db.UniformesCanje.Add(uniforme);
            }
            else
            {
                uniforme.Existencia += cantidad;
            }

            db.SaveChanges();

            cmbTipoUniforme.SelectedIndex = -1;
            cmbColorUniforme.SelectedIndex = -1;
            cmbTallaUniforme.SelectedIndex = -1;
            txtCantidadUniforme.Clear();

            CargarUniformes();

            MessageBox.Show("Entrada de uniformes registrada correctamente.");
        }

        private void BtnRegistrarEntregaPaquete_Click(object sender, RoutedEventArgs e)
        {
            if (cmbEntregaPaquete.SelectedItem is not PaqueteCanje seleccionado)
            {
                MessageBox.Show("Seleccione un paquete.");
                return;
            }

            if (!int.TryParse(txtEntregaPaquete.Text, out int cantidad))
            {
                MessageBox.Show("Cantidad inválida.");
                return;
            }

            using var db = new AppDbContext();

            var paquete = db.PaquetesCanje
                .First(p => p.Id == seleccionado.Id);

            if (paquete == null)
            {
                MessageBox.Show("No se encontró el paquete");
                return;
            }

            if (paquete.Existencia < cantidad)
            {
                MessageBox.Show(
                    $"No hay suficiente paquetes disponibles.\n\n" +
                    $"Disponibles: {paquete.Existencia}\n" +
                    $"Solicitados: {cantidad}",
                    "Existencia insuficiente",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            paquete.Existencia -= cantidad;
            paquete.Entregados += cantidad;

            db.SaveChanges();

            MessageBox.Show(
                "Canje de útiles registrado correctamente.",
                "Canjes",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            txtEntregaPaquete.Text = "1";
            cmbEntregaPaquete.SelectedIndex = -1;

            CargarPaquetes();
            CargarComboEntregaPaquetes();

            lblExistenciaPaquete.Text = "Disponibles: 0";

            ValidarEntregaPaquete();
        }

        private void BtnCapturarConjuntos_Click(object sender, RoutedEventArgs e)
        {
            int conjuntos = rbDosConjuntos.IsChecked == true ? 2 : 1;

            var ventana = new CapturarConjuntosWindows(conjuntos)
            {
                Owner = Application.Current.MainWindow
            };

            if (ventana.ShowDialog() == true)
            {
                CargarUniformes();
                //CargarResumenCanjes();
                //CargarValesPendientes();
            }
        }

        private void cmbTipoUniforme_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbTipoUniforme.SelectedItem is not string tipo)
            {
                cmbColorUniforme.ItemsSource = null;
                cmbTallaUniforme.ItemsSource = null;
                return;
            }

            CargarColoresPorTipo(tipo);
            CargarTallasPorTipo(tipo);
        }

        private void CargarTallasPorTipo(string tipo)
        {
            string[] tallas;

            switch (tipo)
            {
                case "Falda":
                    tallas = new[]
                    {
                        "6", "8", "10", "12", "14",
                        "16", "18/CH", "M", "G"
                    };
                    break;

                case "Camisa Blanca":
                case "Playera Blanca":
                    tallas = new[]
                    {
                        "4", "6", "8", "10", "12",
                        "14", "16", "18/CH", "M", "G"
                    };
                    break;

                case "Jumper":
                    tallas = new[]
                    {
                        "10", "12", "14", "16"
                    };
                    break;

                case "Pantalón":
                    tallas = new[]
                    {
                        "4", "6", "8", "10", "12", "14",
                        "16", "18", "20", "22", "24", "26",
                        "30", "32", "34", "36", "38"
                    };
                    break;

                case "Short/Falda":
                    tallas = new[]
                    {
                        "4", "6", "8"
                    };
                    break;

                default:
                    tallas = Array.Empty<string>();
                    break;
            }

            cmbTallaUniforme.ItemsSource = tallas;
            cmbTallaUniforme.SelectedIndex = -1;
        }

        private void CargarColoresPorTipo(string tipo)
        {
            string[] colores;

            switch (tipo)
            {
                case "Falda":
                    colores = new[]
                    {
                        "Azul",
                        "Tinto"
                    };
                    break;

                case "Camisa Blanca":
                case "Playera Blanca":
                    colores = new[]
                    {
                        "Blanco"
                    };
                    break;

                case "Jumper":
                    colores = new[]
                    {
                        "Rosa",
                        "Celeste",
                        "Tinto"
                    };
                    break;

                case "Pantalón":
                    colores = new[]
                    {
                        "Azul",
                        "Tinto",
                        "Kaki"
                    };
                    break;

                case "Short/Falda":
                    colores = new[]
                    {
                        "Azul"
                    };
                    break;

                default:
                    colores = Array.Empty<string>();
                    break;
            }

            cmbColorUniforme.ItemsSource = colores;
            cmbColorUniforme.SelectedIndex =
                colores.Length == 1 ? 0 : -1;
        }

        private void cmbEntregaPaquete_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbEntregaPaquete.SelectedItem is PaqueteCanje paquete)
            {
                lblExistenciaPaquete.Text =
                    $"Disponibles: {paquete.Existencia}";
            }
            else
            {
                lblExistenciaPaquete.Text = "Disponibles: 0";
            }

            ValidarEntregaPaquete();
        }

        private void txtEntregaPaquete_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidarEntregaPaquete();
        }

        private void ValidarEntregaPaquete()
        {
            if (btnRegistrarEntregaPaquete == null)
                return;

            bool paqueteSeleccionado =
                cmbEntregaPaquete.SelectedItem is PaqueteCanje;

            bool cantidadValida =
                int.TryParse(txtEntregaPaquete.Text, out int cantidad) &&
                cantidad > 0;

            btnRegistrarEntregaPaquete.IsEnabled = paqueteSeleccionado && cantidadValida;

        }

        private void SoloEnteros_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(char.IsDigit);
        }
    }
}
