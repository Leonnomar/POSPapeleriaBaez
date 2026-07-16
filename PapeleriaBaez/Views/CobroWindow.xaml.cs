using Microsoft.EntityFrameworkCore.Metadata;
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
    /// Lógica de interacción para CobroWindow.xaml
    /// </summary>
    public partial class CobroWindow : Window
    {
        private decimal total;

        public bool VentaConfirmada { get; private set; }

        public CobroWindow(decimal totalVenta)
        {
            InitializeComponent();

            total = totalVenta;

            lblTotal.Text = total.ToString("C");
            lblCambio.Text = "$0.00";
        }

        private void txtRecibido_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!decimal.TryParse(txtRecibido.Text, out decimal recibido))
            {
                lblCambio.Text = "$0.00";
                return;
            }

            decimal cambio = recibido - total;

            lblCambio.Text = cambio.ToString("C");

            btnCobrar.IsEnabled = recibido >= total;
        }

        private void btnCobrar_Click(object sender, RoutedEventArgs e)
        {
            VentaConfirmada = true;

            DialogResult = true;

            Close();
        }
    }
}
