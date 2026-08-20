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
using Microsoft.Extensions.Logging.Abstractions;
using PapeleriaBaez.Data;
using PapeleriaBaez.Models;

namespace PapeleriaBaez.Views
{
    /// <summary>
    /// Lógica de interacción para VentasView.xaml
    /// </summary>
    public partial class VentasView : UserControl
    {
        private List<Producto> productos = new();

        private ObservableCollection<VentaItem> carrito = new();

        private VentaItem? productoSeleccionado;

        public VentasView()
        {
            InitializeComponent();

            CargarProductos();
        }

        private void CargarProductos()
        {
            using var db = new AppDbContext();

            productos = db.Productos
                          .Include(p => p.Categoria)
                          .ToList();
        }

        private void BuscarProducto()
        {
            string texto = txtBuscar.Text.Trim();

            if (string.IsNullOrWhiteSpace(texto))
                return;

            var producto = BuscarProductoPorTexto(texto);

            if (producto == null)
            {
                MessageBox.Show("Producto no encontrado.");
                txtBuscar.SelectAll();
                txtBuscar.Focus();
                return;
            }

            AgregarProducto(producto);
        }

        private Producto? BuscarProductoPorTexto(string texto)
        {
            return productos.FirstOrDefault(p =>
                p.Codigo.Equals(texto, StringComparison.OrdinalIgnoreCase) ||
                p.Nombre.Contains(texto, StringComparison.OrdinalIgnoreCase));
        }

        private void MostrarResultados(List<Producto> resultados)
        {
            lstResultados.ItemsSource = resultados;

            panelResultados.Visibility =
                resultados.Any()
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void LimpiarBusqueda()
        {
            txtBuscar.Clear();
            txtBuscar.Focus();

            lstResultados.ItemsSource = null;
            panelResultados.Visibility = Visibility.Collapsed;
        }

        private void AgregarProducto(Producto producto)
        {
            if (producto.Stock <= 0)
            {
                MessageBox.Show(
                    $"El producto '{producto.Nombre}' está agotado.",
                    "Sin existencias",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                LimpiarBusqueda();
                return;
            }

            var existente = carrito.FirstOrDefault(x => x.ProductoId == producto.Id);

            if (existente != null)
            {
                if (existente.Cantidad >= producto.Stock)
                {
                    MessageBox.Show(
                        $"Solo hay {producto.Stock} piezas disponibles de '{producto.Nombre}'.",
                        "Stock insuficiente",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    LimpiarBusqueda();
                    return;
                }

                existente.Cantidad++;

                RefrescarCarrito();
                LimpiarBusqueda();

                return;
            }

            carrito.Add(new VentaItem
            {
                ProductoId = producto.Id,
                Codigo = producto.Codigo,
                Nombre = producto.Nombre,
                Precio = producto.PrecioVenta,
                Cantidad = 1
            });

            RefrescarCarrito();
            LimpiarBusqueda();

        }

        private void ActualizarCantidad(VentaItem item, int nuevaCantidad)
        {
            var producto = productos.FirstOrDefault(p => p.Id == item.ProductoId);

            if (producto == null)
                return;

            if (nuevaCantidad <= 0)
            {
                carrito.Remove(item);
                RefrescarCarrito();
                return;
            }

            if (nuevaCantidad > producto.Stock)
            {
                MessageBox.Show($"Solo hay {producto.Stock} piezas disponibles.");
                RefrescarCarrito();
                return;
            }

            item.Cantidad = nuevaCantidad;

            RefrescarCarrito();
        }

        private void RefrescarCarrito()
        {
            dgVenta.ItemsSource = null;
            dgVenta.ItemsSource = carrito;

            ActualizarTotales();
        }

        private void ActualizarTotales()
        {
            decimal subtotal = carrito.Sum(x => x.Importe);

            lblSubtotal.Text = $"Subtotal: {subtotal:C}";
            lblDescuento.Text = $"Descuento: $0.00";
            lblTotal.Text = $"TOTAL: {subtotal:C}";
        }

        private void txtBuscar_TextChanged(object sender, TextChangedEventArgs e)
        {
            string texto = txtBuscar.Text.Trim();

            if (texto.Length < 2)
            {
                lstResultados.ItemsSource = null;

                panelResultados.Visibility = Visibility.Collapsed;

                return;
            }

            var resultados = productos
                .Where(p =>
                    p.Nombre.Contains(texto, StringComparison.OrdinalIgnoreCase) ||
                    p.Codigo.Contains(texto, StringComparison.OrdinalIgnoreCase) ||
                    p.Categoria!.Nombre.Contains(texto, StringComparison.OrdinalIgnoreCase))
                .OrderBy(p => p.Nombre)
                .Take(15)
                .ToList();

            MostrarResultados(resultados);
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

            BuscarProducto();
        }

        private void lstResultados_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down &&
                panelResultados.Visibility == Visibility.Visible)
            {
                lstResultados.Focus();
                lstResultados.SelectedIndex = 0;
                return;
            }

            if (e.Key == Key.Escape)
            {
                panelResultados.Visibility = Visibility.Collapsed;
                txtBuscar.Focus();
                return;
            }

            if (e.Key != Key.Enter)
                return;

            if (lstResultados.SelectedItem is not Producto producto)
                return;

            AgregarProducto(producto);

            panelResultados.Visibility = Visibility.Collapsed;
        }

        private void lstResultados_MouseDoubleClick(object sender , MouseButtonEventArgs e)
        {
            if (lstResultados.SelectedItem is not Producto producto)
                return;

            AgregarProducto(producto);

            panelResultados.Visibility = Visibility.Collapsed;
        }

        private void TxtCantidad_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            if (sender is not TextBox txt)
                return;

            if (txt.Tag is not VentaItem item)
                return;

            if (!int.TryParse(txt.Text, out int cantidad))
            {
                RefrescarCarrito();
                return;
            }

            ActualizarCantidad(item, cantidad);
        }

