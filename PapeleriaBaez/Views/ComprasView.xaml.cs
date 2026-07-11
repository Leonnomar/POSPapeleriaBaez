using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
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
using PapeleriaBaez.Services;

namespace PapeleriaBaez.Views
{
    /// <summary>
    /// Lógica de interacción para ComprasView.xaml
    /// </summary>
    public partial class ComprasView : UserControl
    {
        private List<ProductoGrid> listaProductos = new();

        private List<CompraItem> detalleCompra = new();
        public ComprasView()
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
                    Costo = p.Costo,
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
            {
                MessageBox.Show("Seleccione un producto");
                return;
            }

            if (!int.TryParse(txtCantidad.Text, out int cantidad))
            {
                MessageBox.Show("Cantidad inválida");
                return;
            }

            if (!decimal.TryParse(txtCosto.Text, out decimal costo))
            {
                MessageBox.Show("Costo inválido");
                return;
            }

            var existente = detalleCompra
                .FirstOrDefault(x => x.ProductoId == producto.Id);

            if (existente != null)
            {
                existente.Cantidad += cantidad;
                existente.Costo += costo;

                dgDetalle.ItemsSource = null;
                dgDetalle.ItemsSource = detalleCompra;

                ActualizarTotal();
                return;
            }

            detalleCompra.Add(new CompraItem
            {
                ProductoId = producto.Id,
                Codigo = producto.Codigo,
                Nombre = producto.Nombre,
                Cantidad = cantidad,
                Costo = costo
            });

            dgDetalle.ItemsSource = null;
            dgDetalle.ItemsSource = detalleCompra;

            txtCantidad.Clear();
            txtCosto.Clear();

            ActualizarTotal();
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (dgDetalle.SelectedItem is not CompraItem item)
            {
                MessageBox.Show("Seleccione un artículo.");
                return;
            }

            detalleCompra.Remove(item);

            dgDetalle.ItemsSource = null;
            dgDetalle.ItemsSource = detalleCompra;

            txtCantidad.Clear();
            txtCosto.Clear();

            ActualizarTotal();
        }

        private void ActualizarTotal()
        {
            lblTotal.Text =
                detalleCompra
                .Sum(x => x.Importe)
                .ToString("C");
        }

        private void BtnGuardarCompra_Click(object sender, RoutedEventArgs e)
        {
            using var db = new AppDbContext();

            var service = new CompraService(db);

            bool guardado = service.GuardarCompra(
                detalleCompra,
                txtLugar.Text,
                txtObservaciones.Text);

            if (!guardado)
            {
                MessageBox.Show("No hay productos en la compra.");
                return;
            }

            MessageBox.Show("Compra guardada correctamente.");

            detalleCompra.Clear();

            dgDetalle.ItemsSource = null;

            ActualizarTotal();

            CargarProductos();

            txtLugar.Clear();

            txtObservaciones.Clear();

            txtBuscar.Clear();

            txtCantidad.Clear();

            txtCosto.Clear();
            
        }
    }
}