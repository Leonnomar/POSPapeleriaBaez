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
    /// Lógica de interacción para DeudasView.xaml
    /// </summary>
    public partial class DeudasView : UserControl
    {
        private List<Deuda> listaDeudas = new();

        private List<AbonoDeuda> listaAbonos = new();

        private int? deudaSeleccionadaId;

        private enum FiltroDeudas
        {
            Pendientes,
            Pagadas,
            Todas
        }

        private FiltroDeudas filtroActual = FiltroDeudas.Pendientes;
        public DeudasView()
        {
            InitializeComponent();

            CargarDeudas();
        }

        private void CargarDeudas()
        {
            using var db = new AppDbContext();

            var consulta = db.Deudas.AsQueryable();

            switch (filtroActual)
            {
                case FiltroDeudas.Pendientes:
                    consulta = consulta.Where(d => !d.Pagada);
                    break;

                case FiltroDeudas.Pagadas:
                    consulta = consulta.Where(d => d.Pagada);
                    break;

                case FiltroDeudas.Todas:
                    break;
            }

            listaDeudas = consulta
                .OrderByDescending(d => d.Fecha)
                .ToList();
            
            dgDeudas.ItemsSource = listaDeudas;
        }

        private void BtnRegistrarDeuda_Click(object sender, RoutedEventArgs e)
        {
            string cliente = txtCliente.Text.Trim();
            string concepto = txtConcepto.Text.Trim();

            if (string.IsNullOrWhiteSpace(cliente))
            {
                MessageBox.Show(
                    "Capture el nombre del cliente.",
                    "Deudas",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                txtCliente.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(concepto))
            {
                MessageBox.Show(
                    "Capture el concepto de la deuda.",
                    "Deudas",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                txtConcepto.Focus();
                return;
            }

            if (!decimal.TryParse(txtMonto.Text, out decimal monto) || monto <= 0)
            {
                MessageBox.Show(
                    "Capture un monto válido.",
                    "Deudas",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                txtMonto.Focus();
                return;
            }

            using var db = new AppDbContext();

            var deuda = new Deuda
            {
                Cliente = cliente,
                Concepto = concepto,
                Fecha = DateTime.Now,
                MontoOriginal = monto,
                SeldoPendiente = monto,
                Pagada = false
            };

            db.Deudas.Add(deuda);
            db.SaveChanges();

            txtCliente.Clear();
            txtConcepto.Clear();
            txtMonto.Clear();

            CargarDeudas();

            MessageBox.Show(
                "Deuda registrada correctamente.",
                "Deudas",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            txtCliente.Focus();
        }

        private void dgDeudas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgDeudas.SelectedItem is not Deuda deuda)
            {
                deudaSeleccionadaId = null;

                lblDeudaSeleccionada.Text = "Seleccione una deuda";

                lblSaldo.Text = "Saldo: $0.00";

                dgAbonos.ItemsSource = null;

                return;
            }

            deudaSeleccionadaId = deuda.Id;

            lblDeudaSeleccionada.Text = $"{deuda.Cliente} - {deuda.Concepto}";

            lblSaldo.Text = $"{deuda.SeldoPendiente:C}";

            CargarAbonos(deuda.Id);
        }

        private void CargarAbonos(int deudaId)
        {
            using var db = new AppDbContext();

            listaAbonos = db.AbonosDeudas
                .Where(a => a.DeudaId == deudaId)
                .OrderByDescending(a => a.Fecha)
                .ToList();

            dgAbonos.ItemsSource = listaAbonos;
        }


        private void BtnRegistrarAbono_Click(object sender, RoutedEventArgs e)
        {
            if (deudaSeleccionadaId == null)
            {
                MessageBox.Show(
                    "Seleccione una deuda.",
                    "Deudas",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (!decimal.TryParse(txtAbono.Text, out decimal monto) || monto <= 0)
            {
                MessageBox.Show(
                    "Capture un abono válido.",
                    "Deudas",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                txtAbono.Focus();
                return;
            }

            using var db = new AppDbContext();

            var deuda = db.Deudas
                .FirstOrDefault(d => d.Id == deudaSeleccionadaId.Value);

            if (deuda == null)
            {
                MessageBox.Show(
                    "No se encontró la deuda.",
                    "Deudas",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (deuda.Pagada)
            {
                MessageBox.Show(
                    "Esta deuda ya está pagada.",
                    "Deudas",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                return;
            }

            if (monto > deuda.SeldoPendiente)
            {
                MessageBox.Show(
                    $"El abono no puede se mayor al saldo pendiente.\n\n" +
                    $"Saldo actual: {deuda.SeldoPendiente:C}",
                    "Monto inválido",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            var abono = new AbonoDeuda
            {
                DeudaId = deuda.Id,
                Fecha = DateTime.Now,
                Monto = monto
            };

            db.AbonosDeudas.Add(abono);

            deuda.SeldoPendiente -= monto;

            if (deuda.SeldoPendiente <= 0)
            {
                deuda.SeldoPendiente = 0;
                deuda.Pagada = true;
            }

            db.SaveChanges();

            txtAbono.Clear();

            if (deuda.Pagada)
            {
                MessageBox.Show(
                    $"La deuda de {deuda.Cliente} quedó liquidada.",
                    "Deuda pagada",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                deudaSeleccionadaId = null;

                lblDeudaSeleccionada.Text = "Seleccione una deuda";

                lblSaldo.Text = "Saldo: $0.00";

                dgAbonos.ItemsSource = null;
            }
            else
            {
                MessageBox.Show(
                    $"Abono registrado correctamente. \n\n" +
                    $"Saldo pendiente: {deuda.SeldoPendiente:C}",
                    "Deudas",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                lblSaldo.Text = $"Saldo: {deuda.SeldoPendiente:C}";

                CargarAbonos(deuda.Id);
            }

            CargarDeudas();
        }

        private void BtnPendientes_Click(object sender, RoutedEventArgs e)
        {
            filtroActual = FiltroDeudas.Pendientes;

            LimpiarSeleccionDeuda();
            CargarDeudas();
        }

        private void BtnPagadas_Click(object sender, RoutedEventArgs e)
        {
            filtroActual = FiltroDeudas.Pagadas;

            LimpiarSeleccionDeuda();
            CargarDeudas();
        }

        private void BtnTodas_Click(object sender, RoutedEventArgs e)
        {
            filtroActual = FiltroDeudas.Todas;

            LimpiarSeleccionDeuda();
            CargarDeudas();
        }

        private void LimpiarSeleccionDeuda()
        {
            deudaSeleccionadaId = null;

            dgDeudas.SelectedItem = null;
            dgAbonos.ItemsSource = null;

            txtAbono.Clear();

            lblDeudaSeleccionada.Text = "Seleccione una deuda";

            lblSaldo.Text = "Saldo: $0.00";
        }
    }
}
