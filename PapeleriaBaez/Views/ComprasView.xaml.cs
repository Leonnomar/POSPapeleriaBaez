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
            try
            {
                if (detalleCompra.Count == 0)
                {
                    MessageBox.Show("No hay productos en la compra.");
                    return;
                }

                using var db = new AppDbContext();

                var compra = new Compra
                {
                    Fecha = DateTime.Now,
                    Total = detalleCompra.Sum(x => x.Importe)
                };

                db.Compras.Add(compra);
                db.SaveChanges();

                foreach (var item in detalleCompra)
                {
                    var producto = db.Productos
                        .FirstOrDefault(p => p.Id == item.ProductoId);

                    if (producto == null)
                        continue;

                    // COSTO PROMEDIO
                    decimal stockAnterior = producto.Stock;
                    decimal costoAnterior = producto.Costo;

                    decimal stockNuevo =
                        stockAnterior + item.Cantidad;

                    decimal costoPromedio =
                        ((stockAnterior * costoAnterior) +
                         (item.Cantidad * item.Costo))
                         / stockNuevo;

                    // ACTUALIZAR PRODUCTO
                    producto.Stock += item.Cantidad;
                    producto.Costo = Math.Round(costoPromedio, 2);

                    // GUARDAR DETALLE
                    db.DetalleCompras.Add(new DetalleCompra
                    {
                        CompraId = compra.Id,
                        ProductoId = producto.Id,
                        Cantidad = item.Cantidad,
                        Costo = item.Costo,
                        Importe = item.Importe
                    });
                }

                db.SaveChanges();

                MessageBox.Show("Compra guardada correctamente.");

                detalleCompra.Clear();

                dgDetalle.ItemsSource = null;

                ActualizarTotal();

                CargarProductos();
            }
            catch (Exception ex)
            {
                var error = ex.ToString();

                if (ex.InnerException != null)
                    error += "\n\nINNER:\n" + ex.InnerException;

                MessageBox.Show(error);
            }
        }
    }
}