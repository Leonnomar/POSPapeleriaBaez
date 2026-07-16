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

        private List<VentaItem> carrito = new();

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

        private void BuscarProducto()
        {
            string texto = txtBuscar.Text.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(texto))
                return;

            var producto = productos.FirstOrDefault(p =>
                p.Codigo.ToLower() == texto ||
                p.Nombre.ToLower().Contains(texto));

            if (producto == null)
            {
                MessageBox.Show("Producto no encontrado.");
                txtBuscar.SelectAll();
                txtBuscar.Focus();
                return;
            }

            AgregarProducto(producto);
        }

        private void AgregarProducto(Producto producto)
        {
            var existente = carrito.FirstOrDefault(x => x.ProductoId == producto.Id);

            if (existente != null)
            {
                existente.Cantidad++;

                dgVenta.ItemsSource = null;
                dgVenta.ItemsSource = carrito;

                ActualizarTotales();

                txtBuscar.Clear();
                txtBuscar.Focus();

                lstResultados.ItemsSource = null;
                panelResultados.Visibility = Visibility.Collapsed;

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

            dgVenta.ItemsSource = null;
            dgVenta.ItemsSource = carrito;

            ActualizarTotales();

            txtBuscar.Clear();
            txtBuscar.Focus();

            lstResultados.ItemsSource = null;

            panelResultados.Visibility = Visibility.Collapsed;

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
            string texto = txtBuscar.Text.Trim().ToLower();

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

            lstResultados.ItemsSource = resultados;

            panelResultados.Visibility =
                resultados.Any()
                ? Visibility.Visible
                : Visibility.Collapsed;
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
    }
}
