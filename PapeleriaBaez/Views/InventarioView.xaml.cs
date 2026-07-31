using Microsoft.EntityFrameworkCore;
using PapeleriaBaez.Data;
using PapeleriaBaez.Models;
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
using System.Windows.Shapes;

namespace PapeleriaBaez.Views
{
    /// <summary>
    /// Lógica de interacción para InventarioView.xaml
    /// </summary>
    public partial class InventarioView : UserControl
    {        
        public InventarioView()
        {
            InitializeComponent();

            CargarInventario();
        }

        private List<InventarioGrid> listaInventario = new();

        private void CargarInventario()
        {
            using var db = new AppDbContext();

            listaInventario = db.Productos
                .Include(p => p.Categoria)
                .OrderBy(p => p.Nombre)
                .Select(p => new InventarioGrid
                {
                    Id = p.Id,
                    Codigo = p.Codigo,
                    Nombre = p.Nombre,
                    Categoria = p.Categoria!.Nombre,
                    Costo = p.Costo,
                    Precio = p.PrecioVenta,
                    Stock = p.Stock,
                    StockMinimo = p.StockMinimo
                })
                .ToList();

            dgInventario.ItemsSource = listaInventario;

            ActualizarResumen();
        }

        private void txtBuscar_TextChanged(object sender, TextChangedEventArgs e)
        {
            string texto = txtBuscar.Text
                .Trim()
                .ToLower();

            dgInventario.ItemsSource = listaInventario
                .Where(p =>
                    p.Codigo.ToLower().Contains(texto) ||
                    p.Nombre.ToLower().Contains(texto) ||
                    p.Categoria.ToLower().Contains(texto))
                .ToList();

            ActualizarResumen();
        }

        private void ActualizarResumen()
        {
            var lista = dgInventario.ItemsSource.Cast<InventarioGrid>().ToList();

            int productos = lista.Count;

            int agotados = lista.Count(x => x.Stock <= 0);

            int stockBajo = lista.Count(x =>
                x.Stock > 0 &&
                x.Stock <= x.StockMinimo);

            decimal valorInventario =
                lista.Sum(x => x.ValorInventario);

            lblResumen.Text =
                $"Productos: {productos}        " +
                $"Stock Bajo: {stockBajo}       " +
                $"Agotados: {agotados}          " +
                $"Valor Inventario: {valorInventario:C}";
        }

        private void MostrarInventario(List<InventarioGrid> lista)
        {
            dgInventario.ItemsSource = lista;
            ActualizarResumen();
        }

        private void BtnTodos_Click(object sender, RoutedEventArgs e)
        {
            MostrarInventario(listaInventario);
        }
        
        private void BtnDisponibles_Click(object sender, RoutedEventArgs e)
        {
            MostrarInventario(
                listaInventario
                    .Where(p => p.Stock > p.StockMinimo)
                    .ToList());
        }

        private void BtnStockBajo_Click(object sender, RoutedEventArgs e)
        {
            MostrarInventario(
                listaInventario
                    .Where(p =>
                        p.Stock > 0 &&
                        p.Stock <= p.StockMinimo)
                    .ToList());
        }

        private void BtnAgotados_Click(object sender, RoutedEventArgs e)
        {
            MostrarInventario(
                listaInventario
                    .Where(p => p.Stock <= 0)
                    .ToList());
        }

        private void dgInventario_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgInventario.SelectedItem is not InventarioGrid producto)
                return;

            MessageBox.Show($"Abrir producto: {producto.Nombre}");
        }
    }
}
