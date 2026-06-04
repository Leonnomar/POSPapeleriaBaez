using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PapeleriaBaez.Views;

namespace PapeleriaBaez
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            MainFrame.Content = new ProductosView();
        }

        private void BtnDashboard_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Modulo en construcción");
        }

        private void BtnVentas_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Modulo en construcción");
        }

        private void BtnProductos_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new ProductosView();
        }

        private void BtnCategorias_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new CategoriasView();
        }

        private void BtnInventario_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Modulo en construcción");
        }

        private void BtnCanjes_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Modulo en construcción");
        }

        private void BtnDeudas_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Modulo en construcción");
        }

        private void BtnReportes_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Modulo en construcción");
        }

        private void BtnConfiguracion_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Modulo en construcción");
        }
    }
}