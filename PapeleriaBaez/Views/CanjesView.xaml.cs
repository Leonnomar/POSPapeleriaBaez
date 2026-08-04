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

            cmbColorUniforme.ItemsSource = new[]
            {
                "Azul",
                "Tinto",
                "Kaki",
                "Rosa",
                "Celeste",
                "Blanco"
            };

            cmbTallaUniforme.ItemsSource = new[]
            {
                "4", "6", "8", "10", "12", "14", "16",
                "18/CH", "M", "G",
                "20", "22", "24", "26", "30", "32", "34", "36"
            };
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

            if (!string.IsNullOrWhiteSpace(talla))
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
            MessageBox.Show("Registro de entrega de útiles pendiente.");
        }

        private void BtnCapturarConjuntos_Click(object sender, RoutedEventArgs e)
        {
            int conjuntos = rbDosConjuntos.IsChecked == true ? 2 : 1;

            MessageBox.Show($"Se capturarán {conjuntos} conjunto(s).");
        }
    }
}
