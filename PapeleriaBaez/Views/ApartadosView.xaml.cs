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

        private List<Apartado> listaApartados = new();

        private int? apartadosSeleccionadoId;

        private string filtroApartado = "Pendiente";

        public ApartadosView()
        {
            InitializeComponent();

            CargarProductos();
            CargarApartados();

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

        private void CargarApartados()
        {
            using var db = new AppDbContext();

            var consulta = db.Apartados.AsQueryable();

            if (filtroApartado != "Todos")
            {
                consulta = consulta.Where(a => a.Estado == filtroApartado);
            }

            string texto = txtBuscarApartado?.Text.Trim() ?? "";

            if (!string.IsNullOrWhiteSpace(texto))
            {
                consulta = consulta.Where(a => a.Cliente.Contains(texto));
            }

            listaApartados = consulta
                .OrderByDescending(a => a.Fecha)
                .ToList();

            dgApartadosRegistrados.ItemsSource = listaApartados;
        }

        private void CargarAbonosApartado(int apartadoId)
        {
            using var db = new AppDbContext();

            var abonos = db.abonoApartados
                .Where(a => a.ApartadoId == apartadoId)
                .OrderByDescending(a => a.Fecha)
                .ToList();

            dgAbonosApartado.ItemsSource = abonos;
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

        private void LimpiarSeleccionApartado()
        {
            apartadosSeleccionadoId = null;

            dgApartadosRegistrados.SelectedItem = null;
            dgAbonosApartado.ItemsSource = null;

            txtAbonoApartado.Clear();

            lblApartadoSeleccionado.Text = "Seleccione un apartado";

            lblSaldoApartado.Text = "Saldo: $0.00";
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

        private void BtnRegistrarAbonoApartado_Click(object sender, RoutedEventArgs e)
        {
            if (apartadosSeleccionadoId == null)
            {
                MessageBox.Show("Seleccione un apartado.");
                return;
            }

            if (!decimal.TryParse(txtAbonoApartado.Text, out decimal monto) || monto <= 0)
            {
                MessageBox.Show("Capture un abono válido.");
                return;
            }

            using var db = new AppDbContext();

            var apartado = db.Apartados
                .FirstOrDefault(a =>
                    a.Id == apartadosSeleccionadoId.Value);

            if (apartado == null)
                return;

            if (apartado.Estado != "Pendiente")
            {
                MessageBox.Show("Solo se pueden registrar abonos en apartados pendientes.");

                return;
            }

            if (monto > apartado.SaldoPendiente)
            {
                MessageBox.Show(
                    $"El abonoo no puede ser mayor al saldo.\n\n" +
                    $"Saldo: {apartado.SaldoPendiente:C}");

                return;
            }

            db.abonoApartados.Add(
                new AbonoApartado
                {
                    ApartadoId = apartado.Id,
                    Fecha = DateTime.Now,
                    Monto = monto
                });

            apartado.Pagado += monto;
            apartado.SaldoPendiente -= monto;

            if (apartado.SaldoPendiente < 0)
                apartado.SaldoPendiente = 0;

            db.SaveChanges();

            txtAbonoApartado.Text = $"Saldo: {apartado.SaldoPendiente:C}";

            CargarAbonosApartado(apartado.Id);
            CargarApartados();

            MessageBox.Show(
                $"Abono registrado. \n\n" +
                $"Saldo pendiente: {apartado.SaldoPendiente:C}");
        }

        private void BtnCancelarApartado_Click(object sender, RoutedEventArgs e)
        {
            if (apartadosSeleccionadoId == null)
            {
                MessageBox.Show("Seleccione un apartado.");
                return;
            }

            var resultado = MessageBox.Show(
                "¿Desea cancelar este apartado?\n\n" +
                "Los productos regresarán al inventario.",
                "Cancelar apartado",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (resultado != MessageBoxResult.Yes)
                return;

            using var db = new AppDbContext();
            using var transaccion = db.Database.BeginTransaction();

            try
            {
                var apartado = db.Apartados
                    .Include(a => a.Detalles)
                    .FirstOrDefault(a =>
                        a.Id == apartadosSeleccionadoId.Value);

                if (apartado == null)
                    return;

                if (apartado.Estado != "Pendiente")
                {
                    MessageBox.Show("Este apartado ya no está pendiente.");

                    return;
                }

                foreach (var detalle in apartado.Detalles)
                {
                    var producto = db.Productos
                        .FirstOrDefault(p =>
                            p.Id == detalle.ProductoId);

                    if (producto != null)
                    {
                        producto.Stock += detalle.Cantidad;
                    }
                }

                apartado.Estado = "Cancelado";

                db.SaveChanges();
                transaccion.Commit();

                LimpiarSeleccionApartado();
                CargarApartados();
                CargarProductos();

                MessageBox.Show("Apartado cancelado. Los productos regresaron al inventario.");
            }
            catch (Exception ex)
            {
                transaccion.Rollback();

                MessageBox.Show(
                    ex.InnerException?.Message ?? ex.Message,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void BtnEntregarApartado_Click(object sender, RoutedEventArgs e)
        {
            if (apartadosSeleccionadoId == null)
            {
                MessageBox.Show("Seleccione un apartado.");
                return;
            }

            using var db = new AppDbContext();
            using var transaccion = db.Database.BeginTransaction();

            try
            {
                var apartado = db.Apartados
                    .Include(a => a.Detalles)
                    .FirstOrDefault(a =>
                        a.Id == apartadosSeleccionadoId.Value);

                if (apartado == null)
                    return;

                if (apartado.Estado != "Pendiente")
                {
                    MessageBox.Show("Este apartado ya fue cerrado.");

                    return;
                }

                if (apartado.SaldoPendiente > 0)
                {
                    MessageBox.Show(
                        $"El apartado todavía tiene saldo pendiente.\n\n" +
                        $"Saldo: {apartado.SaldoPendiente:C}",
                        "Apartado pendiente",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }

                var confirmar = MessageBox.Show(
                    $"¿Entregar el apartado #{apartado.Id}?",
                    "Entregar apartado",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (confirmar != MessageBoxResult.Yes)
                    return;

                var venta = new Venta
                {
                    Fecha = DateTime.Now,
                    Subtotal = apartado.Total,
                    PorcentajeDescuento = 0,
                    Descuento = 0,
                    Total = apartado.Total
                };

                db.Ventas.Add(venta);
                db.SaveChanges();

                foreach (var detalle in apartado.Detalles)
                {
                    db.DetalleVentas.Add(
                        new DetalleVenta
                        {
                            VentaId = venta.Id,
                            ProductoId = detalle.ProductoId,
                            Cantidad = detalle.Cantidad,
                            Precio = detalle.Precio,
                            Importe = detalle.Importe
                        });
                }

                apartado.Estado = "Entregado";
                apartado.FechaEntrega = DateTime.Now;

                db.SaveChanges();
                transaccion.Commit();

                LimpiarSeleccionApartado();
                CargarApartados();

                MessageBox.Show(
                    $"Apartado entregado correctamente.\n\n" +
                    $"Venta generada: #{venta.Id}");
            }
            catch (Exception ex)
            {
                transaccion.Rollback();

                MessageBox.Show(
                    ex.InnerException?.Message ?? ex.Message,
                    "Error al entregar apartado",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void BtnApartadosPendientes_Click(object sender, RoutedEventArgs e)
        {
            filtroApartado = "Pendiente";
            LimpiarSeleccionApartado();
            CargarApartados();
        }

        private void BtnApartadosEntregados_Click(object sender, RoutedEventArgs e)
        {
            filtroApartado = "Entregado";
            LimpiarSeleccionApartado();
            CargarApartados();
        }

        private void BtnApartadosCancelados_Click(object sender, RoutedEventArgs e)
        {
            filtroApartado = "Cancelado";
            LimpiarSeleccionApartado();
            CargarApartados();
        }

        private void BtnApartadosTodos_Click(object sender, RoutedEventArgs e)
        {
            filtroApartado = "Todos";
            LimpiarSeleccionApartado();
            CargarApartados();
        }

        private void txtAnticipo_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (lblTotal == null)
                return;

            ActualizarTotales();
        }

        private void txtBuscarApartado_TextChanged(object sender, TextChangedEventArgs e)
        {
            LimpiarSeleccionApartado();
            CargarApartados();
        }

        private void dgApartadosRegistrados_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgApartadosRegistrados.SelectedItem is not Apartado apartado)
            {
                LimpiarSeleccionApartado();
                return;
            }

            apartadosSeleccionadoId = apartado.Id;

            lblApartadoSeleccionado.Text = $"#{apartado.Id} - {apartado.Cliente}";

            lblSaldoApartado.Text = $"Salkdo: {apartado.SaldoPendiente:C}";

            CargarAbonosApartado(apartado.Id);
        }


    }
}
