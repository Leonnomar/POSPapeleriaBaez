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
using PapeleriaBaez.Models;

namespace PapeleriaBaez.Views
{
    /// <summary>
    /// Lógica de interacción para CategoriasView.xaml
    /// </summary>
    public partial class CategoriasView : UserControl
    {
        private int? categoriaSeleccionadaId = null;

        private List<Categoria> listaCategorias = new();
        public CategoriasView()
        {
            InitializeComponent();

            CargarCategorias();
        }

        private void CargarCategorias()
        {
            using var db = new AppDbContext();

            listaCategorias = db.Categorias
                .OrderBy(c => c.Nombre)
                .ToList();

            dgCategorias.ItemsSource = listaCategorias;
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("Capture un nombre.");
                return;
            }

            using var db = new AppDbContext();

            bool existe = db.Categorias
                .Any(c => c.Nombre == txtNombre.Text);

            if (existe)
            {
                MessageBox.Show("La categoría ya existe.");
                return;
            }

            db.Categorias.Add(new Categoria
            {
                Nombre = txtNombre.Text.Trim()
            });

            db.SaveChanges();

            CargarCategorias();

            BtnLimpiar_Click(null!, null!);
        }

        private void BtnLimpiar_Click(object? sender, RoutedEventArgs? e)
        {
            categoriaSeleccionadaId = null;

            txtNombre.Clear();

            txtNombre.Focus();
        }

        private void BtnActualizar_Click(object sender, RoutedEventArgs e)
        {
            if (categoriaSeleccionadaId == null)
                return;

            using var db = new AppDbContext();

            var categoria = db.Categorias
                .FirstOrDefault(c =>
                    c.Id == categoriaSeleccionadaId);

            if (categoria == null)
                return;

            categoria.Nombre = txtNombre.Text.Trim();

            db.SaveChanges();

            CargarCategorias();
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (categoriaSeleccionadaId == null)
                return;

            using var db = new AppDbContext();

            bool tieneProductos = db.Productos
                .Any(p =>
                    p.CategoriaId ==
                    categoriaSeleccionadaId);

            if (tieneProductos)
            {
                MessageBox.Show(
                    "No puede eliminar esta categoría porque tiene productos asociados.");

                return;
            }

            var categoria = db.Categorias
                .FirstOrDefault(c =>
                    c.Id == categoriaSeleccionadaId);

            if (categoria == null)
                return;

            db.Categorias.Remove(categoria);

            db.SaveChanges();

            CargarCategorias();

            BtnLimpiar_Click(null!, null!);
        }

        private void dgCategorias_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgCategorias.SelectedItem is not Categoria categoria)
                return;

            categoriaSeleccionadaId = categoria.Id;

            txtNombre.Text = categoria.Nombre;
        }

        private void txtBuscar_TextChanged(object sender, TextChangedEventArgs e)
        {
            string texto =
                txtBuscar.Text
                .ToLower()
                .Trim();

            dgCategorias.ItemsSource =
                listaCategorias
                .Where(c =>
                    c.Nombre
                     .ToLower()
                     .Contains(texto))
                .ToList();
        }
    }
}
