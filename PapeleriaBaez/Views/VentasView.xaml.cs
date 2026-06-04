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
        private List<ProductoGrid> listaProductos = new();

        private List<CarritoItem> carrito = new();

        public VentasView()
        {
            InitializeComponent();

            CargarProductos();
        }

        private void CargarProductos()
        {
            using var db = new AppDbContext();

            listaProductos = db.Productos
                .Include(p => p.Categoria)
                .Select(p => new ProductoGrid
                {
                    Id = p.Id,
                    Codigo = p.Codigo,
                    Nombre = p.Nombre,
                    Precio = p.PrecioVenta,
                    Stock = p.Stock,
                    Categoria = p.Categoria!.Nombre
                })
                .ToList();

            dgProductos.ItemsSource = listaProductos;
        }

        private void txtBuscar_TextChanged(object sender, TextChangedEventArgs e)
        {
            string texto = txtBuscar.Text
                .ToLower()
                .Trim();

            dgProductos.ItemsSource =
                listaProductos
                .Where(p =>
                    p.Nombre.ToLower().Contains(texto) ||
                    p.Codigo.ToLower().Contains(texto))
                .ToList();
        }

        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            if (dgProductos.SelectedItem is not ProductoGrid producto)
                return;

            var existente = carrito
                .FirstOrDefault(c =>
                    c.ProductoId == producto.Id);

            if (existente != null)
            {
                existente.Cantidad++;
            }
            else
            {
                carrito.Add(new CarritoItem
                {
                    ProductoId = producto.Id,
                    Codigo = producto.Codigo,
                    Nombre = producto.Nombre,
                    Precio = producto.Precio,
                    Cantidad = 1
                });
            }

            dgCarrito.ItemsSource = null;
            dgCarrito.ItemsSource = carrito;

            ActualizarTotal();
        }

        private void ActualizarTotal()
        {
            decimal total =
                carrito.Sum(c => c.Importe);

            lblTotal.Text =
                total.ToString("C");
        }
    }
}
