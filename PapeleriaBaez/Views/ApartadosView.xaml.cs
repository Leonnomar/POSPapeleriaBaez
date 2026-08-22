using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Microsoft.EntityFrameworkCore;
using PapeleriaBaez.Data;
using PapeleriaBaez.Models;

namespace PapeleriaBaez.Views
{
    /// <summary>
    /// Lógica de interacción para ApartadosView.xaml
    /// </summary>
    public partial class ApartadosView : UserControl
    {
        private List<Producto> productos = new();

        private ObservableCollection<ApartadoItem> carrito = new();

        public ApartadosView()
        {
            InitializeComponent();

            CargarProductos();

            dgApartado.ItemsSource = carrito;
        }

        private void CargarProductos()
        {
            using var db = new AppDbContext();

            productos = db.Productos
                .Include(p => p.Categoria)
                .OrderBy(p => p.Nombre)
                .ToList();
        }

        private void txtBuscar_TextChanged(object sender, TextChangedEventArgs e)
        {
            string texto = txtBuscar.Text.Trim();

            if (texto.Length < 2)
            {
                panelResultados.Visibility = Visibility.Collapsed;

                lstResultados.ItemsSource = null;

                return;
            }

            var resultados = productos
                .Where(p =>
                    p.Stock > 0 &&
                    (
                        p.Nombre.Contains(
                            texto,
                            StringComparison.OrdinalIgnoreCase)
                        ||
                        p.Codigo.Contains(
                            texto,
                            StringComparison.OrdinalIgnoreCase)
                    ))
                .Take(15)
                .ToList();

            lstResultados.ItemsSource = resultados;

            panelResultados.Visibility =
                resultados.Any()
                    ? Visibility.Visible
                    : Visibility.Collapsed;
        }

        private void txtBuscar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down &&
                panelResultados.Visibility == Visibility.Visible)
            {
                lstResultados.Focus();
                lstResultados.SelectedIndex = 0;
                return;
            }

            if (e.Key != Key.Enter)
                return;

            var producto = productos.FirstOrDefault(p =>
                p.Codigo.Equals(
                    txtBuscar.Text.Trim(),
                    StringComparison.OrdinalIgnoreCase));

