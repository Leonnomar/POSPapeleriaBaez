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
using ClosedXML.Excel;
using Microsoft.Win32;
using DocumentFormat.OpenXml.Drawing.Charts;

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

            decimal utilidadEstimada =
                CalcularUtilidadEstimada();

            lblResumen.Text =
                $"Ventas: {ventas}      " +
                $"Artículos: {productos}        " +
                $"Ingresos: {total:C}       " +
                $"Promedio: {promedioVenta:C}       " +
                $"Utilidad estimada: {utilidadEstimada:C}";

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

        private decimal CalcularUtilidadEstimada()
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

            decimal utilidad = detalles.Sum(d =>
                (d.Precio - d.Producto.Costo) * d.Cantidad);

            return utilidad;
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

        private void BtnExportarExcel_Click(object sender, RoutedEventArgs e)
        {
            if (listaVentas.Count == 0)
            {
                MessageBox.Show(
                    "No hay ventas para exportar.",
                    "Reporte de ventas",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                return;
            }

            var dialogo = new SaveFileDialog
            {
                Filter = "Archivo de Excel (*.xlsx)|*.xlsx",
                FileName = $"ReporteVentas_{DateTime.Now:yyyyMMdd_HHmm}.xlsx"
            };

            if (dialogo.ShowDialog() != true)
                return;

            try
            {
                using var libro = new XLWorkbook();

                CrearHojaResumen(libro);
                CrearHojaVentas(libro);
                CrearHojaMasVendidos(libro);
                CrearHojaDetalleVentas(libro);

                libro.SaveAs(dialogo.FileName);

                MessageBox.Show(
                    "Reporte exportado correctamente.",
                    "Reporte de ventas",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Error al exportar",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void CrearHojaResumen(XLWorkbook libro)
        {
            var hoja = libro.Worksheets.Add("Resumen");

            int ventas = listaVentas.Count;

            int articulos = listaVentas.Sum(x => x.Productos);

            decimal ingresos = listaVentas.Sum(x => x.Total);

            decimal promedio =
                ventas > 0
                    ? ingresos / ventas
                    : 0;

            decimal utilidad = CalcularUtilidadEstimada();

            hoja.Cell("A1").Value = "PAPELERÍA BÁEZ";
            hoja.Cell("A2").Value = "Reporte de Ventas";

            hoja.Cell("A4").Value = "Desde";
            hoja.Cell("B4").Value = dpDesde.SelectedDate?.Date ?? DateTime.Today;

            hoja.Cell("A5").Value = "Hasta";
            hoja.Cell("B5").Value = dpHasta.SelectedDate?.Date ?? DateTime.Today;

            hoja.Cell("A7").Value = "Ventas";
            hoja.Cell("B7").Value = ventas;

            hoja.Cell("A8").Value = "Artículos vendidos";
            hoja.Cell("B8").Value = articulos;

            hoja.Cell("A9").Value = "Ingresos";
            hoja.Cell("B9").Value = ingresos;

            hoja.Cell("A10").Value = "Promedio por venta";
            hoja.Cell("B10").Value = promedio;

            hoja.Cell("A11").Value = "Utilidad estimada";
            hoja.Cell("B11").Value = utilidad;

            hoja.Range("A1:B1").Merge();

            hoja.Cell("A1").Style.Font.Bold = true;
            hoja.Cell("A1").Style.Font.FontSize = 18;

            hoja.Cell("A2").Style.Font.Bold = true;
            hoja.Cell("A2").Style.Font.FontSize = 14;

            hoja.Range("A7:A11").Style.Font.Bold = true;

            hoja.Range("B9:B11").Style.NumberFormat.Format = "$#,##0.00";

            hoja.Range("B4:B5").Style.DateFormat.Format = "dd/MM/yyyy";

            hoja.Columns().AdjustToContents();
        }

        private void CrearHojaVentas(XLWorkbook libro)
        {
            var hoja = libro.Worksheets.Add("Ventas");

            hoja.Cell(1, 1).Value = "Folio";
            hoja.Cell(1, 2).Value = "Fecha";
            hoja.Cell(1, 3).Value = "Artículos";
            hoja.Cell(1, 4).Value = "Total";

            int fila = 2;

            foreach (var venta in listaVentas)
            {
                hoja.Cell(fila, 1).Value = venta.Id;
                hoja.Cell(fila, 2).Value = venta.Fecha;
                hoja.Cell(fila, 3).Value = venta.Productos;
                hoja.Cell(fila, 4).Value = venta.Total;

                fila++;
            }

            var encabezado = hoja.Range(1, 1, 1, 4);

            encabezado.Style.Font.Bold = true;
            encabezado.Style.Fill.BackgroundColor = XLColor.LightGray;

            hoja.Column(2).Style.DateFormat.Format = "dd/MM/yyyy HH:mm";

            hoja.Column(4).Style.NumberFormat.Format = "$#,##0.00";

            hoja.RangeUsed()?.SetAutoFilter();

            hoja.SheetView.FreezeRows(1);

            hoja.Columns().AdjustToContents();

        }

        private void CrearHojaMasVendidos(XLWorkbook libro)
        {
            var hoja = libro.Worksheets.Add("Más Vendidos");

            hoja.Cell(1, 1).Value = "Código";
            hoja.Cell(1, 2).Value = "Producto";
            hoja.Cell(1, 3).Value = "Cantidad vendida";
            hoja.Cell(1, 4).Value = "Ingresos";

            int fila = 2;

            foreach (var producto in listaMasVendidos)
            {
                hoja.Cell(fila, 1).Value = producto.Codigo;

                hoja.Cell(fila, 2).Value = producto.Producto;

                hoja.Cell(fila, 3).Value = producto.Cantidad;

                hoja.Cell(fila, 4).Value = producto.Ingresos;

                fila++;
            }

            var encabezado = hoja.Range(1, 1, 1, 4);

            encabezado.Style.Font.Bold = true;
            encabezado.Style.Fill.BackgroundColor = XLColor.LightGray;

            hoja.Column(4).Style.NumberFormat.Format = "$#,##0.00";

            hoja.RangeUsed()?.SetAutoFilter();

            hoja.SheetView.FreezeRows(1);

            hoja.Columns().AdjustToContents();
        }

        private void CrearHojaDetalleVentas(XLWorkbook libro)
        {
            using var db = new AppDbContext();

            DateTime desde = dpDesde.SelectedDate?.Date ?? DateTime.Today;

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
                .OrderByDescending(d => d.Venta.Fecha)
                .ThenBy(d => d.VentaId)
                .ToList();

            var hoja = libro.Worksheets.Add("Detalle Ventas");

            hoja.Cell(1, 1).Value = "Folio";
            hoja.Cell(1, 2).Value = "Fecha";
            hoja.Cell(1, 3).Value = "Código";
            hoja.Cell(1, 4).Value = "Producto";
            hoja.Cell(1, 5).Value = "Cantidad";
            hoja.Cell(1, 6).Value = "Precio";
            hoja.Cell(1, 7).Value = "Importe";

            int fila = 2;

            foreach (var detalle in detalles)
            {
                hoja.Cell(fila, 1).Value = detalle.VentaId;
                hoja.Cell(fila, 2).Value = detalle.Venta.Fecha;
                hoja.Cell(fila, 3).Value = detalle.Producto.Codigo;
                hoja.Cell(fila, 4).Value = detalle.Producto.Nombre;
                hoja.Cell(fila, 5).Value = detalle.Cantidad;
                hoja.Cell(fila, 6).Value = detalle.Precio;
                hoja.Cell(fila, 7).Value = detalle.Importe;

                fila++;
            }

            var encabezado = hoja.Range(1, 1, 1, 7);

            encabezado.Style.Font.Bold = true;
            encabezado.Style.Fill.BackgroundColor = XLColor.LightGray;

            hoja.Column(2).Style.DateFormat.Format = "dd/MM/yyyy HH:mm";

            hoja.Columns(6, 7).Style.NumberFormat.Format = "$#,##0.00";

            hoja.RangeUsed()?.SetAutoFilter();

            hoja.SheetView.FreezeRows(1);

            hoja.Columns().AdjustToContents();
        }

        private void dgVentas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgVentas.SelectedItem is not ReporteVentaGrid venta)
                return;

            CargarDetalleVenta(venta.Id);
        }
    }
}
