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
        public ReporteVentasView()
        {
            InitializeComponent();

            dpDesde.SelectedDate = DateTime.Today;
            dpHasta.SelectedDate = DateTime.Today;

            CargarVentas();
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

            ActualizarResumen();
        }

        private void ActualizarResumen()
        {
            int ventas = listaVentas.Count;

            int productos = listaVentas.Sum(x => x.Productos);

            decimal total = listaVentas.Sum(x => x.Total);

            lblResumen.Text =
                $"Ventas: {ventas}      " +
                $"Artículos: {productos}        " +
                $"Ingresos: {total:C}";

        }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            CargarVentas();
        }

        private void dgVentas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