            if (producto != null)
                AgregarProducto(producto);
        }

        private void lstResultados_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter &&
                lstResultados.SelectedItem is Producto producto)
            {
                AgregarProducto(producto);
            }
        }

        private void lstResultados_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lstResultados.SelectedItem is Producto producto)
            {
                AgregarProducto(producto);
            }
        }

        private void AgregarProducto(Producto producto)
        {
            if (producto.Stock <= 0)
            {
                MessageBox.Show(
                    "No hay existencia disponible.");

                return;
            }

            var existente = carrito
                .FirstOrDefault(x =>
                    x.ProductoId == producto.Id);

            if  (existente != null)
            {
                if (existente.Cantidad >= producto.Stock)
                {
                    MessageBox.Show(
                        $"Solo hay {producto.Stock} disponibles.");

                    return;
                }

                existente.Cantidad++;
            }
            else
            {
                carrito.Add(
                    new ApartadoItem
                    {
                        ProductoId = producto.Id,
                        Codigo = producto.Codigo,
                        Nombre = producto.Nombre,
                        Precio = producto.PrecioVenta,
                        Cantidad = 1
                    });
            }

            RefrescarCarrito();

            txtBuscar.Clear();
            txtBuscar.Focus();

            panelResultados.Visibility = Visibility.Collapsed;
        }

        private void RefrescarCarrito()
        {
            dgApartado.ItemsSource = null;
            dgApartado.ItemsSource = carrito;

            ActualizarTotales();
        }

        private void ActualizarTotales()
        {
            decimal total = carrito.Sum(x => x.Importe);

            decimal anticipo = 0;

            decimal.TryParse(txtAnticipo.Text, out anticipo);

            decimal saldo = Math.Max(0, total - anticipo);

            lblTotal.Text = $"Total: {total:C}";

            lblSaldo.Text = $"Saldo: {saldo:C}";
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            carrito.Clear();

            txtCliente.Clear();
            txtAnticipo.Text = "0";

            RefrescarCarrito();
        }

        private void BtnRegistrarApartado_Click(object sender, RoutedEventArgs e)
        {
            string cliente = txtCliente.Text.Trim();

            if (string.IsNullOrWhiteSpace(cliente))
            {
                MessageBox.Show(
                    "Capture el nombre del cliente.",
                    "Apartados",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                txtCliente.Focus();
                return;
            }

            if (carrito.Count == 0)
            {
                MessageBox.Show(
                    "Agregue al menos un producto al apartado.",
                    "Apartados",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            decimal total = carrito.Sum(x => x.Importe);

            if (!decimal.TryParse(txtAnticipo.Text, out decimal anticipo) || anticipo < 0)
            {
                MessageBox.Show(
                    "Capture un anticipo válido.",
                    "Apartados",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                txtAnticipo.Focus();
                return;
            }

            if (anticipo > total)
            {
                MessageBox.Show(
                    $"El anticipo no puede ser mayor al total.\n\n" +
                    $"Total: {total:C}",
                    "Anticipo inválido",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            using var db = new AppDbContext();
            using var transaccion = db.Database.BeginTransaction();

            try
            {
                foreach (var item in carrito)
                {
                    var producto = db.Productos
                        .FirstOrDefault(p =>
                            p.Id == item.ProductoId);

                    if (producto == null)
                    {
                        transaccion.Rollback();

                        MessageBox.Show(
                            $"No se encontró el producto '{item.Nombre}'.",
                            "Apartados",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);

                        return;
                    }

                    if (producto.Stock < item.Cantidad)
                    {
                        transaccion.Rollback();

                        MessageBox.Show(
                            $"No hay suficiente existencia de '{producto.Nombre}'.\n\n" +
                            $"Disponible: {producto.Stock}\n" +
                            $"Solicitado: {item.Cantidad}",
                            "Stock insuficiente",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);

                        return;
                    }
                }

                var apartado = new Apartado
                {
                    Cliente = cliente,
                    Fecha = DateTime.Now,
                    FechaEntrega = null,
                    Total = total,
                    Pagado = anticipo,
                    SaldoPendiente = total - anticipo,
                    Estado = "Pendiente"
                };

                db.Apartados.Add(apartado);

                db.SaveChanges();

                foreach (var item in carrito)
                {
                    var producto = db.Productos
                        .First(p =>
                            p.Id == item.ProductoId);

                    producto.Stock -= item.Cantidad;

                    db.detalleApartados.Add(
                        new DetalleApartado
                        {
                            ApartadoId = apartado.Id,
                            ProductoId = producto.Id,
                            Cantidad = item.Cantidad,
                            Precio = item.Precio,
                            Importe = item.Importe
                        });
                }

                if (anticipo > 0)
                {
                    db.abonoApartados.Add(
                        new AbonoApartado
                        {
                            ApartadoId = apartado.Id,
                            Fecha = DateTime.Now,
                            Monto = anticipo
                        });
                }

                db.SaveChanges();
                transaccion.Commit();

                MessageBox.Show(
                    $"Apartado registrado correctamente.\n\n" +
                    $"Folio: #{apartado.Id}\n" +
                    $"Cliente: {apartado.Cliente}\n" +
                    $"Total: {apartado.Total:C}\n" +
                    $"Anticipo: {apartado.Pagado:C}\n" +
                    $"Saldo: {apartado.SaldoPendiente:C}",
                    "Apartado registrado",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                carrito.Clear();

                txtCliente.Clear();
                txtAnticipo.Text = "0";

                RefrescarCarrito();
                CargarProductos();

                txtBuscar.Focus();
            }
            catch (Exception ex)
            {
                transaccion.Rollback();

                MessageBox.Show(
                    ex.InnerException?.Message ?? ex.Message,
                    "Error al registrar apartado",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void txtAnticipo_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (lblTotal == null)
                return;

            ActualizarTotales();
        }
    }
}
