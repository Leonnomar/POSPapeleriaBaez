using PapeleriaBaez.Data;
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
    /// Lógica de interacción para ProductosView.xaml
    /// </summary>
    public partial class ProductosView : UserControl
    {
        public ProductosView()
        {
            InitializeComponent();

            using (var db = new AppDbContext())
            {
                DbInitializer.Seed(db);
            }

            CargarCategorias();
        }

        private void CargarCategorias()
        {
            using var db = new AppDbContext();

            cmbCategorias.ItemsSource =
                db.Categorias
                    .OrderBy(c => c.Nombre)
                    .ToList();
        }
    }
}
