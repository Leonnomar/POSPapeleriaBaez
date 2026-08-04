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
using Microsoft.EntityFrameworkCore;
using PapeleriaBaez.Data;
using PapeleriaBaez.Models;

namespace PapeleriaBaez.Views
{
    /// <summary>
    /// Lógica de interacción para CapturarConjuntosWindows.xaml
    /// </summary>
    public partial class CapturarConjuntosWindows : Window
    {
        private readonly int cantidadConjuntos;

        private List<UniformeOpcion> uniformes = new();
        public CapturarConjuntosWindows(int cantidadConjuntos)
        {
            InitializeComponent();

            this.cantidadConjuntos = cantidadConjuntos;

            grpConjunto2.Visibility =
                cantidadConjuntos == 2
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            CargarUniformes();
        }

        private void CargarUniformes()
        {
            using var db = new AppDbContext();

            uniformes = db.UniformesCanje
                .OrderBy(u => u.Tipo)
                .ThenBy(u => u.Color)
                .ThenBy(u => u.Talla)
                .Select(u => new UniformeOpcion
                {
                    Id = u.Id,
                    Tipo = u.Tipo,
                    Color = u.Color,
                    Talla = u.Talla,
                    Existencia = u.Existencia
                })
                .ToList();

            cmbConjunto1Prenda1.ItemsSource = uniformes;
            cmbConjunto1Prenda2.ItemsSource = uniformes;

            cmbConjunto2Prenda1.ItemsSource = uniformes;
            cmbConjunto2Prenda2.ItemsSource = uniformes;
        }

        private bool ValidarFormulario()
        {
            if (cmbConjunto1Prenda1.SelectedItem is not UniformeOpcion ||
                cmbConjunto1Prenda2.SelectedItem is not UniformeOpcion)
            {
                MessageBox.Show(
                    "Selecciones las dos prendas del conjunto 1.",
                    "Canje de Uniformes",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return false;
            }

            if (cantidadConjuntos == 2 &&
                (cmbConjunto2Prenda1.SelectedItem is not UniformeOpcion ||
                 cmbConjunto2Prenda2.SelectedItem is not UniformeOpcion))
            {
                MessageBox.Show(
                    "Seleccione las dos prendas del conjunto 2.",
                    "Canje de uniformes",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return false;
            }

            return true;
        }

        private void BtnRegistrarCanje_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarFormulario())
                return;

            var prendasSeleccionadas =
                new List<(UniformeOpcion Prenda, int Conjunto)>
                {
                    (
                        (UniformeOpcion)cmbConjunto1Prenda1.SelectedItem,
                        1
                    ),
                    (
                        (UniformeOpcion)cmbConjunto1Prenda2.SelectedItem,
                        1
                    )
                };

            if (cantidadConjuntos == 2)
            {
                prendasSeleccionadas.Add(
                    (
                        (UniformeOpcion)cmbConjunto2Prenda1.SelectedItem,
                        2
                    ));

                prendasSeleccionadas.Add(
                    (
                        (UniformeOpcion)cmbConjunto2Prenda2.SelectedItem,
                        2
                    ));
            }

            using var db = new AppDbContext();
            using var transaccion = db.Database.BeginTransaction();

            try
            {
                var canje = new CanjeUniforme
                {
                    Fecha = DateTime.Now,
                    CantidadConjuntos = cantidadConjuntos
                };

                db.CanjeUniformes.Add(canje);
                db.SaveChanges();

                var pendientes = new List<string>();

                foreach (var seleccion in prendasSeleccionadas)
                {
                    var uniforme = db.UniformesCanje
                        .FirstOrDefault(
                            u => u.Id == seleccion.Prenda.Id);

                    if (uniforme == null)
                    {
                        throw new Exception(
                            $"No se encontró la prenda " +
                            $"{seleccion.Prenda.Descripcion}.");
                    }

                    bool pendiente = uniforme.Existencia <= 0;

                    if (!pendiente)
                    {
                        uniforme.Existencia--;
                        uniforme.Entregados++;
                    }
                    else
                    {
                        pendientes.Add(
                            $"Conjunto {seleccion.Conjunto}: " +
                            $"{uniforme.Tipo}, " +
                            $"{uniforme.Color}, talla {uniforme.Talla}");
                    }

                    db.DetalleCanjeUniformes.Add(
                        new DetalleCanjeUniforme
                        {
                            CanjeUniformeId = canje.Id,
                            UniformeCanjeId = uniforme.Id,
                            NumeroConjunto = seleccion.Conjunto,
                            Cantidad = 1,
                            Pendiente = pendiente
                        });
                }

                db.SaveChanges();
                transaccion.Commit();

                if (pendientes.Any())
                {
                    MessageBox.Show(
                        "Canje registrado. \n\n" +
                        "Se generaron vales pendientes para:\n\n" +
                        string.Join("\n", pendientes),
                        "Canje registrado con pendientes",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }
                else
                {
                    MessageBox.Show(
                        "Canje de uniformes registrado correctamente.",
                        "Canjes",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                transaccion.Rollback();

                MessageBox.Show(
                    ex.InnerException?.Message ?? ex.Message,
                    "Error al registrar el canje",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}
