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
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using PapeleriaBaez.Data;
using PapeleriaBaez.Models;

namespace PapeleriaBaez.Views
{
    /// <summary>
    /// Lógica de interacción para ReportesView.xaml
    /// </summary>
    public partial class ReportesView : UserControl
    {
        private List<ReporteVentaGrid> listaVentas = new();
        private List<ReporteDetalleVentaGrid> listaDetalleVentas = new();
        private List<ProductoMasVendidoGrid> listaMasVendidos = new();

        private List<ReporteCompraGrid> listaCompras = new();
        private List<ReporteDetalleCompraGrid> listaDetalleCompras = new();
        public ReportesView()
        {
            InitializeComponent();

            DateTime hoy = DateTime.Today;

            dpVentasDesde.SelectedDate = hoy;
            dpVentasHasta.SelectedDate = hoy;

            dpComprasDesde.SelectedDate = hoy;
            dpComprasHasta.SelectedDate = hoy;

            CargarVentas();
            CargarProductosMasVendidos();

            CargarCompras();
        }

        // =========================
        // VENTAS
        // =========================

        private void CargarVentas()
        {
            using var db = new AppDbContext();

            DateTime desde = dpVentasDesde.SelectedDate?.Date ?? DateTime.Today;

            DateTime hasta =
                (dpVentasHasta.SelectedDate?.Date ?? DateTime.Today)
                .AddDays(1)
                .AddTicks(-1);

            listaVentas = db.Ventas
                .Include(v => v.Detalles)
                .Where(v =>
                    v.Fecha >= desde &&
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

            listaDetalleVentas.Clear();
            dgDetalleVenta.ItemsSource = null;

            ActualizarResumenVentas();
        }

        private void CargarDetalleVenta(int ventaId)
        {
            using var db = new AppDbContext();

            listaDetalleVentas = db.DetalleVentas
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

            dgDetalleVenta.ItemsSource = listaDetalleVentas;
        }

        private void CargarProductosMasVendidos()
        {
            using var db = new AppDbContext();

            DateTime desde = dpVentasDesde.SelectedDate?.Date ?? DateTime.Today;

            DateTime hasta =
                (dpVentasHasta.SelectedDate?.Date ?? DateTime.Today)
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

            DateTime desde = dpVentasDesde.SelectedDate?.Date ?? DateTime.Today;

            DateTime hasta =
                (dpVentasHasta.SelectedDate?.Date ?? DateTime.Today)
                .AddDays(1)
                .AddTicks(-1);

            var detalles = db.DetalleVentas
                .Include(d => d.Venta)
                .Include(d => d.Producto)
                .Where(d =>
                    d.Venta.Fecha >= desde &&
                    d.Venta.Fecha <= hasta)
                .ToList();

            return detalles.Sum(d =>
                (d.Precio - d.Producto.Costo) *
                d.Cantidad);
        }

        private void ActualizarResumenVentas()
        {
            int ventas = listaVentas.Count;
            int articulos = listaVentas.Sum(x => x.Productos);
            decimal ingresos = listaVentas.Sum(x => x.Total);

            decimal promedio =
                ventas > 0
                    ? ingresos / ventas
                    : 0;

            decimal utilidad = CalcularUtilidadEstimada();

            lblResumenVentas.Text =
                $"Ventas: {ventas}      " +
                $"Artículos: {articulos}        " +
                $"Ingresos: {ingresos:C}        " +
                $"Promedio: {promedio:C}        " +
                $"Utilidad estimada: {utilidad:C}";
        }

        private void dgVentas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgVentas.SelectedItem is not ReporteVentaGrid venta)
                return;

            CargarDetalleVenta(venta.Id);
        }

        // =========================
        // COMPRAS
        // =========================

        private void CargarCompras()
        {
            using var db = new AppDbContext();

            DateTime desde = dpComprasDesde.SelectedDate?.Date ?? DateTime.Today;

            DateTime hasta =
                (dpComprasHasta.SelectedDate?.Date ?? DateTime.Today)
                .AddDays(1)
                .AddTicks(-1);

            listaCompras = db.Compras
                .Include(c => c.Detalles)
                .Where(c =>
                    c.Fecha >= desde &&
                    c.Fecha <= hasta)
                .OrderByDescending(c => c.Fecha)
                .Select(c => new ReporteCompraGrid
                {
                    Id = c.Id,
                    Fecha = c.Fecha,
                    Productos = c.Detalles.Sum(d => d.Cantidad),
                    Total = c.Total
                })
                .ToList();

            dgCompras.ItemsSource = listaCompras;

            listaDetalleCompras.Clear();
            dgDetalleCompra.ItemsSource = null;

            ActualizarResumenCompras();
        }

