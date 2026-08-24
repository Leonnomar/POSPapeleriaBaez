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
using Microsoft.EntityFrameworkCore;

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
            CargarComboEntregaTenis();
            CargarTenis();
            CargarValesPendientes();
            CargarResumenCanjes();
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

            cmbTallaTenis.ItemsSource = new[]
            {
                "14", "15", "16", "17", "18", "19"
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

        private void CargarComboEntregaTenis()
        {
            using var db = new AppDbContext();

            cmbEntregaTenis.ItemsSource = db.TenisCanjes
                .OrderBy(t => t.Talla)
                .ToList();

            cmbEntregaTenis.DisplayMemberPath = "Talla";
        }

        private void CargarValesPendientes()
        {
            using var db = new AppDbContext();

            var vales = db.DetalleCanjeUniformes
                .Include(d => d.CanjeUniforme)
                .Include(d => d.UniformeCanje)
                .Where(d => d.Pendiente)
                .OrderByDescending(d => d.CanjeUniforme.Fecha)
                .Select(d => new ValePendienteGrid
                {
                    DetalleId = d.Id,
                    Fecha = d.CanjeUniforme.Fecha,
                    NumeroConjunto = d.NumeroConjunto,
                    Tipo = d.UniformeCanje.Tipo,
                    Color = d.UniformeCanje.Color,
                    Talla = d.UniformeCanje.Talla
                })
                .ToList();

            dgVales.ItemsSource = vales;
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
            CargarComboEntregaPaquetes();
            CargarResumenCanjes();

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

        private void CargarTenis()
        {
            using var db = new AppDbContext();

            dgTenis.ItemsSource = db.TenisCanjes
                .OrderBy(t => t.Talla)
                .ToList();
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
            CargarValesPendientes();
            CargarResumenCanjes();

            MessageBox.Show("Entrada de uniformes registrada correctamente.");
        }

        private void BtnEntradaTenis_Click(object sender, RoutedEventArgs e)
        {
            string talla = cmbTallaTenis.SelectedItem?.ToString() ?? "";

            if (string.IsNullOrWhiteSpace(talla))
            {
                MessageBox.Show("Seleccione la talla.");
                return;
            }

            if (!int.TryParse(txtCantidadTenis.Text, out int cantidad) || cantidad <= 0)
            {
                MessageBox.Show("Capture una cantidad válida.");
                return;
            }

            using var db = new AppDbContext();

            var tenis = db.TenisCanjes
                .FirstOrDefault(t => t.Talla == talla);

            if (tenis == null)
            {
                tenis = new TenisCanje
                {
                    Talla = talla,
                    Existencia = cantidad,
                    Entregados = 0
                };

                db.TenisCanjes.Add(tenis);
            }
            else
            {
                tenis.Existencia += cantidad;
            }

            db.SaveChanges();

            txtCantidadTenis.Clear();
            cmbTallaTenis.SelectedIndex = -1;

            CargarTenis();
            CargarComboEntregaTenis();
            CargarResumenCanjes();

            MessageBox.Show(
                "Entrada de tenis registrada correctamente.",
                "Tenis",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
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
            CargarResumenCanjes();

            lblExistenciaPaquete.Text = "Disponibles: 0";

            ValidarEntregaPaquete();
        }

        private void BtnEntregarVale_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button boton ||
                boton.Tag is not ValePendienteGrid vale)
            {
                return;
            }

            using var db = new AppDbContext();
            using var transaccion = db.Database.BeginTransaction();

            try
            {
                var detalle = db.DetalleCanjeUniformes
                    .Include(d => d.UniformeCanje)
                    .FirstOrDefault(d => d.Id == vale.DetalleId);

                if (detalle == null)
                {
                    MessageBox.Show(
                        "No se encontró el vale pendiente.",
                        "Vales",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }

                if (!detalle.Pendiente)
                {
                    MessageBox.Show(
                        "Este vale ya fue entregado.",
                        "Vales",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    CargarValesPendientes();
                    return;
                }

                var uniforme = detalle.UniformeCanje;

                if (uniforme.Existencia <= 0)
                {
                    MessageBox.Show(
                        $"Todavía no hay existencia de: \n\n" +
                        $"{uniforme.Tipo}\n" +
                        $"Color: {uniforme.Color}\n" +
                        $"Talla: {uniforme.Talla}",
                        "Sin existencia",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }

                var resultado = MessageBox.Show(
                    $"¿Desea entregar este vale?\n\n" +
                    $"{uniforme.Tipo}\n" +
                    $"Color: {uniforme.Color}\n" +
                    $"Talla: {uniforme.Talla}",
                    "Entregar vale",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (resultado != MessageBoxResult.Yes)
                    return;

                uniforme.Existencia--;
                uniforme.Entregados++;

                detalle.Pendiente = false;

                db.SaveChanges();
                transaccion.Commit();

                MessageBox.Show(
                    "Vale entregado correctamente.",
                    "Vales",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                CargarUniformes();
                CargarValesPendientes();
                CargarResumenCanjes();
            }
            catch (Exception ex)
            {
                transaccion.Rollback();

                MessageBox.Show(
                    ex.InnerException?.Message ?? ex.Message,
                    "Error al entregar el vale",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
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
                CargarResumenCanjes();
                CargarValesPendientes();
            }
        }

        private void BtnRegistrarEntregaTenis_Click(object sender, RoutedEventArgs e)
        {
            if (cmbEntregaTenis.SelectedItem is not TenisCanje seleccionado)
            {
                MessageBox.Show(
                    "Seleccione una talla.",
                    "Canje de tenis",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            using var db = new AppDbContext();

            var tenis = db.TenisCanjes
                .FirstOrDefault(t => t.Id == seleccionado.Id);

            if (tenis == null)
            {
                MessageBox.Show("No se encontró la talla seleccionada.");

                return;
            }

            if (tenis.Existencia <= 0)
            {
                MessageBox.Show(
                    $"No hay tenis disponibles en talla {tenis.Talla}.\n\n" +
                    "Esta talla deberá registrarse como vale pendiente.",
                    "Sin existencia",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                return;
            }

            tenis.Existencia--;
            tenis.Entregados++;

            db.SaveChanges();

            MessageBox.Show(
                $"Canje de tenis registrado correctamente.\n\n" +
                $"Talla: {tenis.Talla}",
                "Caje de tenis",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            cmbEntregaTenis.SelectedIndex = -1;

            lblExistenciaTenis.Text = "Disponibles: 0";

            btnRegistrarEntregaTenis.IsEnabled = false;

            CargarTenis();
            CargarComboEntregaTenis();

            CargarResumenCanjes();
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

        private void cmbEntregaTenis_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbEntregaTenis.SelectedItem is TenisCanje tenis)
            {
                lblExistenciaTenis.Text = $"Disponibles: {tenis.Existencia}";

                btnRegistrarEntregaTenis.IsEnabled = true;
            }
            else
            {
                lblExistenciaTenis.Text = "Disponibles: 0";

                btnRegistrarEntregaTenis.IsEnabled = false;
            }
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
                        "12", "14", "16", "18/CH"
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
                        "Marino",
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

        private void CargarResumenCanjes()
        {
            using var db = new AppDbContext();

            int paquetesEntregados = db.PaquetesCanje
                .Sum(p => (int?)p.Entregados) ?? 0;

            int canjesUniforme = db.CanjeUniformes.Count();

            int conjuntosEntregados = db.CanjeUniformes
                .Sum(c => (int?)c.CantidadConjuntos) ?? 0;

            int prendasEntregadas = db.UniformesCanje
                .Sum(u => (int?)u.Entregados) ?? 0;

            int valesPendientes = db.DetalleCanjeUniformes
                .Count(d => d.Pendiente);

            lblPaquetesEntregados.Text =
                paquetesEntregados.ToString();

            lblCanjesUniforme.Text =
                canjesUniforme.ToString();

            lblConjuntosEntregados.Text =
                conjuntosEntregados.ToString();

            lblPrendasEntregadas.Text =
                prendasEntregadas.ToString();

            lblValesPendientes.Text =
                valesPendientes.ToString();

            lblValesPendientes.Foreground =
                valesPendientes > 0
                    ? Brushes.Red
                    : Brushes.Green;
            var resumenPaquete = db.PaquetesCanje
                .OrderBy(p => p.NumeroPaquete)
                .Select(p => new ResumenPaqueteGrid
                {
                    NumeroPaquete = p.NumeroPaquete,
                    Existencia = p.Existencia,
                    Entregados = p.Entregados
                })
                .ToList();

            dgResumenPaquetes.ItemsSource =
                resumenPaquete;

            var resumenUniformes = db.UniformesCanje
                .GroupBy(u => u.Tipo)
                .Select(grupo => new ResumenUniformeGrid
                {
                    Tipo = grupo.Key,
                    Existencia = grupo.Sum(u => u.Existencia),
                    Entregados = grupo.Sum(u =>u.Entregados)
                })
                .OrderBy(u => u.Tipo)
                .ToList();

            dgResumenUniformes.ItemsSource =
                resumenUniformes;


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
