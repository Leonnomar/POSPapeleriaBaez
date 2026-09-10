using Microsoft.EntityFrameworkCore;
using PapeleriaBaez.Data;
using PapeleriaBaez.Models;
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

namespace PapeleriaBaez.Views
{
    /// <summary>
    /// Lógica de interacción para CapturarCierreWindow.xaml
    /// </summary>
    public partial class CapturarCierreWindow : Window
    {
        private readonly int salidaHeladoId;

        private List<CierreHeladosItem> items = new();

        public CapturarCierreWindow(int salidaHeladosId)
        {
            InitializeComponent();

            this.salidaHeladoId = salidaHeladosId;

            CargarHielera();
        }

        private void CargarHielera()
        {
            using var db = new AppDbContext();

            var salida = db.SalidasHelados
                .Include(s => s.Detalles)
                .ThenInclude(d => d.Producto)
                .FirstOrDefault(s => s.Id == salidaHeladoId);

            if (salida == null)
            {
                MessageBox.Show(
                    "No se encontró la hielera.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                Close();
                return;
            }

            if (salida.Estado != "Pendiente")
            {
                MessageBox.Show(
                    "Esta hielera ya fue cerrada.",
                    "Hielera cerrada",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                Close();
                return;
            }

            lblInformacionHielera.Text = $"Hielera #{salida.Id}   •   Salida: {salida.FechaSalida:dd/MM/yyyy HH:mm}";

            items = salida.Detalles
                .Select(d => new CierreHeladosItem
                {
                    DetalleId = d.Id,
                    ProductoId = d.ProductoId,
                    Codigo = d.Producto.Codigo,
                    Nombre = d.Producto.Nombre,
                    CantidadSalida = d.CantidadSalida,
                    CantidadRegresada = 0,
                    Precio = d.Precio
                })
                .ToList();
            
            dgCierreHielera.ItemsSource = items;

            ActualizarTotales();
        }

        private void TxtDineroRecibido_TextChanged(object sender, TextChangedEventArgs e)
        {
            ActualizarTotales();
        }

        private void ActualizarTotales()
        {
            decimal totalVendido = items.Sum(x => x.Importe);

            decimal dineroRecibido = 0m;

            if (decimal.TryParse(txtDineroRecibido?.Text, out decimal recibido))
            {
                dineroRecibido = recibido;
            }

            decimal deuda = totalVendido - dineroRecibido;

            if (deuda < 0)
                deuda = 0;

            if (lblTotalVendido != null)
                lblTotalVendido.Text = totalVendido.ToString("C2");

            if (lblDeudaGenerada != null)
                lblDeudaGenerada.Text = deuda.ToString("C2");
        }

        private void BtnCerrarHielera_Click(object sender, RoutedEventArgs e)
        {
            using var db = new AppDbContext();

            using var transaccion = db.Database.BeginTransaction();

            try
            {
                var salida = db.SalidasHelados
                    .Include(s => s.Detalles)
                    .FirstOrDefault(s => s.Id == salidaHeladoId);

                if (salida == null)
                {
                    MessageBox.Show(
                        "No  se encontró la hielera.",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    return;
                }

                if (salida.Estado != "Pendiente")
                {
                    MessageBox.Show(
                        "Esta hielera ya fue cerrada.",
                        "Hielera cerrada",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }

                decimal dineroRecibido = 0m;

                if (!string.IsNullOrWhiteSpace(txtDineroRecibido.Text))
                {
                    if (!decimal.TryParse(txtDineroRecibido.Text, out dineroRecibido))
                    {
                        MessageBox.Show(
                            "El dinero recibido no es válido.",
                            "Cantidad inválida",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);

                        return;
                    }
                }

                if (dineroRecibido < 0)
                {
                    MessageBox.Show(
                        "El dinero recibido no puede ser negativo.",
                        "Cantidad inválida",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }

                foreach (var item in items)
                {
                    if (item.CantidadRegresada < 0)
                    {
                        MessageBox.Show(
                            $"La cantidad regresada de \"{item.Nombre}\" no puede ser negativa.",
                            "Cantidad inválida",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);

                        return;
                    }

                    if (item.CantidadRegresada > item.CantidadSalida)
                    {
                        MessageBox.Show(
                            $"No puedes regresar más unidades de \"{item.Nombre}\" " +
                            $"de las que salieron.\n\n" +
                            $"Salieron: {item.CantidadSalida}\n" +
                            $"Regresaron: {item.CantidadRegresada}",
                            "Cantidad inválida",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);

                        return;
                    }
                }

                decimal totalVendido = 0m;

                foreach (var item in items)
                {
                    var detalle = salida.Detalles
                        .FirstOrDefault(d => d.Id == item.DetalleId);

                    if (detalle == null)
                        continue;

                    detalle.CantidadRegresada = item.CantidadRegresada;

                    int cantidadVendida = detalle.CantidadSalida - detalle.CantidadRegresada;

                    totalVendido += cantidadVendida * detalle.Precio;

                    var producto = db.Productos
                        .FirstOrDefault(p => p.Id == detalle.ProductoId);

                    if (producto != null)
                    {
                        producto.Stock += detalle.CantidadRegresada;
                    }
                }

                decimal deudaGenerada = totalVendido - dineroRecibido;

                if (deudaGenerada < 0)
                    deudaGenerada = 0;

                salida.TotalVendido = totalVendido;
                salida.DineroRecibido = dineroRecibido;
                salida.SaldoPendiente = deudaGenerada;
                salida.FechaCierre = DateTime.Now;
                salida.Estado = "Cerrada";

                db.SaveChanges();

                transaccion.Commit();

                MessageBox.Show(
                    $"La hielera #{salida.Id} fue cerrada correctamente.\n\n" +
                    $"Total vendido: {totalVendido:C2}\n" +
                    $"Dinero recibido: {dineroRecibido:C2}\n" +
                    $"Deuda generada: {deudaGenerada:C2}",
                    "Cierre registrado",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                try
                {
                    transaccion.Rollback();
                }
                catch
                {
                }

                MessageBox.Show(
                    ex.InnerException?.Message ?? ex.Message,
                    "Error al cerrar la hielera",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }

    public class CierreHeladosItem
    {
        public int DetalleId { get; set; }

        public int ProductoId { get; set; }

        public string Codigo { get; set; } = "";

        public string Nombre { get; set; } = "";

        public int CantidadSalida { get; set; }

        public int CantidadRegresada { get; set; }

        public decimal Precio { get; set; }

        public int CantidadVendida => CantidadSalida - CantidadRegresada;

        public decimal Importe => CantidadVendida * Precio;
    }
}