        private void CargarDetalleCompra(int compraId)
        {
            using var db = new AppDbContext();

            listaDetalleCompras = db.DetalleCompras
                .Include(d => d.Producto)
                .Where(d => d.CompraId == compraId)
                .Select(d => new ReporteDetalleCompraGrid
                {
                    Codigo = d.Producto.Codigo,
                    Producto = d.Producto.Nombre,
                    Cantidad = d.Cantidad,
                    Costo = d.Costo,
                    Importe = d.Importe
                })
                .ToList();

            dgDetalleCompra.ItemsSource = listaDetalleCompras;
        }

        private void ActualizarResumenCompras()
        {
            int compras = listaCompras.Count;
            int articulos = listaCompras.Sum(x => x.Productos);
            decimal invertido = listaCompras.Sum(x => x.Total);

            decimal promedio =
                compras > 0
                    ? invertido / compras
                    : 0;

            lblResumenCompras.Text =
                $"Compras: {compras}        " +
                $"Artículos: {articulos}    " +
                $"Invertido: {invertido:C}  " +
                $"Promedio: {promedio:C}";
        }

        private void dgCompras_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgCompras.SelectedItem is not ReporteCompraGrid compra)
                return;

            CargarDetalleCompra(compra.Id);
        }

        // =========================
        // FILTROS
        // =========================

        private void AplicarFiltroVentas(DateTime desde, DateTime hasta)
        {
            dpVentasDesde.SelectedDate = desde.Date;
            dpVentasHasta.SelectedDate = hasta.Date;

            CargarVentas();
            CargarProductosMasVendidos();
        }

        private void AplicarFiltroCompras(DateTime desde, DateTime hasta)
        {
            dpComprasDesde.SelectedDate = desde.Date;
            dpComprasHasta.SelectedDate = hasta.Date;

            CargarCompras();
        }

        private void BtnVentasHoy_Click(object sender, RoutedEventArgs e)
        {
            AplicarFiltroVentas(DateTime.Today, DateTime.Today);
        }

        private void BtnVentasSemana_Click(object sender, RoutedEventArgs e)
        {
            DateTime hoy = DateTime.Today;

            int diferencia = (7 + (hoy.DayOfWeek - DayOfWeek.Monday)) % 7;

            AplicarFiltroVentas(hoy.AddDays(-diferencia), hoy);
        }

        private void BtnVentasMes_Click(object sender, RoutedEventArgs e)
        {
            DateTime hoy = DateTime.Today;

            AplicarFiltroVentas(new DateTime(hoy.Year, hoy.Month, 1), hoy);
        }

        private void BtnComprasHoy_Click(object sender, RoutedEventArgs e)
        {

            AplicarFiltroCompras(DateTime.Today, DateTime.Today);
        }

        private void BtnComprasSemana_Click(object sender, RoutedEventArgs e)
        {
            DateTime hoy = DateTime.Today;

            int diferencia = (7 + (hoy.DayOfWeek - DayOfWeek.Monday)) % 7;

            AplicarFiltroCompras(hoy.AddDays(-diferencia), hoy);
        }

        private void BtnComprasMes_Click(object sender, RoutedEventArgs e)
        {
            DateTime hoy = DateTime.Today;

            AplicarFiltroCompras(new DateTime(hoy.Year, hoy.Month, 1), hoy);
        }

        private void BtnBuscarVentas_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarFechas(dpVentasDesde, dpVentasHasta))
                return;

            CargarVentas();
            CargarProductosMasVendidos();
        }

        private void BtnBuscarCompras_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarFechas(dpComprasDesde, dpComprasHasta))
                return;

            CargarCompras();
        }

        private bool ValidarFechas(DatePicker desde, DatePicker hasta)
        {
            if (desde.SelectedDate == null ||
                hasta.SelectedDate == null)
            {
                MessageBox.Show("Seleccione ambas fechas.");

                return false;
            }

            if (desde.SelectedDate.Value.Date > hasta.SelectedDate.Value.Date)
            {
                MessageBox.Show("La fecha inicial no puede ser mayor que la final.");

                return false;
            }

            return true;
        }

        private void BtnExportarVentas_Click(object sender, RoutedEventArgs e)
        {
            if (listaVentas.Count == 0)
            {
                MessageBox.Show(
                    "No hay ventas para exportar.",
                    "Reportes",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                return;
            }

            var dialogo = new SaveFileDialog
            {
                Filter = "Archivo de Excel (*.xlsx)|*.xlsx",
                FileName = $"ReporteVenta_{DateTime.Now:yyyyMMdd_HHmm}.xlsx"
            };

            if (dialogo.ShowDialog() != true)
                return;

            try
            {
                using var libro = new XLWorkbook();

                CrearHojaResumenVentas(libro);
                CrearHojaVentas(libro);
                CrearHojaDetalleVentas(libro);
                CrearHojaMasVendidos(libro);

                libro.SaveAs(dialogo.FileName);

                MessageBox.Show(
                    "Reporte de ventas exportado correctamente.",
                    "Reportes",
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

        private void BtnExportarCompras_Click(object sender, RoutedEventArgs e)
        {
            if (listaCompras.Count == 0)
            {
                MessageBox.Show(
                    "No hay compras para exportar.",
                    "Reportes",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                return;
            }

            var dialogo = new SaveFileDialog
            {
                Filter = "Archivo de Excel (*.xlsx)|*.xlsx",
                FileName = $"ReporteCompras_{DateTime.Now:yyyyMMdd_HHmm}.xlsx"
            };

            if (dialogo.ShowDialog() != true)
                return;

            try
            {
                using var libro = new XLWorkbook();

                CrearHojaResumenCompras(libro);
                CrearHojaCompras(libro);
                CrearHojaDetalleCompras(libro);

                libro.SaveAs(dialogo.FileName);

                MessageBox.Show(
                    "Reporte de compras exportado correctamente.",
                    "Reportes",
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

        private void CrearHojaResumenVentas(XLWorkbook libro)
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
            hoja.Cell("B4").Value = dpVentasDesde.SelectedDate?.Date ?? DateTime.Today;

            hoja.Cell("A5").Value = "Hasta";
            hoja.Cell("B5").Value = dpVentasHasta.SelectedDate?.Date ?? DateTime.Today;

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

        private void CrearHojaDetalleVentas(XLWorkbook libro)
        {
            using var db = new AppDbContext();

            DateTime desde = dpVentasDesde.SelectedDate?.Date ?? DateTime.Today;

            DateTime hasta =
                (dpVentasHasta.SelectedDate?.Date ?? DateTime.Today)
                .AddDays(1)
                .AddTicks(-1);

            var detalles = db.DetalleVentas
                .Include(d => d.Venta)
                .Include(d => d.Producto)
                .Where(d =>
                    d.Venta.Fecha >= desde &&
                    d.Venta.Fecha <= hasta)
                .OrderByDescending(d => d.Venta.Fecha)
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

            hoja.Range(1, 1, 1, 7).Style.Font.Bold = true;

            hoja.Column(2).Style.DateFormat.Format = "dd/MM/yyyy HH:mm";

            hoja.Columns(6, 7).Style.NumberFormat.Format = "$#,##0.00";

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

            hoja.Range(1, 1, 1, 4).Style.Font.Bold = true;

            hoja.Column(4).Style.NumberFormat.Format = "$#,##0.00";

            hoja.RangeUsed()?.SetAutoFilter();
            hoja.SheetView.FreezeRows(1);
            hoja.Columns().AdjustToContents();
        }

        private void CrearHojaResumenCompras(XLWorkbook libro)
        {
            var hoja = libro.Worksheets.Add("Resumen");

            int compras = listaCompras.Count();
            int articulos = listaCompras.Sum(x => x.Productos);
            decimal invertido = listaCompras.Sum(x => x.Total);

            decimal promedio =
                compras > 0
                    ? invertido / compras
                    : 0;

            hoja.Cell("A1").Value = "PAPELERÍA BÁEZ";
            hoja.Cell("A2").Value = "Reporte de Compras";

            hoja.Cell("A4").Value = "Desde";
            hoja.Cell("B4").Value = dpComprasDesde.SelectedDate?.Date ?? DateTime.Today;

            hoja.Cell("A5").Value = "Hasta";
            hoja.Cell("B5").Value = dpComprasHasta.SelectedDate?.Date ?? DateTime.Today;

            hoja.Cell("A7").Value = "Compras";
            hoja.Cell("B7").Value = compras;

            hoja.Cell("A8").Value = "Artículos comprados";
            hoja.Cell("B8").Value = articulos;

            hoja.Cell("A9").Value = "Total invertido";
            hoja.Cell("B9").Value = invertido;

            hoja.Cell("A10").Value = "Promedio por compra";
            hoja.Cell("B10").Value = promedio;

            hoja.Range("A1:B1").Merge();

            hoja.Cell("A1").Style.Font.Bold = true;
            hoja.Cell("A1").Style.Font.FontSize = 18;

            hoja.Range("A7:A10").Style.Font.Bold = true;

            hoja.Range("B9:B10").Style.NumberFormat.Format = "$#,##0.00";

            hoja.Range("B4:B5").Style.DateFormat.Format = "dd/MM/yyyy";

            hoja.Columns().AdjustToContents();
        }

        private void CrearHojaCompras(XLWorkbook libro)
        {
            var hoja = libro.Worksheets.Add("Compras");

            hoja.Cell(1, 1).Value = "Folio";
            hoja.Cell(1, 2).Value = "Fecha";
            hoja.Cell(1, 3).Value = "Artículos";
            hoja.Cell(1, 4).Value = "Total";

            int fila = 2;

            foreach (var compra in listaCompras)
            {
                hoja.Cell(fila, 1).Value = compra.Id;
                hoja.Cell(fila, 2).Value = compra.Fecha;
                hoja.Cell(fila, 3).Value = compra.Productos;
                hoja.Cell(fila, 4).Value = compra.Total;

                fila++;
            }

            hoja.Range(1, 1, 1, 4).Style.Font.Bold = true;

            hoja.Column(2).Style.DateFormat.Format = "dd/MM/yyyy HH:mm";

            hoja.Column(4).Style.NumberFormat.Format = "$#,##0.00";

            hoja.RangeUsed()?.SetAutoFilter();
            hoja.SheetView.FreezeRows(1);
            hoja.Columns().AdjustToContents();
        }

        private void CrearHojaDetalleCompras(XLWorkbook libro)
        {
            using var db = new AppDbContext();

            DateTime desde = dpComprasDesde.SelectedDate?.Date ?? DateTime.Today;

            DateTime hasta =
                (dpComprasHasta.SelectedDate?.Date ?? DateTime.Today)
                .AddDays(1)
                .AddTicks(-1);

            var detalles = db.DetalleCompras
                .Include(d => d.Compra)
                .Include(d => d.Producto)
                .Where(d =>
                    d.Compra.Fecha >= desde &&
                    d.Compra.Fecha <= hasta)
                .OrderByDescending(d => d.Compra.Fecha)
                .ToList();

            var hoja = libro.Worksheets.Add("Detalle Compras");

            hoja.Cell(1, 1).Value = "Folio";
            hoja.Cell(1, 2).Value = "Fecha";
            hoja.Cell(1, 3).Value = "Código";
            hoja.Cell(1, 4).Value = "Producto";
            hoja.Cell(1, 5).Value = "Cantidad";
            hoja.Cell(1, 6).Value = "Costo";
            hoja.Cell(1, 7).Value = "Importe";

            int fila = 2;

            foreach (var detalle in detalles)
            {
                hoja.Cell(fila, 1).Value = detalle.CompraId;
                hoja.Cell(fila, 2).Value = detalle.Compra.Fecha;
                hoja.Cell(fila, 3).Value = detalle.Producto.Codigo;
                hoja.Cell(fila, 4).Value = detalle.Producto.Nombre;
                hoja.Cell(fila, 5).Value = detalle.Cantidad;
                hoja.Cell(fila, 6).Value = detalle.Costo;
                hoja.Cell(fila, 7).Value = detalle.Importe;

                fila++;

                hoja.Range(1, 1, 1, 7).Style.Font.Bold = true;

                hoja.Column(2).Style.DateFormat.Format = "dd/MM/yyyy HH:mm";

                hoja.Columns(6, 7).Style.NumberFormat.Format = "$#,##0.00";

                hoja.RangeUsed()?.SetAutoFilter();
                hoja.SheetView.FreezeRows(1);
                hoja.Columns().AdjustToContents();
            }
        }
    }
}
