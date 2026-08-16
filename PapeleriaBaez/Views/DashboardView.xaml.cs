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

namespace PapeleriaBaez.Views
{
    /// <summary>
    /// Lógica de interacción para DashboardView.xaml
    /// </summary>
    public partial class DashboardView : UserControl
    {
        public DashboardView()
        {
            InitializeComponent();

            CargarDashboard();
        }

        private void CargarDashboard()
        {
            using var db = new AppDbContext();

            DateTime desde = DateTime.Today;
            DateTime hasta = DateTime.Today
                .AddDays(1)
                .AddTicks(-1);

            var ventasHoy = db.Ventas
                .Where(v =>
                    v.Fecha >= desde &&
                    v.Fecha <= hasta)
                .ToList();

            lblVentasHoy.Text = ventasHoy.Count.ToString();

            lblIngresosHoy.Text = ventasHoy.Sum(v => v.Total).ToString("C");

            decimal deudaPendiente = db.Deudas
                .Where(d => !d.Pagada)
                .Select(d => d.SaldoPendiente)
                .ToList()
                .Sum();

            lblDeudasPendientes.Text = deudaPendiente.ToString("C");

            int stockBajo = db.Productos
                .Count(p =>
                    p.Stock > 0 &&
                    p.Stock <= p.StockMinimo);

            lblStockBajo.Text = stockBajo.ToString();

            int paquetesCanje = db.PaquetesCanje
                .Sum(p => (int?)p.Entregados) ?? 0;

            lblPaquetesCanje.Text = paquetesCanje.ToString();

            int conjuntosCanjes = db.CanjeUniformes
                .Sum(c => (int?)c.CantidadConjuntos) ?? 0;

            lblConjuntosCanje.Text = conjuntosCanjes.ToString();

            int ValesCanje = db.DetalleCanjeUniformes
                .Count(d => d.Pendiente);

            lblValesCanje.Text = ValesCanje.ToString();
        }
    }
}
