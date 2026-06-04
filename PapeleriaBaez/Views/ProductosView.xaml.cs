using PapeleriaBaez.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using PapeleriaBaez.Models;
using Microsoft.EntityFrameworkCore;

namespace PapeleriaBaez.Views
{
    /// <summary>
    /// Lógica de interacción para ProductosView.xaml
    /// </summary>
    public partial class ProductosView : UserControl
    {
        private int? productoSeleccionadoId = null;

        private List<ProductoGrid> listaProductos = new();
        public ProductosView()
        {
            InitializeComponent();

            using (var db = new AppDbContext())
            {
                DbInitializer.Seed(db);
            }

            CargarCategorias();
            CargarProductos();
        }

        private void CargarCategorias()
        {
            using var db = new AppDbContext();

            cmbCategorias.ItemsSource =
                db.Categorias
                    .OrderBy(c => c.Nombre)
                    .ToList();

            cmbCategorias.DisplayMemberPath = "Nombre";
            cmbCategorias.SelectedValuePath = "Id";
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
                    StockMinimo = p.StockMinimo,
                    Categoria = p.Categoria!.Nombre
                })
                .ToList();

            dgProductos.ItemsSource = listaProductos;
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarFormulario())
                return;
            try
            {
                using var db = new AppDbContext();

                var categoria = (Models.Categoria)cmbCategorias.SelectedItem;

                var producto = new Producto
                {
                    Codigo = txtCodigo.Text,
                    Nombre = txtNombre.Text,
                    Costo = decimal.Parse(txtCosto.Text),
                    PrecioVenta = decimal.Parse(txtPrecio.Text),
                    Stock = int.Parse(txtStock.Text),
                    StockMinimo = int.Parse(txtStockMinimo.Text),
                    CategoriaId = categoria.Id
                };

                bool existeCodigo = db.Productos
                    .Any(p => p.Codigo == txtCodigo.Text);

                if (existeCodigo)
                {
                    MessageBox.Show(
                        "Ya existe un producto con ese código.",
                        "Papelería Báez",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }

                db.Productos.Add(producto);

                db.SaveChanges();

                CargarProductos();

                MessageBox.Show(
                    "Producto guardado correctamente",
                    "Papelería Báez",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BtnLimpiar_Click(object? sender, RoutedEventArgs? e)
        {
            productoSeleccionadoId = null;

            txtCodigo.Clear();
            txtNombre.Clear();
            txtCosto.Clear();
            txtPrecio.Clear();
            txtStock.Clear();
            txtStockMinimo.Clear();

            cmbCategorias.SelectedIndex = -1;

            txtCodigo.Focus();
        }

        private void BtnActualizar_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarFormulario())
                return;

            if (productoSeleccionadoId == null)
            {
                MessageBox.Show("Seleccione un producto.");
                return;
            }

            using var db = new AppDbContext();

            var producto = db.Productos
                            .FirstOrDefault(
                                p => p.Id == productoSeleccionadoId);

            if (producto == null)
                return;

            producto.Codigo = txtCodigo.Text;
            producto.Nombre = txtNombre.Text;
            producto.Costo = decimal.Parse(txtCosto.Text);
            producto.PrecioVenta = decimal.Parse(txtPrecio.Text);
            producto.Stock = int.Parse(txtStock.Text);
            producto.StockMinimo = int.Parse(txtStockMinimo.Text);

            producto.CategoriaId =
                (int)cmbCategorias.SelectedValue;

            bool codigoDuplicado = db.Productos.Any(p =>
                p.Codigo == txtCodigo.Text &&
                p.Id != productoSeleccionadoId);

            if (codigoDuplicado)
            {
                MessageBox.Show(
                    "Ya existe otro producto con ese código.",
                    "Papelería Báez",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }
            
            db.SaveChanges();

            CargarProductos();

            MessageBox.Show(
                "Producto actualizado correctamente");
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (productoSeleccionadoId == null)
            {
                MessageBox.Show(
                    "Seleccione un producto.",
                    "Papelería Báez",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            using var db = new AppDbContext();

            var producto = db.Productos
                             .FirstOrDefault(
                                p => p.Id == productoSeleccionadoId);

            if (producto == null)
                return;

            var resultado = MessageBox.Show(
                $"¿Desea eliminar el producto '{producto.Nombre}'?",
                "Confirmar eliminación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (resultado != MessageBoxResult.Yes)
                return;

            db.Productos.Remove(producto);

            db.SaveChanges();

            CargarProductos();

            BtnLimpiar_Click(null!, null!);

            MessageBox.Show(
                "Producto eliminado correctamente.",
                "Papelería Báez",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void dgProductos_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (dgProductos.SelectedItem is not ProductoGrid producto)
                return;

            using var db = new AppDbContext();

            var productoReal = db.Productos
                                 .FirstOrDefault(
                                    p => p.Id == producto.Id);

            txtCodigo.Text = productoReal.Codigo;
            txtNombre.Text = productoReal.Nombre;
            txtCosto.Text = productoReal.Costo.ToString();
            txtPrecio.Text = productoReal.PrecioVenta.ToString();
            txtStock.Text = productoReal.Stock.ToString();
            txtStockMinimo.Text = productoReal.StockMinimo.ToString();


            cmbCategorias.SelectedValue = productoReal.CategoriaId;
        }

        private void Entero_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");

            e.Handled = regex.IsMatch(e.Text);
        }

        private void Decimal_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            string texto =
                textBox.Text.Insert(
                    textBox.SelectionStart,
                    e.Text);

            e.Handled =
                !decimal.TryParse(texto, out _);
        }

        private void txtBuscar_TextChanged(object sender, TextChangedEventArgs e)
        {
            string texto = txtBuscar.Text
                .Trim()
                .ToLower();

            dgProductos.ItemsSource = listaProductos
                .Where(p =>
                    p.Codigo.ToLower().Contains(texto) ||
                    p.Nombre.ToLower().Contains(texto) ||
                    p.Categoria.ToLower().Contains(texto))
                .ToList();
        }

        private bool ValidarFormulario()
        {
            if (string.IsNullOrWhiteSpace(txtCodigo.Text))
            {
                MessageBox.Show("Debe capturar el código.");
                txtCodigo.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("Debe capturar el nombre.");
                txtNombre.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtCosto.Text))
            {
                MessageBox.Show("Debe capturar el costo.");
                txtCosto.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPrecio.Text))
            {
                MessageBox.Show("Debe capturar el precio.");
                txtPrecio.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtStock.Text))
            {
                MessageBox.Show("Debe capturar el stock.");
                txtStock.Focus();
                return false;
            }

            if (cmbCategorias.SelectedItem == null)
            {
                MessageBox.Show("Debe seleccionar una categoría.");
                cmbCategorias.Focus();
                return false;
            }

            decimal costo = decimal.Parse(txtCosto.Text);
            decimal precio = decimal.Parse(txtPrecio.Text);

            if (precio < costo)
            {
                var resultado = MessageBox.Show(
                    "El precio es menor al costo.\n\n¿Desea continuar?",
                    "Advertencia",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (resultado == MessageBoxResult.No)
                    return false;
            }

            return true;
        }

        private string ObtenerPrefijo(string categoria)
        {
            return categoria switch
            {
                "Papelería" => "PAP",
                "Limpieza" => "LIM",
                "Chucherías" => "CHU",
                "Helados" => "HEL",
                "Antojitos" => "ANT",
                "Uniformes" => "UNI",
                "Faldas de mezclilla" => "FAL",
                "Regalos" => "REG",
                "Servicios" => "SER",
                _ => "OTR"
            };
        }

        private string GenerarCodigoPorCategoria(string categoria)
        {
            using var db = new AppDbContext();

            string prefijo = ObtenerPrefijo(categoria);

            var ultimoCodigo = db.Productos
                .Where(p => p.Codigo.StartsWith(prefijo))
                .OrderByDescending(p => p.Codigo)
                .Select(p => p.Codigo)
                .FirstOrDefault();

            int siguienteNumero = 1;

            if (!string.IsNullOrEmpty(ultimoCodigo))
            {
                string numeroTexto = ultimoCodigo.Substring(3);

                if (int.TryParse(numeroTexto, out int numero))
                    siguienteNumero = numero + 1;
            }

            return $"{prefijo}{siguienteNumero:D4}";
        }

        private void cmbCategorias_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbCategorias.SelectedItem is not Categoria categoria)
                return;

            if (productoSeleccionadoId != null)
                return;

            txtCodigo.Text = GenerarCodigoPorCategoria(categoria.Nombre);
        }
    }
}
