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
    /// Lógica de interacción para HielerasView.xaml
    /// </summary>
    public partial class HielerasView : UserControl
    {
        private readonly ObservableCollection<HieleraItem> itemsHielera = new();

        private List<Producto> productosHelados = new();
        public HielerasView()
        {
            InitializeComponent();

            dgNuevaHielera.ItemsSource = itemsHielera;

            CargarHelados();
            CargarSalidasPendientes();
            CargarHistorialHieleras();
            CargarResumenDeudaHelados();
            CargarAbonosHelados();
        }

        private void CargarHelados()
        {
            using var db = new AppDbContext();

            productosHelados = db.Productos
                .Include(p => p.Categoria)
                .Where(p => p.Categoria != null &&
                            p.Categoria.Nombre == "Helados")
                .OrderBy(p => p.Nombre)
                .ToList();

            lstResultadosHelados.ItemsSource = productosHelados;
        }

        private void TxtBuscarHelado_TextChanged(object sender, TextChangedEventArgs e)
        {
            string texto = txtBuscarHelado.Text.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(texto))
            {
                lstResultadosHelados.ItemsSource = productosHelados;
                return;
            }

            lstResultadosHelados.ItemsSource = productosHelados
                .Where(p =>
                    p.Nombre.ToLower().Contains(texto) ||
                    p.Codigo.ToLower().Contains(texto))
                .ToList();
        }

        private void BtnLimpiarBusqueda_Click(object sender, RoutedEventArgs e)
        {
            txtBuscarHelado.Clear();
        }

        private void LstResultadosHelados_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lstResultadosHelados.SelectedItem is not Producto producto)
                return;

            var existente = itemsHielera
                .FirstOrDefault(x => x.ProductoId == producto.Id);

            if (existente != null)
            {
                if (existente.Cantidad >= existente.StockDisponible)
                {
                    MessageBox.Show(
                        $"No puedes agregar más unidades de \"{producto.Nombre}\".\n\n" +
                        $"Stock disponible: {existente.StockDisponible}",
                        "Stock insuficiente",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }

                existente.Cantidad++;
                dgNuevaHielera.Items.Refresh();
                return;
            }

            if (producto.Stock <= 0)
            {
                MessageBox.Show(
                    $"El producto \"{producto.Nombre}\" no tiene existencia.",
                    "Sin existencia",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            itemsHielera.Add(new HieleraItem
            {
                ProductoId = producto.Id,
                Codigo = producto.Codigo,
                Nombre = producto.Nombre,
                Cantidad = 1,
                StockDisponible = producto.Stock
            });
        }

        private void BtnEliminarHelado_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button boton ||
                boton.DataContext is not HieleraItem item)
                return;

            itemsHielera.Remove(item);
        }

        private void BtnLimpiarSalida_Click(object sender, RoutedEventArgs e)
        {
            if (itemsHielera.Count == 0)
                return;

            var respuesta = MessageBox.Show(
                "¿Deseas quitar todos los helados de la hielera actual?",
                "Limpiar salida",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (respuesta == MessageBoxResult.Yes)
                return;

            itemsHielera.Clear();
        }

        private void BtnRegistrarSalida_Click(object sender, RoutedEventArgs e)
        {
            if (itemsHielera.Count == 0)
            {
                MessageBox.Show(
                    "Agregar al menos un helado a la hielera.",
                    "Hielera vacía",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            using var db = new AppDbContext();
            using var transaccion = db.Database.BeginTransaction();

            try
            {
                var productosIds = itemsHielera
                    .Select(x => x.ProductoId)
                    .ToList();

                var productos = db.Productos
                    .Where(p => productosIds.Contains(p.Id))
                    .ToList();

                foreach (var item in itemsHielera)
                {
                    var producto = productos
                        .FirstOrDefault(p => p.Id == item.ProductoId);

                    if (producto == null)
                    {
                        MessageBox.Show(
                            $"No se encontró el producto \"{item.Nombre}\".",
                            "Producto no encontrado",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);

                        transaccion.Rollback();
                        return;
                    }

                    if (item.Cantidad <= 0)
                    {
                        MessageBox.Show(
                            $"La cantidad de \"{item.Nombre}\" debe ser mayor a cero.",
                            "Cantidad inválida",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);

                        transaccion.Rollback();
                        return;
                    }

                    if (producto.Stock < item.Cantidad)
                    {
                        MessageBox.Show(
                            $"No hay suficiente stock de \"{producto.Nombre}\".\n\n" +
                            $"Stock disponible: {producto.Stock}\n" +
                            $"Cantidad solicitada: {item.Cantidad}",
                            "Stock insuficiente",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);

                        transaccion.Rollback();
                        return;
                    }
                }


                var salida = new SalidaHelados
                {
                    FechaSalida = DateTime.Now,
                    Estado = "Pendiente",
                    TotalVendido = 0m,
                    DineroRecibido = 0m,
                    SaldoPendiente = 0m
                };


                foreach (var item in itemsHielera)
                {
                    var producto = productos
                        .First(p => p.Id == item.ProductoId);

                    var detalle = new DetalleSalidaHelados
                    {
                        ProductoId = producto.Id,
                        CantidadSalida = item.Cantidad,
                        CantidadRegresada = 0,
                        Precio = producto.PrecioVenta
                    };

                    salida.Detalles.Add(detalle);

                    producto.Stock -= item.Cantidad;
                }

                db.SalidasHelados.Add(salida);

                db.SaveChanges();

                transaccion.Commit();

                MessageBox.Show(
                    $"La hielera #{salida.Id} fue registrada correctamente.\n\n" +
                    $"Fecha: {salida.FechaSalida:dd/MM/yyyy HH:mm}",
                    "Salida registrada",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);


                itemsHielera.Clear();

                CargarHelados();

                txtBuscarHelado.Clear();
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
                    "Error al registrar la salida",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void BtnCapturarCierre_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button boton || boton.DataContext is not SalidaHeladosGrid hielera)
                return;

            var ventana = new CapturarCierreWindow(hielera.Id);

            bool? resultado = ventana.ShowDialog();

            if (resultado == true)
            {
                CargarSalidasPendientes();
                CargarHistorialHieleras();
                CargarResumenDeudaHelados();
            }
        }

        private void CargarSalidasPendientes()
        {
            using var db = new AppDbContext();

            var salidas = db.SalidasHelados
                .Where(s => s.Estado == "Pendiente")
                .Include(s => s.Detalles)
                .OrderByDescending(s => s.FechaSalida)
                .ToList();

            var datos = salidas.Select(s => new SalidaHeladosGrid
            {
                Id = s.Id,
                FechaSalida = s.FechaSalida,
                Productos = s.Detalles.Count,
                Piezas = s.Detalles.Sum(d => d.CantidadSalida),
                Estado = s.Estado,
                Deuda = s.SaldoPendiente
            }).ToList();

            dgSalidasPendientes.ItemsSource = datos;
        }

        private void CargarHistorialHieleras()
        {
            using var db = new AppDbContext();

            var salidas = db.SalidasHelados
                .Where(s => s.Estado == "Cerrada")
                .Include(s => s.Detalles)
                .OrderByDescending(s => s.FechaCierre)
                .ToList();

            var datos = salidas.Select(s => new SalidaHeladosGrid
            {
                Id = s.Id,
                FechaSalida = s.FechaSalida,
                Productos = s.Detalles.Count,
                Piezas = s.Detalles.Sum(d => d.CantidadSalida),
                Estado = s.Estado,
                Deuda = s.SaldoPendiente
            }).ToList();

            dgHistoralHieleras.ItemsSource = datos;
        }

        private void CargarResumenDeudaHelados()
        {
            using var db = new AppDbContext();

            decimal deudaGenerada =
                db.SalidasHelados
                    .Where(s => s.Estado == "Cerrada")
                    .Select(s => s.SaldoPendiente)
                    .ToList()
                    .Sum();

            decimal abonos =
                db.AbonosHelados
                    .Select(a => a.Cantidad)
                    .ToList()
                    .Sum();

            decimal deudaActual = deudaGenerada - abonos;

            if (deudaActual < 0)
                deudaActual = 0;

            lblDeudaGenerada.Text = deudaGenerada.ToString("C2");
            lblAbonosHelados.Text = abonos.ToString("C2");
            lblDeudaActual.Text = deudaActual.ToString("C2");
        }

        private void TxtBuscarSalida_TextChanged(object sender, TextChangedEventArgs e)
        {
            string texto = txtBuscarSalida.Text.Trim().ToLower();

            using var db = new AppDbContext();

            var salidas = db.SalidasHelados
                .Where(s => s.Estado == "Pendiente")
                .Include(s => s.Detalles)
                .OrderByDescending(s => s.FechaSalida)
                .ToList();

            if (!string.IsNullOrWhiteSpace(texto))
            {
                salidas = salidas
                    .Where(s =>
                        s.Id.ToString().Contains(texto) ||
                        s.FechaSalida.ToString("dd/MM/yyyy HH:mm").ToLower().Contains(texto))
                    .ToList();
            }

            var datos = salidas.Select(s => new SalidaHeladosGrid
            {
                Id = s.Id,
                FechaSalida = s.FechaSalida,
                Productos = s.Detalles.Count,
                Piezas = s.Detalles.Sum(d => d.CantidadSalida),
                Estado = s.Estado,
                Deuda = s.SaldoPendiente
            }).ToList();

            dgSalidasPendientes.ItemsSource = datos;
        }

        private void BtnRegistrarAbono_Click(object sender, RoutedEventArgs e)
        {
            if (!decimal.TryParse(txtAbonoHelados.Text, out decimal cantidad))
            {
                MessageBox.Show(
                    "Ingrresa una cantidad válida.",
                    "Cantidad inválida",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (cantidad <= 0)
            {
                MessageBox.Show(
                    "El abono debe ser mayor a cero.",
                    "Cantidad inválida",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            using var db = new AppDbContext();

            decimal deudaGenerada =
                db.SalidasHelados
                    .Where(s => s.Estado == "Cerrada")
                    .Select(s => s.SaldoPendiente)
                    .ToList()
                    .Sum();

            decimal abonosAnteriores =
                db.AbonosHelados
                    .Select(a => a.Cantidad)
                    .ToList()
                    .Sum();

            decimal deudaActual = deudaGenerada - abonosAnteriores;

            if (deudaActual < 0)
                deudaActual = 0;

            if (cantidad > deudaActual)
            {
                MessageBox.Show(
                    $"El abono no puede ser mayor que la deuda actual.\n\n" +
                    $"Deuda actual: {deudaActual:C2}",
                    "Abono demasiado alto",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            var abono = new AbonoHelados
            {
                Fecha = DateTime.Now,
                Cantidad = cantidad,
                Observacion = txtObservacionAbono.Text.Trim()
            };

            db.AbonosHelados.Add(abono);
            db.SaveChanges();

            MessageBox.Show(
                $"Abono registrado correctamente.\n\n" +
                $"Cantidad: {cantidad:C2}",
                "Abono registrado",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            txtAbonoHelados.Clear();
            txtObservacionAbono.Clear();

            CargarResumenDeudaHelados();
            CargarAbonosHelados();
        }

        private void CargarAbonosHelados()
        {
            using var db = new AppDbContext();

            var abonos = db.AbonosHelados
                .OrderByDescending(a => a.Fecha)
                .ToList();

            dgAbonosHelados.ItemsSource = abonos;
        }
    }
}
