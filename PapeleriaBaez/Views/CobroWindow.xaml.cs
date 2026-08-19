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
        public decimal Subtotal { get; private set; }

        public decimal PorcentajeDescuento { get; private set; }

        public decimal Descuento { get; private set; }

        public decimal TotalFinal { get; private set; }

        public decimal Recibido { get; private set; }

        public decimal SaldoPendiente { get; private set; }

        public bool GenerarDeuda => SaldoPendiente > 0;

        private decimal subtotal;

        public bool VentaConfirmada { get; private set; }

        public CobroWindow(decimal totalVenta)
        {
            InitializeComponent();

            subtotal = totalVenta;

            Subtotal = totalVenta;
            TotalFinal = totalVenta;

            lblTotal.Text = TotalFinal.ToString("C");
            lblCambio.Text = "$0.00";
        }

        private void txtRecibido_TextChanged(object sender, TextChangedEventArgs e)
        {
            RecalcularPago();
        }

        private void btnCobrar_Click(object sender, RoutedEventArgs e)
        {
            VentaConfirmada = true;

            DialogResult = true;

            Close();
        }

        private void chkDescuento_Changed(object sender, RoutedEventArgs e)
        {
            if (txtPorcentajeDescuento == null)
                return;

            bool aplicar = chkDescuento.IsChecked == true;

            txtPorcentajeDescuento.IsEnabled = aplicar;

            if (!aplicar)
                txtPorcentajeDescuento.Text = "0";

            RecalcularCobro();
        }

        private void txtPorcentajeDescuento_TextChanged(object sender, TextChangedEventArgs e)
        {
            RecalcularCobro();
        }

        private void RecalcularCobro()
        {
            if (lblTotal == null || lblDescuento == null)
                return;

            decimal porcentaje = 0;

            if (chkDescuento?.IsChecked == true)
            {
                decimal.TryParse(txtPorcentajeDescuento.Text, out porcentaje);

                if (porcentaje < 0)
                    porcentaje = 0;

                if (porcentaje > 100)
                    porcentaje = 100;
            }

            PorcentajeDescuento = porcentaje;

            Descuento = Subtotal * (porcentaje / 100m);

            TotalFinal = Subtotal - Descuento;

            lblDescuento.Text = $"Descuento: {Descuento:C}";

            lblTotal.Text = TotalFinal.ToString("C");

            RecalcularPago();

        }

        private void RecalcularPago()
        {
            if (lblCambio == null || btnCobrar == null)
                return;

            if (!decimal.TryParse(txtRecibido.Text, out decimal recibido))
            {
                recibido = 0;
            }

            Recibido = recibido;

            if (recibido >= TotalFinal)
            {
                decimal cambio = recibido - TotalFinal;

                SaldoPendiente = 0;

                lblCambio.Text = $"Cambio: {cambio:C}";
            }
            else
            {
                SaldoPendiente = TotalFinal - recibido;

                lblCambio.Text = $"Pendiente: {SaldoPendiente:C}";
            }

            btnCobrar.IsEnabled = recibido >= 0;
        }
    }
}