        private void TxtCantidad_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is not TextBox txt)
                return;

            if (txt.Tag is not VentaItem item)
                return;

            if (!int.TryParse(txt.Text, out int cantidad))
            {
                RefrescarCarrito();
                return;
            }

            ActualizarCantidad(item, cantidad);
        }

        private void GuardarVenta(CobroWindow cobro)
        {
            List<string> alertasStock = new();

            if (carrito.Count == 0)
            {
                MessageBox.Show("No hay productos para vender.");
                return;
            }

            using var db = new AppDbContext();

            using var transaccion = db.Database.BeginTransaction();

            try
            {
                var venta = new Venta
                {
                    Fecha = DateTime.Now,

                    Subtotal = cobro.Subtotal,
                    PorcentajeDescuento = cobro.PorcentajeDescuento,
                    Descuento = cobro.Descuento,
                    Total = cobro.TotalFinal
                };

                db.Ventas.Add(venta);
                db.SaveChanges();

                foreach (var item in carrito)
                {
                    var producto = db.Productos
                        .First(p => p.Id == item.ProductoId);

                    if (producto == null)
                        continue;

                    if (producto.Stock < item.Cantidad)
                    {
                        transaccion.Rollback();

                        MessageBox.Show(
                            $"No hay suficiente stock de {producto.Nombre}. \n" +
                            $"Disponible: {producto.Stock}",
                            "Stock insuficiente",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);

                        return;
                    }

                    producto.Stock -= item.Cantidad;

                    db.DetalleVentas.Add(new DetalleVenta
                    {
                        VentaId = venta.Id,
                        ProductoId = producto.Id,
                        Cantidad = item.Cantidad,
                        Precio = item.Precio,
                        Importe = item.Importe
                    });

                    if (producto.Stock <= producto.StockMinimo)
                    {
                        alertasStock.Add(
                            $"• {producto.Nombre}\n" +
                            $"   Stock actual: {producto.Stock}\n" +
                            $"   Stock mínimo: {producto.StockMinimo}");
                    }
                }

                if (cobro.GenerarDeuda)
                {
                    var deuda = new Deuda
                    {
                        Cliente = cobro.ClienteDeuda,

                        Concepto = "Compra fiada",

                        Fecha = DateTime.Now,

                        MontoOriginal = cobro.SaldoPendiente,

                        SaldoPendiente = cobro.SaldoPendiente,

                        Pagada = false,

                        VentaId = venta.Id
                    };

                    db.Deudas.Add(deuda);
                }

                db.SaveChanges();

                transaccion.Commit();

                string mensaje = "Venta realizada correctamente.";

                if (cobro.Descuento > 0)
                {
                    mensaje +=
                        $"\n\nSubtotal: {cobro.Subtotal:C}" +
                        $"\nDescuento ({cobro.PorcentajeDescuento:0.##}%): -{cobro.Descuento:C}" +
                        $"\nTotal: {cobro.TotalFinal:C}";
                }

                if (cobro.GenerarDeuda)
                {
                    mensaje +=
                        $"\n\n💳 Venta con deuda" +
                        $"\nCliente: {cobro.ClienteDeuda}" +
                        $"\nPagó: {cobro.Recibido:C}" +
                        $"\nDebe: {cobro.SaldoPendiente:C}";
                }

                if (alertasStock.Any())
                {
                    mensaje +=
                        "\n\n⚠ Inventario bajo:\n\n" +
                        string.Join("\n\n", alertasStock);
                }

                MessageBox.Show(
                    mensaje,
                    cobro.GenerarDeuda
                        ? "Venta registrada con deuda"
                        : "Venta realizada",
                    MessageBoxButton.OK,
                    alertasStock.Any()
                        ? MessageBoxImage.Warning
                        : MessageBoxImage.Information);

                carrito.Clear();

                RefrescarCarrito();

                LimpiarBusqueda();

                productoSeleccionado = null;

                CargarProductos();
            }
            catch (Exception ex)
            {
                transaccion.Rollback();

                string error = ex.ToString();

                if (ex.InnerException != null)
                    error += "\n\nINNER: \n" + ex.InnerException;

                MessageBox.Show(
                    error,
                    "Error al guardar venta",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void BtnAumentar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || btn.Tag is not VentaItem item)
                return;

            var producto = productos.FirstOrDefault(p => p.Id == item.ProductoId);

            if (producto == null)
                return;

            if (item.Cantidad >= producto.Stock)
            {
                MessageBox.Show(
                    $"Solo hay {producto.Stock} piezas disponibles.",
                    "Stock insuficiente",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            ActualizarCantidad(item, item.Cantidad + 1);
        }

        private void BtnDisminuir_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || btn.Tag is not VentaItem item)
                return;

            ActualizarCantidad(item, item.Cantidad - 1);
        }

        private void btnCobrar_Click(object sender, RoutedEventArgs e)
        {
            if (carrito.Count == 0)
            {
                MessageBox.Show("No hay productos.");
                return;
            }

            decimal total = carrito.Sum(x => x.Importe);

            var ventana = new CobroWindow(total)
            {
                Owner = Application.Current.MainWindow
            };

            if (ventana.ShowDialog() != true || !ventana.VentaConfirmada)
            {
                return;
            }

            GuardarVenta(ventana);
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            if (carrito.Count == 0)
            {
                MessageBox.Show(
                    "No hay ninguna venta en proceso.",
                    "Cancelar Venta",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                return;
            }

            var resultado = MessageBox.Show(
                "¿Está seguro de cancelar la venta?\n\nSe eliminarán todos los productos del carrito.",
                "Cancelar venta",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (resultado != MessageBoxResult.Yes)
                return;

            carrito.Clear();

            productoSeleccionado = null;

            RefrescarCarrito();

            LimpiarBusqueda();
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (productoSeleccionado == null)
            {
                MessageBox.Show(
                    "Seleccione un producto del carrito.",
                    "Eliminar producto",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                return;
            }

            var resultado = MessageBox.Show(
                $"¿Desea eliminar '{productoSeleccionado.Nombre} de la venta?",
                "Confirmar",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (resultado != MessageBoxResult.Yes)
                return;

            carrito.Remove(productoSeleccionado);

            productoSeleccionado = null;

            RefrescarCarrito();
        }

        private void dgVenta_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            productoSeleccionado = dgVenta.SelectedItem as VentaItem;
        }

        private void VentasView_Loaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(this);
            txtBuscar.Focus();
        }

        private void VentasView_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F2:
                    btnCobrar_Click(btnCobrar, new RoutedEventArgs());
                    e.Handled = true;
                    break;

                case Key.Escape:
                    btnCancelar_Click(btnCancelar, new RoutedEventArgs());
                    e.Handled = true;
                    break;

                case Key.Delete:
                    btnEliminar_Click(btnEliminar, new RoutedEventArgs());
                    e.Handled = true;
                    break;

                case Key.B:
                    if (Keyboard.Modifiers == ModifierKeys.Control)
                    {
                        txtBuscar.Focus();
                        txtBuscar.SelectAll();
                        e.Handled = true;
                    }
                    break;
            }
        }
    }
}
