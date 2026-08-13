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
using Microsoft.EntityFrameworkCore;
using PapeleriaBaez.Data;
using PapeleriaBaez.Models;

namespace PapeleriaBaez.Views
{
    /// <summary>
    /// Lógica de interacción para ReporteVentasView.xaml
    /// </summary>
    public partial class ReporteVentasView : UserControl
    {
        private List<ReporteVentaGrid> listaVentas = new();

        private List<ReporteDetalleVentaGrid> listaDetalle = new();

        private List<ProductoMasVendidoGrid> listaMasVendidos = new();
        public ReporteVentasView()
        {
            InitializeComponent();

            dpDesde.SelectedDate = DateTime.Today;
            dpHasta.SelectedDate = DateTime.Today;

            CargarVentas();
            CargarProductosMasVendidos();
        }

        private void CargarVentas()
        {
            using var db = new AppDbContext();

            DateTime desde = dpDesde.SelectedDate?.Date ?? DateTime.Today;

            DateTime hasta = (dpHasta.SelectedDate?.Date ?? DateTime.Today)
                             .AddDays(1)
                             .AddTicks(-1);

            listaVentas = db.Ventas
                .Include(v => v.Detalles)
                .Where(v => v.Fecha >= desde &&
                            v.Fecha <= hasta)
                .OrderByDescending(v => v.Fecha)
                .Select(v => new ReporteVentaGrid
                {
                    Id = v.Id,
                    Fecha = v.Fecha,
                    Productos = v.Detalles.Sum(d => d.Cantidad),
                    Total = v.Total
                })
                .ToList();

            dgVentas.ItemsSource = listaVentas;

            listaDetalle.Clear();
            dgDetalle.ItemsSource = null;

            ActualizarResumen();
        }

        private void ActualizarResumen()
        {
            int ventas = listaVentas.Count;

            int productos = listaVentas.Sum(x => x.Productos);

            decimal total = listaVentas.Sum(x => x.Total);

            decimal promedioVenta =
                ventas > 0
                    ? total / ventas
                    : 0;

            lblResumen.Text =
                $"Ventas: {ventas}      " +
                $"Artículos: {productos}        " +
                $"Ingresos: {total:C}       " +
                $"Promedio: {promedioVenta:C}";

        }

        private void CargarDetalleVenta(int ventaId)
        {
            using var db = new AppDbContext();

            listaDetalle = db.DetalleVentas
                .Include(d => d.Producto)
                .Where(d => d.VentaId == ventaId)
                .Select(d => new ReporteDetalleVentaGrid
                {
                    Codigo = d.Producto.Codigo,
                    Producto = d.Producto.Nombre,
                    Cantidad = d.Cantidad,
                    Precio = d.Precio,
                    Importe = d.Importe
                })
                .ToList();

            dgDetalle.ItemsSource = listaDetalle;
        }

        private void CargarProductosMasVendidos()
        {
            using var db = new AppDbContext();

            DateTime desde =
                dpDesde.SelectedDate?.Date ?? DateTime.Today;

            DateTime hasta =
                (dpHasta.SelectedDate?.Date ?? DateTime.Today)
                .AddDays(1)
                .AddTicks(-1);

            var detalles = db.DetalleVentas
                .Include(d => d.Venta)
                .Include(d => d.Producto)
                .Where(d =>
                    d.Venta.Fecha >= desde &&
                    d.Venta.Fecha <= hasta)
                .ToList();

            listaMasVendidos = detalles
                .GroupBy(d => new
                {
                    d.ProductoId,
                    d.Producto.Codigo,
                    d.Producto.Nombre
                })
                .Select(g => new ProductoMasVendidoGrid
                {
                    Codigo = g.Key.Codigo,
                    Producto = g.Key.Nombre,
                    Cantidad = g.Sum(d => d.Cantidad),
                    Ingresos = g.Sum(d => d.Importe)
                })
                .OrderByDescending(x => x.Cantidad)
                .ThenByDescending(x => x.Ingresos)
                .ToList();

            dgMasVendidos.ItemsSource = listaMasVendidos;
        }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            if (dpDesde.SelectedDate == null ||
                dpHasta.SelectedDate == null)
            {
                MessageBox.Show(
                    "Seleccione ambas fechas.",
                    "Reporte de ventas",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (dpDesde.SelectedDate.Value.Date >
                dpHasta.SelectedDate.Value.Date)
            {
                MessageBox.Show(
                    "La fecha inicial no puede ser mayor que la fecha final.",
                    "Reporte de ventas",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            CargarVentas();
            CargarProductosMasVendidos();
        }

        private void dgVentas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgVentas.SelectedItem is not ReporteVentaGrid venta)
                return;

            CargarDetalleVenta(venta.Id);
        }
    }
}
