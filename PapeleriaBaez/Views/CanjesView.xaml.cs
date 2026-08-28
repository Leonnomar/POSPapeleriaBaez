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
using Microsoft.EntityFrameworkCore;

namespace PapeleriaBaez.Views
{
    /// <summary>
    /// Lógica de interacción para CanjesView.xaml
    /// </summary>
    public partial class CanjesView : UserControl
    {
        private List<ApartadoCanjeItem> apartadoCanjeItems = new();

        private List<ApartadoCanjeGrid> listaApartadosCanje = new();

        private string filtroApartadoCanje = "Pendiente";

        public CanjesView()
        {
            InitializeComponent();

            CargarCombos();
            CargarComboEntregaPaquetes();
            CargarPaquetes();
            CargarUniformes();
            CargarComboEntregaTenis();
            CargarTenis();
            CargarCombosApartadoCanje();
            CargarApartadosCanje();
            CargarValesPendientes();
            CargarResumenCanjes();
        }

        private void CargarCombos()
        {
            cmbPaquete.ItemsSource = new[] { 1, 2, 3, 4 };

            cmbTipoUniforme.ItemsSource = new[]
            {
                "Falda",
                "Camisa Blanca",
                "Playera Blanca",
                "Jumper",
                "Pantalón",
                "Short/Falda"
            };

            cmbTallaTenis.ItemsSource = new[]
            {
                "14", "15", "16", "17", "18", "19"
            };

            cmbColorUniforme.ItemsSource = null;
            cmbTallaUniforme.ItemsSource = null;

            cmbDevClienteTipoArticulo.ItemsSource = new[]
            {
                "Utiles",
                "Uniforme",
                "Tenis"
            };

            cmbDevClienteTipoCambio.ItemsSource = new[]
            {
                "CambioTalla",
                "MismaTalla"
            };

            cmbDevFabricaTipoDevolucion.ItemsSource = new[]
            {
                "Defectuosa",
                "Final"
            };

            cmbDevFabricaTipoArticulo.ItemsSource = new[]
            {
                "Uniforme",
                "Tenis"
            };
        }

        private void CargarComboEntregaPaquetes()
        {
            using var db = new AppDbContext();

            cmbEntregaPaquete.ItemsSource = db.PaquetesCanje
                .OrderBy(p => p.NumeroPaquete)
                .ToList();

            cmbEntregaPaquete.DisplayMemberPath = "NumeroPaquete";
        }

        private void CargarPaquetes()
        {
            using var db = new AppDbContext();

            var paquetes = db.PaquetesCanje
                .OrderBy(p => p.NumeroPaquete)
                .ToList();

            dgPaquetes.ItemsSource = paquetes;
        }

        private void CargarComboEntregaTenis()
        {
            using var db = new AppDbContext();

            cmbEntregaTenis.ItemsSource = db.TenisCanjes
                .OrderBy(t => t.Talla)
                .ToList();

            cmbEntregaTenis.DisplayMemberPath = "Talla";
        }

        private void CargarValesPendientes()
        {
            using var db = new AppDbContext();

            var valesUniformes = db.DetalleCanjeUniformes
                .Include(d => d.CanjeUniforme)
                .Include(d => d.UniformeCanje)
                .Where(d => d.Pendiente)
                .Select(d => new ValePendienteGrid
                {
                    DetalleUniformeId = d.Id,
                    ValeTenisId = null,

                    Fecha = d.CanjeUniforme.Fecha,

                    Origen = "Uniforme",

                    Referencia = $"Conjunto {d.NumeroConjunto}",

                    Tipo = d.UniformeCanje.Tipo,
                    Color = d.UniformeCanje.Color,
                    Talla = d.UniformeCanje.Talla
                })
                .ToList();

            var valesTenis = db.ValesTenisCanje
                .Include(v => v.TenisCanje)
                .Where(v => v.Pendiente)
                .Select(v => new ValePendienteGrid
                {
                    DetalleUniformeId = null,
                    ValeTenisId = v.Id,

                    Fecha = v.Fecha,

                    Origen = "Tenis",

                    Referencia = "-",

                    Tipo = "Tenis",

                    Color = "-",

                    Talla = v.TenisCanje.Talla
                })
                .ToList();

            var vales = valesUniformes
                .Concat(valesTenis)
                .OrderByDescending(v => v.Fecha)
                .ToList();

            dgVales.ItemsSource = vales;
        }

        private void BtnEntradaPaquete_Click(object sender, RoutedEventArgs e)
        {
            if (cmbPaquete.SelectedItem is not int numeroPaquete)
            {
                MessageBox.Show("Seleccione un paquete.");
                return;
            }

            if (!int.TryParse(txtCantidadPaquete.Text, out int cantidad) ||
                cantidad <= 0)
            {
                MessageBox.Show("Capture una cantidad válida");
                return;
            }

            using var db = new AppDbContext();

            var paquete = db.PaquetesCanje
                .FirstOrDefault(p => p.NumeroPaquete == numeroPaquete);

            if (paquete == null)
            {
                paquete = new PaqueteCanje
                {
                    NumeroPaquete = numeroPaquete,
                    Existencia = cantidad,
                    Entregados = 0
                };

                db.PaquetesCanje.Add(paquete);
            }
            else
            {
                paquete.Existencia += cantidad;
            }

            db.SaveChanges();

            txtCantidadPaquete.Clear();
            cmbPaquete.SelectedIndex = -1;

            CargarPaquetes();
            CargarComboEntregaPaquetes();
            CargarResumenCanjes();

            MessageBox.Show("Entrada de paquete registrada correctamente.");
        }

        private void CargarUniformes()
        {
            using var db = new AppDbContext();

            var uniformes = db.UniformesCanje
                .OrderBy(u => u.Tipo)
                .ThenBy(u => u.Color)
                .ThenBy(u => u.Talla)
                .ToList();

            dgUniformes.ItemsSource = uniformes;
        }

        private void CargarTenis()
        {
            using var db = new AppDbContext();

            dgTenis.ItemsSource = db.TenisCanjes
                .OrderBy(t => t.Talla)
                .ToList();
        }

        private void CargarCombosApartadoCanje()
        {
            using var db = new AppDbContext();

            cmbApartadoPaquete.ItemsSource = db.PaquetesCanje
                .OrderBy(p => p.NumeroPaquete)
                .ToList();

            cmbApartadoPaquete.DisplayMemberPath = "NumeroPaquete";

            var uniformes = db.UniformesCanje
                .OrderBy(u => u.Tipo)
                .ThenBy(u => u.Color)
                .ThenBy(u => u.Talla)
                .ToList();

            cmbApartadoUniformeTipo.ItemsSource = uniformes
                .Select(u => u.Tipo)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            cmbApartadoTenis.ItemsSource = db.TenisCanjes
                .OrderBy(t => t.Talla)
                .ToList();

            cmbApartadoTenis.DisplayMemberPath = "Talla";
        }

        private void CargarApartadosCanje()
        {
            using var db = new AppDbContext();

            var consulta = db.ApartadosCanje
                .Include(a => a.Detalles)
                .AsQueryable();

            if (filtroApartadoCanje != "Todos")
            {
                consulta = consulta.Where(a =>
                    a.Estado == filtroApartadoCanje);
            }

            string texto = txtBuscarApartadoCanje?.Text.Trim() ?? "";

            if (!string.IsNullOrWhiteSpace(texto))
            {
                consulta = consulta.Where(a =>
                    a.Referencia.Contains(texto));
            }

            listaApartadosCanje = consulta
                .OrderByDescending(a => a.Fecha)
                .Select(a => new ApartadoCanjeGrid
                {
                    Id = a.Id,
                    Fecha = a.Fecha,
                    Referencia = a.Referencia,
                    Estado = a.Estado,
                    CantidadArticulos = a.Detalles.Sum(d => d.Cantidad)
                })
                .ToList();

            dgApartadosCanjeRegistrados.ItemsSource = listaApartadosCanje;
        }

        private void RefrescarNuevoApartadoCanje()
        {
            dgNuevoApartadoCanje.ItemsSource = null;
            dgNuevoApartadoCanje.ItemsSource = apartadoCanjeItems;
        }

        private void BtnEntradaUniforme_Click(object sender, RoutedEventArgs e)
        {
            string tipo = cmbTipoUniforme.SelectedItem?.ToString() ?? "";
            string color = cmbColorUniforme.SelectedItem?.ToString() ?? "";
            string talla = cmbTallaUniforme.SelectedItem?.ToString() ?? "";

            if (string.IsNullOrWhiteSpace(tipo))
            {
                MessageBox.Show("Seleccione el tipo de uniforme.");
                return;
            }

            if (string.IsNullOrWhiteSpace(color))
            {
                MessageBox.Show("Seleccione el color.");
                return;
            }

            if (string.IsNullOrWhiteSpace(talla))
            {
                MessageBox.Show("Seleccione la talla.");
                return;
            }

            if (!int.TryParse(txtCantidadUniforme.Text, out int cantidad) ||
                cantidad <= 0)
            {
                MessageBox.Show("Capture una cantidad válida.");
                return;
            }

            using var db = new AppDbContext();

            var uniforme = db.UniformesCanje
                .FirstOrDefault(u =>
                    u.Tipo == tipo &&
                    u.Color == color &&
                    u.Talla == talla);

            if (uniforme == null)
            {
                uniforme = new UniformeCanje
                {
                    Tipo = tipo,
                    Color = color,
                    Talla = talla,
                    Existencia = cantidad,
                    Entregados = 0
                };

                db.UniformesCanje.Add(uniforme);
            }
            else
            {
                uniforme.Existencia += cantidad;
            }

            db.SaveChanges();

            cmbTipoUniforme.SelectedIndex = -1;
            cmbColorUniforme.SelectedIndex = -1;
            cmbTallaUniforme.SelectedIndex = -1;
            txtCantidadUniforme.Clear();

            CargarUniformes();
            CargarValesPendientes();
            CargarResumenCanjes();

            MessageBox.Show("Entrada de uniformes registrada correctamente.");
        }

        private void BtnEntradaTenis_Click(object sender, RoutedEventArgs e)
        {
            string talla = cmbTallaTenis.SelectedItem?.ToString() ?? "";

            if (string.IsNullOrWhiteSpace(talla))
            {
                MessageBox.Show("Seleccione la talla.");
                return;
            }

            if (!int.TryParse(txtCantidadTenis.Text, out int cantidad) || cantidad <= 0)
            {
                MessageBox.Show("Capture una cantidad válida.");
                return;
            }

            using var db = new AppDbContext();

            var tenis = db.TenisCanjes
                .FirstOrDefault(t => t.Talla == talla);

            if (tenis == null)
            {
                tenis = new TenisCanje
                {
                    Talla = talla,
                    Existencia = cantidad,
                    Entregados = 0
                };

                db.TenisCanjes.Add(tenis);
            }
            else
            {
                tenis.Existencia += cantidad;
            }

            db.SaveChanges();

            txtCantidadTenis.Clear();
            cmbTallaTenis.SelectedIndex = -1;

            CargarTenis();
            CargarComboEntregaTenis();
            CargarResumenCanjes();

            MessageBox.Show(
                "Entrada de tenis registrada correctamente.",
                "Tenis",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void BtnAgregarApartadoPaquete_Click(object sender, RoutedEventArgs e)
        {
            if (cmbApartadoPaquete.SelectedItem is not PaqueteCanje paquete)
            {
                MessageBox.Show("Seleccione un paquete.");
                return;
            }

            if (!int.TryParse(txtApartadoPaqueteCantidad.Text, out int cantidad) || cantidad <= 0)
            {
                MessageBox.Show("Capture una cantidad válida.");
                return;
            }

            int yaApartados = apartadoCanjeItems
                .Where(x => x.PaqueteCanjeId == paquete.Id)
                .Sum(x => x.Cantidad);

            if (yaApartados + cantidad > paquete.Existencia)
            {
                MessageBox.Show(
                    $"No hay suficiente existencia.\n\n" +
                    $"Disponibles: {paquete.Existencia}\n" +
                    $"Ya agregados al apartado: {yaApartados}");

                return;
            }

            apartadoCanjeItems.Add(
                new ApartadoCanjeItem
                {
                    Tipo = "Utiles",
                    Descripcion = $"Paquete {paquete.NumeroPaquete}",
                    Cantidad = cantidad,
                    PaqueteCanjeId = paquete.Id
                });

            RefrescarNuevoApartadoCanje();
        }

        private void BtnAgregarApartadoTenis_Click(object sender, RoutedEventArgs e)
        {
            if (cmbApartadoTenis.SelectedItem is not TenisCanje tenis)
            {
                MessageBox.Show("Seleccione una talla.");
                return;
            }

            int yaApartados = apartadoCanjeItems
                .Where(x => x.TenisCanjeId == tenis.Id)
                .Sum(x => x.Cantidad);

            if (yaApartados + 1 > tenis.Existencia)
            {
                MessageBox.Show($"No hay existencia suficiente de tenis talla {tenis.Talla}.");

                return;
            }

            apartadoCanjeItems.Add(
                new ApartadoCanjeItem
                {
                    Tipo = "Tenis",
                    Descripcion = $"Tenis talla {tenis.Talla}",
                    Cantidad = 1,
                    TenisCanjeId = tenis.Id
                });

            RefrescarNuevoApartadoCanje();
        }

        private void cmbApartadoUniformeTipo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string tipo = cmbApartadoUniformeTipo.SelectedItem?.ToString() ?? "";

            using var db = new AppDbContext();

            var colores = db.UniformesCanje
                .Where(u => u.Tipo == tipo)
                .Select(u => u.Color)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            cmbApartadoUniformeColor.ItemsSource = colores;
            cmbApartadoUniformeTalla.ItemsSource = null;

            lblApartadoUniformeExistencia.Text = "Disponibles: 0";
        }

        private void cmbApartadoUniformeColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string tipo = cmbApartadoUniformeTipo.SelectedItem?.ToString() ?? "";

            string color = cmbApartadoUniformeColor.SelectedItem?.ToString() ?? "";

            using var db = new AppDbContext();

            var tallas = db.UniformesCanje
                .Where(u =>
                    u.Tipo == tipo &&
                    u.Color == color)
                .Select(u => u.Talla)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            cmbApartadoUniformeTalla.ItemsSource = tallas;

            lblApartadoUniformeExistencia.Text = "Disponibles: 0";
        }

        private void cmbApartadoUniformeTalla_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string tipo = cmbApartadoUniformeTipo.SelectedItem?.ToString() ?? "";

            string color = cmbApartadoUniformeColor.SelectedItem?.ToString() ?? "";

            string talla = cmbApartadoUniformeTalla.SelectedItem?.ToString() ?? "";

            using var db = new AppDbContext();

            var uniforme = db.UniformesCanje
                .FirstOrDefault(u =>
                    u.Tipo == tipo &&
                    u.Color == color &&
                    u.Talla == talla);

            lblApartadoUniformeExistencia.Text =
                uniforme == null
                    ? "Disponibles: 0"
                    : $"Disponibles: {uniforme.Existencia}";
        }

        private void BtnAgregarApartadoUniforme_Click(object sender, RoutedEventArgs e)
        {
            string tipo = cmbApartadoUniformeTipo.SelectedItem?.ToString() ?? "";

            string color = cmbApartadoUniformeColor.SelectedItem?.ToString() ?? "";

            string talla = cmbApartadoUniformeTalla.SelectedItem?.ToString() ?? "";

            if (string.IsNullOrWhiteSpace(tipo) ||
                string.IsNullOrWhiteSpace(color) ||
                string.IsNullOrWhiteSpace(talla))
            {
                MessageBox.Show("Seleccione tipo, color y talla");

                return;
            }

            using var db = new AppDbContext();

            var uniforme = db.UniformesCanje
                .FirstOrDefault(u =>
                    u.Tipo == tipo &&
                    u.Color == color &&
                    u.Talla == talla);

            if (uniforme == null)
            {
                MessageBox.Show("No se encontró el uniforme.");

                return;
            }

            int yaApartados = apartadoCanjeItems
                .Where(x => x.UniformeCanjeId == uniforme.Id)
                .Sum(x => x.Cantidad);

            if (yaApartados + 1 > uniforme.Existencia)
            {
                MessageBox.Show(
                    $"No hay existencia suficiente.\n\n" +
                    $"Disponibles: {uniforme.Existencia}");

                return;
            }

            apartadoCanjeItems.Add(
                new ApartadoCanjeItem
                {
                    Tipo = "Uniforme",
                    Descripcion = $"{uniforme.Tipo} - {uniforme.Color} - {uniforme.Talla}",
                    Cantidad = 1,
                    UniformeCanjeId = uniforme.Id
                });

            RefrescarNuevoApartadoCanje();
        }

        private void cmbApartadoTenis_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbApartadoTenis.SelectedItem is TenisCanje tenis)
            {
                lblApartadoTenisExistencia.Text = $"Disponibles: {tenis.Existencia}";
            }
            else
            {
                lblApartadoTenisExistencia.Text = "Disponibles: 0";
            }
        }

        private void BtnRegistrarApartadoCanje_Click(object sender, RoutedEventArgs e)
        {
            string referencia = txtReferenciaApartadoCanje.Text.Trim();

            if (string.IsNullOrWhiteSpace(referencia))
            {
                MessageBox.Show("Capture una referencia para identificar el apartado.");

                return;
            }

            if (apartadoCanjeItems.Count == 0)
            {
                MessageBox.Show("Agregue al menos un artículo al apartado.");

                return;
            }

            using var db = new AppDbContext();
            using var transaccion = db.Database.BeginTransaction();

            try
            {
                var apartado = new ApartadoCanje
                {
                    Fecha = DateTime.Now,
                    FechaEntrega = null,
                    Referencia = referencia,
                    Estado = "Pendiente"
                };

                db.ApartadosCanje.Add(apartado);
                db.SaveChanges();

                foreach (var item in apartadoCanjeItems)
                {
                    if (item.Tipo == "Utiles" && item.PaqueteCanjeId.HasValue)
                    {
                        var paquete = db.PaquetesCanje
                            .First(p =>
                                p.Id == item.PaqueteCanjeId.Value);

                        if (paquete.Existencia < item.Cantidad)
                            throw new Exception(
                                $"No hay suficiente existencia de Paquete {paquete.NumeroPaquete}.");

                        paquete.Existencia -= item.Cantidad;
                    }

                    if (item.Tipo == "Uniforme" && item.UniformeCanjeId.HasValue)
                    {
                        var uniforme = db.UniformesCanje
                            .First(u =>
                                u.Id == item.UniformeCanjeId.Value);

                        if (uniforme.Existencia < item.Cantidad)
                            throw new Exception(
                                $"No hay suficiente existencia de {uniforme.Tipo}.");

                        uniforme.Existencia -= item.Cantidad;
                    }

                    if (item.Tipo == "Tenis" && item.TenisCanjeId.HasValue)
                    {
                        var tenis = db.TenisCanjes
                            .First(t =>
                                t.Id == item.TenisCanjeId.Value);

                        if (tenis.Existencia < item.Cantidad)
                            throw new Exception(
                                $"No hay suficiente existencia de tenis talla {tenis.Talla}.");

                        tenis.Existencia -= item.Cantidad;
                    }

                    db.DetalleApartadosCanjes.Add(
                        new DetalleApartadoCanje
                        {
                            ApartadoCanjeId = apartado.Id,
                            Tipo = item.Tipo,
                            PaqueteCanjeId = item.PaqueteCanjeId,
                            UniformeCanjeId = item.UniformeCanjeId,
                            TenisCanjeId = item.TenisCanjeId,
                            Cantidad = item.Cantidad
                        });
                }

                db.SaveChanges();
                transaccion.Commit();

                MessageBox.Show(
                    $"Apartado de canje registrado correctamente.\n\n" +
                    $"Referencia: {apartado.Referencia}",
                    "Apartados de canje",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                apartadoCanjeItems.Clear();

                txtReferenciaApartadoCanje.Clear();

                RefrescarNuevoApartadoCanje();

                CargarPaquetes();
                CargarComboEntregaPaquetes();

                CargarUniformes();

                CargarTenis();
                CargarComboEntregaTenis();

                CargarApartadosCanje();
                CargarCombosApartadoCanje();

                CargarResumenCanjes();
            }
            catch (Exception ex)
            {
                transaccion.Rollback();

                MessageBox.Show(
                    ex.InnerException?.Message ?? ex.Message,
                    "Error al registrar apartado",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void BtnApartadosCanjePendientes_Click(object sender, RoutedEventArgs e)
        {
            filtroApartadoCanje = "Pendiente";
            CargarApartadosCanje();
        }

        private void BtnApartadosCanjeEntregados_Click(object sender, RoutedEventArgs e)
        {
            filtroApartadoCanje = "Entregado";
            CargarApartadosCanje();
        }

        private void BtnApartadosCanjeCancelados_Click(object sender, RoutedEventArgs e)
        {
            filtroApartadoCanje = "Cancelado";
            CargarApartadosCanje();
        }

        private void BtnApartadosCanjeTodos_Click(object sender, RoutedEventArgs e)
        {
            filtroApartadoCanje = "Todos";
            CargarApartadosCanje();
        }

        private void txtBuscarApartadoCanje_TextChanged(object sender, TextChangedEventArgs e)
        {
            CargarApartadosCanje();
        }

        private void BtnEntregarApartadoCanje_Click(object sender, RoutedEventArgs e)
        {
            if (dgApartadosCanjeRegistrados.SelectedItem is not ApartadoCanjeGrid seleccionado)
            {
                MessageBox.Show(
                    "Seleccione un apartado.",
                    "Apartados de canje",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            using var db = new AppDbContext();
            using var transaccion = db.Database.BeginTransaction();

            try
            {
                var apartado = db.ApartadosCanje
                    .Include(a => a.Detalles)
                    .FirstOrDefault(a =>
                        a.Id == seleccionado.Id);

                if (apartado == null)
                    return;

                if (apartado.Estado != "Pendiente")
                {
                    MessageBox.Show(
                        "Este apartado ya no está pendiente.");

                    return;
                }

                var confirmar = MessageBox.Show(
                    $"¿Entregar el apartado #{apartado.Id}?\n\n" +
                    $"Referencia: {apartado.Referencia}",
                    "Entregar apartado",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);


                if (confirmar != MessageBoxResult.Yes)
                    return;

                foreach (var detalle in apartado.Detalles)
                { 
                    if (detalle.Tipo == "Utiles" &&
                        detalle.PaqueteCanjeId.HasValue)
                    {
                        var paquete = db.PaquetesCanje
                            .First(p =>
                                p.Id == detalle.PaqueteCanjeId.Value);

                        paquete.Entregados += detalle.Cantidad;
                    }

                    if (detalle.Tipo == "Uniforme" &&
                        detalle.UniformeCanjeId.HasValue)
                    {
                        var uniforme = db.UniformesCanje
                            .First(u =>
                                u.Id == detalle.UniformeCanjeId.Value);

                        uniforme.Entregados += detalle.Cantidad;
                    }

                    if (detalle.Tipo == "Tenis" &&
                        detalle.TenisCanjeId.HasValue)
                    {
                        var tenis = db.TenisCanjes
                            .First(t =>
                                t.Id == detalle.TenisCanjeId.Value);

                        tenis.Entregados += detalle.Cantidad;
                    }
                }

                apartado.Estado = "Entregado";
                apartado.FechaEntrega = DateTime.Now;

                db.SaveChanges();
                transaccion.Commit();

                CargarApartadosCanje();
                CargarPaquetes();
                CargarUniformes();
                CargarTenis();
                CargarResumenCanjes();

                MessageBox.Show(
                    "Apartado entregado correctamente.",
                    "Apartados de canje",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                transaccion.Rollback();

                MessageBox.Show(
                    ex.InnerException?.Message ?? ex.Message,
                    "Error al entregar apartado",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void BtnCancelarApartadoCanje_Click(object sender, RoutedEventArgs e)
        {
            if (dgApartadosCanjeRegistrados.SelectedItem is not ApartadoCanjeGrid seleccionado)
            {
                MessageBox.Show(
                    "Seleccione un apartado.",
                    "Apartado de canje",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            var confirmar = MessageBox.Show(
                "¿Cancelar este apartado?\n\n" +
                "Los artículos regresarán a existencia.",
                "Cancelar apartado",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirmar != MessageBoxResult.Yes)
                return;

            using var db = new AppDbContext();
            using var transaccion = db.Database.BeginTransaction();

            try
            {
                var apartado = db.ApartadosCanje
                    .Include(a => a.Detalles)
                    .FirstOrDefault(a => a.Id == seleccionado.Id);

                if (apartado == null)
                    return;

                if (apartado.Estado != "Pendiente")
                {
                    MessageBox.Show("Este apartado ya no está pendiente.");

                    return;
                }

                foreach (var detalle in apartado.Detalles)
                {
                    if (detalle.Tipo == "Utiles" &&
                        detalle.PaqueteCanjeId.HasValue)
                    {
                        var paquete = db.PaquetesCanje
                            .First(p =>
                                p.Id == detalle.PaqueteCanjeId.Value);

                        paquete.Existencia += detalle.Cantidad;
                    }

                    if (detalle.Tipo == "Uniforme" &&
                        detalle.UniformeCanjeId.HasValue)
                    {
                        var uniforme = db.UniformesCanje
                            .First(u =>
                                u.Id == detalle.UniformeCanjeId.Value);

                        uniforme.Existencia += detalle.Cantidad;
                    }

                    if (detalle.Tipo == "Tenis" &&
                        detalle.TenisCanjeId.HasValue)
                    {
                        var tenis = db.TenisCanjes
                            .First(t =>
                                t.Id == detalle.TenisCanjeId.Value);

                        tenis.Existencia += detalle.Cantidad;
                    }
                }

                apartado.Estado = "Cancelado";

                db.SaveChanges();
                transaccion.Commit();

                CargarApartadosCanje();

                CargarPaquetes();
                CargarComboEntregaPaquetes();

                CargarUniformes();

                CargarTenis();
                CargarComboEntregaTenis();

                CargarCombosApartadoCanje();
                CargarResumenCanjes();

                MessageBox.Show(
                    "Apartado cancelado. Los artículos regresaron a existencia.",
                    "Apartados de canje",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                transaccion.Rollback();

                MessageBox.Show(
                    ex.InnerException?.Message ?? ex.Message,
                    "Error al cancelar apartado",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void BtnRegistrarEntregaPaquete_Click(object sender, RoutedEventArgs e)
        {
            if (cmbEntregaPaquete.SelectedItem is not PaqueteCanje seleccionado)
            {
                MessageBox.Show("Seleccione un paquete.");
                return;
            }

            if (!int.TryParse(txtEntregaPaquete.Text, out int cantidad))
            {
                MessageBox.Show("Cantidad inválida.");
                return;
            }

            using var db = new AppDbContext();

            var paquete = db.PaquetesCanje
                .First(p => p.Id == seleccionado.Id);

            if (paquete == null)
            {
                MessageBox.Show("No se encontró el paquete");
                return;
            }

            if (paquete.Existencia < cantidad)
            {
                MessageBox.Show(
                    $"No hay suficiente paquetes disponibles.\n\n" +
                    $"Disponibles: {paquete.Existencia}\n" +
                    $"Solicitados: {cantidad}",
                    "Existencia insuficiente",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            paquete.Existencia -= cantidad;
            paquete.Entregados += cantidad;

            db.SaveChanges();

            MessageBox.Show(
                "Canje de útiles registrado correctamente.",
                "Canjes",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            txtEntregaPaquete.Text = "1";
            cmbEntregaPaquete.SelectedIndex = -1;

            CargarPaquetes();
            CargarComboEntregaPaquetes();
            CargarResumenCanjes();

            lblExistenciaPaquete.Text = "Disponibles: 0";

            ValidarEntregaPaquete();
        }

        private void BtnEntregarVale_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button boton ||
                boton.Tag is not ValePendienteGrid vale)
            {
                return;
            }

            if (vale.Origen == "Uniforme")
            {
                EntregarValeUniforme(vale);
                return;
            }

            if (vale.Origen == "Tenis")
            {
                EntregarValeTenis(vale);
                return;
            }
        }

        private void EntregarValeUniforme(ValePendienteGrid vale)
        {
            if (vale.DetalleUniformeId == null)
                return;

            using var db = new AppDbContext();
            using var transaccion = db.Database.BeginTransaction();

            try
            {
                var detalle = db.DetalleCanjeUniformes
                    .Include(d => d.UniformeCanje)
                    .FirstOrDefault(d => d.Id == vale.DetalleUniformeId.Value);

                if (detalle == null)
                    return;

                if (!detalle.Pendiente)
                {
                    MessageBox.Show("Este vale ya fue entregado.");

                    CargarValesPendientes();
                    return;
                }

                var uniforme = detalle.UniformeCanje;

                if (uniforme.Existencia <= 0)
                {
                    MessageBox.Show(
                        $"Todavía no hay existencia de:\n\n" +
                        $"{uniforme.Tipo}\n" +
                        $"Color: {uniforme.Color}\n" +
                        $"Talla: {uniforme.Talla}",
                        "Sen existencia",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }

                var respuesta = MessageBox.Show(
                    $"¿Entregar este vale?\n\n" +
                    $"{uniforme.Tipo}\n" +
                    $"Color: {uniforme.Color}\n" +
                    $"Talla: {uniforme.Talla}",
                    "Entregar vale",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (respuesta != MessageBoxResult.Yes)
                    return;

                uniforme.Existencia--;
                uniforme.Entregados++;

                detalle.Pendiente = false;

                db.SaveChanges();
                transaccion.Commit();

                CargarUniformes();
                CargarValesPendientes();
                CargarResumenCanjes();

                MessageBox.Show("Vale entregado correctamente.");
            }
            catch (Exception ex)
            { 
                transaccion.Rollback();

                MessageBox.Show(
                    ex.InnerException?.Message ??
                    ex.Message);
            }
        }

        private void EntregarValeTenis(ValePendienteGrid vale)
        {
            if (vale.ValeTenisId == null)
                return;

            using var db = new AppDbContext();
            using var transaccion = db.Database.BeginTransaction();

            try
            {
                var valeReal = db.ValesTenisCanje
                    .Include(v => v.TenisCanje)
                    .FirstOrDefault(v => v.Id == vale.ValeTenisId.Value);

                if (valeReal == null)
                    return;

                if (!valeReal.Pendiente)
                {
                    MessageBox.Show("Este vale ya fue entregado");

                    CargarValesPendientes();
                    return;
                }

                var tenis = valeReal.TenisCanje;

                if (tenis.Existencia <= 0)
                {
                    MessageBox.Show(
                        $"Todavía no hay tenis disponibles.\n\n" +
                        $"Talla: {tenis.Talla}",
                        "Sin existencia",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }

                var respuesta = MessageBox.Show(
                    $"¿Entregar este vale de tenis?\n\n" +
                    $"Talla: {tenis.Talla}",
                    "Entregar vale",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (respuesta != MessageBoxResult.Yes)
                    return;

                tenis.Existencia--;
                tenis.Entregados++;

                valeReal.Pendiente = false;
                valeReal.FechaEntrega = DateTime.Now;

                db.SaveChanges();
                transaccion.Commit();

                CargarTenis();
                CargarComboEntregaTenis();
                CargarValesPendientes();
                CargarResumenCanjes();

                MessageBox.Show(
                    "Vale de tenis entregado correctamente",
                    "Canjes",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                transaccion.Rollback();

                MessageBox.Show(
                    ex.InnerException?.Message ??
                    ex.Message,
                    "Error al entregar vale",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void BtnCapturarConjuntos_Click(object sender, RoutedEventArgs e)
        {
            int conjuntos = rbDosConjuntos.IsChecked == true ? 2 : 1;

            var ventana = new CapturarConjuntosWindows(conjuntos)
            {
                Owner = Application.Current.MainWindow
            };

            if (ventana.ShowDialog() == true)
            {
                CargarUniformes();
                CargarResumenCanjes();
                CargarValesPendientes();
            }
        }

        private void BtnRegistrarEntregaTenis_Click(object sender, RoutedEventArgs e)
        {
            if (cmbEntregaTenis.SelectedItem is not TenisCanje seleccionado)
            {
                MessageBox.Show(
                    "Seleccione una talla.",
                    "Canje de tenis",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            using var db = new AppDbContext();

            var tenis = db.TenisCanjes
                .FirstOrDefault(t => t.Id == seleccionado.Id);

            if (tenis == null)
            {
                MessageBox.Show("No se encontró la talla seleccionada.");

                return;
            }

            if (tenis.Existencia <= 0)
            {
                var respuesta = MessageBox.Show(
                    $"No hay tenis disponibles en talla {tenis.Talla}.\n\n" +
                    "¿Desea generar un vale pendiente?",
                    "Sin existencia",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (respuesta != MessageBoxResult.Yes)
                    return;

                var vale = new ValeTenisCanje
                {
                    Fecha = DateTime.Now,
                    TenisCanjeId = tenis.Id,
                    Pendiente = true,
                    FechaEntrega = null
                };

                db.ValesTenisCanje.Add(vale);
                db.SaveChanges();

                MessageBox.Show(
                    $"Vale de tenis registrado correctamente.\n\n" +
                    $"Talla: {tenis.Talla}",
                    "Vale pendiente",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                CargarValesPendientes();
                CargarResumenCanjes();

                cmbEntregaTenis.SelectedIndex = -1;

                return;
            }

            tenis.Existencia--;
            tenis.Entregados++;

            db.SaveChanges();

            MessageBox.Show(
                $"Canje de tenis registrado correctamente.\n\n" +
                $"Talla: {tenis.Talla}",
                "Canje de tenis",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            cmbEntregaTenis.SelectedIndex = -1;

            lblExistenciaTenis.Text = "Disponibles: 0";

            btnRegistrarEntregaTenis.IsEnabled = false;

            CargarTenis();
            CargarComboEntregaTenis();

            CargarResumenCanjes();
        }

        private void cmbTipoUniforme_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbTipoUniforme.SelectedItem is not string tipo)
            {
                cmbColorUniforme.ItemsSource = null;
                cmbTallaUniforme.ItemsSource = null;
                return;
            }

            CargarColoresPorTipo(tipo);
            CargarTallasPorTipo(tipo);
        }

        private void cmbEntregaTenis_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbEntregaTenis.SelectedItem is TenisCanje tenis)
            {
                lblExistenciaTenis.Text = $"Disponibles: {tenis.Existencia}";

                btnRegistrarEntregaTenis.IsEnabled = true;
            }
            else
            {
                lblExistenciaTenis.Text = "Disponibles: 0";

                btnRegistrarEntregaTenis.IsEnabled = false;
            }
        }

        private void cmbDevClienteTipoArticulo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string tipoArticulo = cmbDevClienteTipoArticulo.SelectedItem?.ToString() ?? "";

            cmbDevClienteDevueltoTipo.ItemsSource = null;
            cmbDevClienteDevueltoColor.ItemsSource = null;
            cmbDevClienteDevueltoTalla.ItemsSource = null;

            cmbDevClienteEntregadoTipo.ItemsSource = null;
            cmbDevClienteEntregadoColor.ItemsSource = null;
            cmbDevClienteEntregadoTalla.ItemsSource = null;

            lblDevClienteExistenciaNueva.Text = "Disponibles: 0";

            using var db = new AppDbContext();

            if (tipoArticulo == "Uniforme")
            {
                var tipos = db.UniformesCanje
                    .Select(u => u.Tipo)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();

                cmbDevClienteDevueltoTipo.ItemsSource = tipos;
                cmbDevClienteEntregadoTipo.ItemsSource = tipos;
            }
            else if (tipoArticulo == "Tenis")
            {
                cmbDevClienteDevueltoTipo.ItemsSource = new[] { "Tenis" };

                cmbDevClienteEntregadoTipo.ItemsSource = new[] { "Tenis" };

                cmbDevClienteDevueltoTipo.SelectedIndex = 0;
                cmbDevClienteEntregadoTipo.SelectedIndex = 0;
            }
        }

        private void cmbDevClienteDevueltoTipo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string tipoArticulo = cmbDevClienteTipoArticulo.SelectedItem?.ToString() ?? "";

            cmbDevClienteDevueltoColor.ItemsSource = null;
            cmbDevClienteDevueltoTalla.ItemsSource = null;

            using var db = new AppDbContext();

            if (tipoArticulo == "Uniforme")
            {
                string tipo = cmbDevClienteDevueltoTipo.SelectedItem?.ToString() ?? "";

                var colores = db.UniformesCanje
                    .Where(u => u.Tipo == tipo)
                    .Select(u => u.Color)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();

                cmbDevClienteDevueltoColor.ItemsSource = colores;
            }
            else if (tipoArticulo == "Tenis")
            {
                cmbDevClienteDevueltoColor.ItemsSource = new[] { "-" };

                cmbDevClienteDevueltoColor.SelectedIndex = 0;

                var tallas = db.TenisCanjes
                    .OrderBy(t => t.Talla)
                    .Select(t => t.Talla)
                    .ToList();

                cmbDevClienteDevueltoTalla.ItemsSource= tallas;
            }    
        }

        private void cmbDevClienteDevueltoColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string tipoArticulo = cmbDevClienteTipoArticulo.SelectedItem?.ToString() ?? "";

            if (tipoArticulo != "Uniforme")
                return;

            string tipo = cmbDevClienteDevueltoTipo.SelectedItem?.ToString() ?? "";

            string color = cmbDevClienteDevueltoColor.SelectedItem?.ToString() ?? "";

            using var db = new AppDbContext();

            cmbDevClienteDevueltoTalla.ItemsSource =
                db.UniformesCanje
                    .Where(u =>
                        u.Tipo == tipo &&
                        u.Color == color)
                    .Select(u => u.Talla)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();
        }

        private void cmbDevClienteEntregadoTipo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string tipoArticulo = cmbDevClienteTipoArticulo.SelectedItem?.ToString() ?? "";

            cmbDevClienteEntregadoColor.ItemsSource = null;
            cmbDevClienteEntregadoTalla.ItemsSource = null;

            using var db = new AppDbContext();

            if (tipoArticulo == "Uniforme")
            {
                string tipo = cmbDevClienteEntregadoTipo.SelectedItem?.ToString() ?? "";

                cmbDevClienteEntregadoColor.ItemsSource =
                    db.UniformesCanje
                        .Where(u => u.Tipo == tipo)
                        .Select(u => u.Color)
                        .Distinct()
                        .OrderBy(x => x)
                        .ToList();
            }
            else if (tipoArticulo == "Tenis")
            {
                cmbDevClienteEntregadoColor.ItemsSource = new[] { "-" };

                cmbDevClienteEntregadoColor.SelectedIndex = 0;

                cmbDevClienteEntregadoTalla.ItemsSource =
                    db.TenisCanjes
                        .OrderBy(t => t.Talla)
                        .Select(t => t.Talla)
                        .ToList();
            }
        }

        private void cmbDevClienteEntregadoColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string tipoArticulo = cmbDevClienteTipoArticulo.SelectedItem?.ToString() ?? "";

            if (tipoArticulo != "Uniforme")
                return;

            string tipo = cmbDevClienteEntregadoTipo.SelectedItem?.ToString() ?? "";

            string color = cmbDevClienteEntregadoColor.SelectedItem?.ToString() ?? "";

            using var db = new AppDbContext();

            cmbDevClienteEntregadoTalla.ItemsSource =
                db.UniformesCanje
                    .Where(u =>
                        u.Tipo == tipo &&
                        u.Color == color)
                    .Select(u => u.Talla)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();
        }

        private void cmbDevClienteEntregadoTalla_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string tipoArticulo = cmbDevClienteTipoArticulo.SelectedItem?.ToString() ?? "";

            using var db = new AppDbContext();

            if (tipoArticulo == "Uniforme")
            {
                string tipo = cmbDevClienteEntregadoTipo.SelectedItem?.ToString() ?? "";

                string color = cmbDevClienteEntregadoColor.SelectedItem?.ToString() ?? "";

                string talla = cmbDevClienteEntregadoTalla.SelectedItem?.ToString() ?? "";

                var uniforme = db.UniformesCanje
                    .FirstOrDefault(u =>
                        u.Tipo == tipo &&
                        u.Color == color &&
                        u.Talla == talla);

                lblDevClienteExistenciaNueva.Text = $"Disponibles: {uniforme?.Existencia ?? 0}";
            }
            else if (tipoArticulo == "Tenis")
            {
                string talla = cmbDevClienteEntregadoTalla.SelectedItem?.ToString() ?? "";

                var tenis = db.TenisCanjes
                    .FirstOrDefault(t => t.Talla == talla);

                lblDevClienteExistenciaNueva.Text = $"Disponibles: {tenis?.Existencia ?? 0}";
            }
        }

        private void BtnRegistrarDevolucionCliente_Click(object sender, RoutedEventArgs e)
        {
            string tipoArticulo = cmbDevClienteTipoArticulo.SelectedItem?.ToString() ?? "";

            string tipoCambio = cmbDevClienteTipoCambio.SelectedItem?.ToString() ?? "";

            if (string.IsNullOrWhiteSpace(tipoArticulo) ||
                string.IsNullOrWhiteSpace(tipoCambio))
            {
                MessageBox.Show("Seleccione el tipo de artículo y el tipo de cambio.");

                return;
            }

            using var db = new AppDbContext();
            using var transaccion = db.Database.BeginTransaction();

            try
            {
                if (tipoArticulo == "Uniforme")
                {
                    string devTipo = cmbDevClienteDevueltoTipo.SelectedItem?.ToString() ?? "";

                    string devColor = cmbDevClienteDevueltoColor.SelectedItem?.ToString() ?? "";

                    string devTalla = cmbDevClienteDevueltoTalla.SelectedItem?.ToString() ?? "";

                    string entTipo = cmbDevClienteEntregadoTipo.SelectedItem?.ToString() ?? "";

                    string entColor = cmbDevClienteEntregadoColor.SelectedItem?.ToString() ?? "";

                    string entTalla = cmbDevClienteEntregadoTalla.SelectedItem?.ToString() ?? "";

                    var devuelto = db.UniformesCanje
                        .FirstOrDefault(u =>
                            u.Tipo == devTipo &&
                            u.Color == devColor &&
                            u.Talla == devTalla);

                    var entregado = db.UniformesCanje
                        .FirstOrDefault(u =>
                            u.Tipo == entTipo &&
                            u.Color == entColor &&
                            u.Talla == entTalla);

                    if (devuelto == null || entregado == null)
                    {
                        MessageBox.Show("No se encontró alguno de los uniformes.");

                        return;
                    }

                    if (entregado.Existencia <= 0)
                    {
                        MessageBox.Show("No hay existencia disponible del uniforme que se quiere entregar.");

                        return;
                    }

                    devuelto.Existencia++;

                    entregado.Existencia--;

                    db.DevolucionesClienteCanje.Add(
                        new DevolucionClienteCanje
                        {
                            Fecha = DateTime.Now,
                            TipoCambio = tipoCambio,
                            TipoArticulo = "Uniforme",

                            UniformeDevueltoId = devuelto.Id,

                            UniformeEntregadoId = entregado.Id,

                            Observacion = txtDevClienteObservacion.Text.Trim()
                        });
                }
                else if (tipoArticulo == "Tenis")
                {
                    string devTalla = cmbDevClienteDevueltoTalla.SelectedItem?.ToString() ?? "";

                    string entTalla = cmbDevClienteEntregadoTalla.SelectedItem?.ToString() ?? "";

                    var devuelto = db.TenisCanjes
                        .FirstOrDefault(t =>
                            t.Talla == devTalla);

                    var entregado = db.TenisCanjes
                        .FirstOrDefault(t =>
                            t.Talla == entTalla);

                    if (devuelto == null || entregado == null)
                    {
                        MessageBox.Show("No se encontró alguno de los tenis.");

                        return;
                    }

                    if (entregado.Existencia <= 0)
                    {
                        MessageBox.Show("No hay existencia disponible de la talla que se quiere entregar.");

                        return;
                    }

                    devuelto.Existencia++;
                    entregado.Existencia--;

                    db.DevolucionesClienteCanje.Add(
                        new DevolucionClienteCanje
                        {
                            Fecha = DateTime.Now,
                            TipoCambio = tipoCambio,
                            TipoArticulo = "Tenis",

                            TenisDevueltoId = devuelto.Id,

                            TenisEntregadoId = entregado.Id,

                            Observacion = txtDevClienteObservacion.Text.Trim()
                        });
                }

                db.SaveChanges();
                transaccion.Commit();

                MessageBox.Show(
                    "Cambio registrado correctamente.",
                    "Devolución cliente",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                CargarUniformes();
                CargarTenis();
                CargarComboEntregaTenis();
                CargarResumenCanjes();
            }
            catch (Exception ex)
            {
                transaccion.Rollback();

                MessageBox.Show(
                    ex.InnerException?.Message ?? ex.Message,
                    "Error al registrar cambio",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void cmbDevFabricaTipoArticulo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string tipoArticulo = cmbDevFabricaTipoArticulo.SelectedItem?.ToString() ?? "";

            cmbDevFabricaTipo.ItemsSource = null;
            cmbDevFabricaColor.ItemsSource = null;
            cmbDevFabricaTalla.ItemsSource = null;

            using var db = new AppDbContext();

            if (tipoArticulo == "Utiles")
            {
                cmbDevFabricaTipo.ItemsSource = db.PaquetesCanje
                    .OrderBy(p => p.NumeroPaquete)
                    .Select(p => $"Paquete {p.NumeroPaquete}")
                    .ToList();

                cmbDevFabricaColor.IsEnabled = false;
                cmbDevFabricaTalla.IsEnabled = false;
            }
            else if (tipoArticulo == "Uniforme")
            {
                cmbDevFabricaColor.IsEnabled = true;
                cmbDevFabricaTalla.IsEnabled = true;

                cmbDevFabricaTipo.ItemsSource = db.UniformesCanje
                    .Select(u => u.Tipo)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();
            }
            else if (tipoArticulo == "Tenis")
            {
                cmbDevFabricaColor.IsEnabled = false;
                cmbDevFabricaTalla.IsEnabled = true;

                cmbDevFabricaTipo.ItemsSource = new[] { "Tenis" };

                cmbDevFabricaTipo.SelectedIndex = 0;
            }
        }

        private void cmbDevFabricaTipo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string tipoArticulo = cmbDevFabricaTipoArticulo.SelectedItem?.ToString() ?? "";

            cmbDevFabricaColor.ItemsSource = null;
            cmbDevFabricaTalla.ItemsSource = null;

            using var db = new AppDbContext();

            if (tipoArticulo == "Uniforme")
            {
                string tipo = cmbDevFabricaTipo.SelectedItem?.ToString() ?? "";

                cmbDevFabricaColor.ItemsSource = db.UniformesCanje
                    .Where(u => u.Tipo == tipo)
                    .Select(u => u.Color)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();
            }
            else if (tipoArticulo == "Tenis")
            {
                cmbDevFabricaColor.ItemsSource = new[] { "-" };

                cmbDevFabricaColor.SelectedIndex = 0;

                cmbDevFabricaTalla.ItemsSource = db.TenisCanjes
                    .OrderBy(t => t.Talla)
                    .Select(t => t.Talla)
                    .ToList();
            }
        }

        private void cmbDevFabricaColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string tipoArticulo = cmbDevFabricaTipoArticulo.SelectedItem?.ToString() ?? "";

            if (tipoArticulo != "Uniforme")
                return;

            string tipo = cmbDevFabricaTipo.SelectedItem?.ToString() ?? "";

            string color = cmbDevFabricaColor.SelectedItem?.ToString() ?? "";

            using var db = new AppDbContext();

            cmbDevFabricaTalla.ItemsSource = db.UniformesCanje
                .Where(u =>
                    u.Tipo == tipo &&
                    u.Color == color)
                .Select(u => u.Talla)
                .Distinct()
                .OrderBy(x => x)
                .ToList();
        }
        
        private void BtnRegistrarDevolucionFabrica_Click(object sender, RoutedEventArgs e)
        {
            string tipoDevolucion = cmbDevFabricaTipoDevolucion.SelectedItem?.ToString() ?? "";

            string tipoArticulo = cmbDevFabricaTipoArticulo.SelectedItem?.ToString() ?? "";

            if (string.IsNullOrWhiteSpace(tipoDevolucion) ||
                string.IsNullOrWhiteSpace(tipoArticulo))
            {
                MessageBox.Show("Seleccione el tipo de devolución y el tipo de artículo");

                return;
            }

            if (!int.TryParse(
                    txtDevFabricaCantidad.Text,
                    out int cantidad) ||
                cantidad <= 0)
            {
                MessageBox.Show("Capture una cantidad válida.");

                return;
            }

            using var db = new AppDbContext();
            using var transaccion = db.Database.BeginTransaction();

            try
            {
                if (tipoArticulo == "Utiles")
                {
                    string seleccionado = cmbDevFabricaTipo.SelectedItem?.ToString() ?? "";

                    if (string.IsNullOrWhiteSpace(seleccionado))
                    {
                        MessageBox.Show("Seleccione un paquete.");
                        return;
                    }

                    string numeroTexto = seleccionado.Replace("Paquete", "").Trim();

                    if (!int.TryParse(numeroTexto, out int numeroPaquete))
                    {
                        MessageBox.Show("Paquete inválido.");
                        return;
                    }

                    var paquete = db.PaquetesCanje
                        .FirstOrDefault(p =>
                            p.NumeroPaquete == numeroPaquete);

                    if (paquete == null)
                    {
                        MessageBox.Show("No se encontró el paquete.");

                        return;
                    }

                    if (paquete.Existencia < cantidad)
                    {
                        MessageBox.Show(
                            $"No hay suficiente existencia.\n\n" +
                            $"Disponibles: {paquete.Existencia}");

                        return;
                    }

                    paquete.Existencia -= cantidad;

                    db.DevolucionesFabricaCanje.Add(
                        new DevolucionFabricaCanje
                        {
                            Fecha = DateTime.Now,

                            TipoDevolucion = tipoDevolucion,

                            TipoArticulo = "Utiles",

                            PaqueteCanjeId = paquete.Id,

                            Cantidad = cantidad,

                            EstadoReposicion =
                                tipoDevolucion == "Defectuosa"
                                    ? "Pendiente"
                                    : "",

                            FechaReposicion = null,

                            Observacion = txtDevFabricaObservacion.Text.Trim()
                        });
                }
                else if (tipoArticulo == "Uniforme")
                {
                    string tipo = cmbDevFabricaTipo.SelectedItem?.ToString() ?? "";

                    string color = cmbDevFabricaColor.SelectedItem?.ToString() ?? "";

                    string talla = cmbDevFabricaTalla.SelectedItem?.ToString() ?? "";

                    var uniforme = db.UniformesCanje
                        .FirstOrDefault(u =>
                            u.Tipo == tipo &&
                            u.Color == color &&
                            u.Talla == talla);

                    if (uniforme == null)
                    {
                        MessageBox.Show("No se encontró el uniforme.");

                        return;
                    }

                    if (uniforme.Existencia < cantidad)
                    {
                        MessageBox.Show(
                            $"No hay suficiente existencia.\n\n" +
                            $"Disponibles: {uniforme.Existencia}");

                        return;
                    }

                    uniforme.Existencia -= cantidad;

                    db.DevolucionesFabricaCanje.Add(
                        new DevolucionFabricaCanje
                        {
                            Fecha = DateTime.Now,
                            TipoDevolucion = tipoDevolucion,
                            TipoArticulo = "Uniforme",
                            UniformeCanjeId = uniforme.Id,
                            Cantidad = cantidad,

                            EstadoReposicion =
                                tipoDevolucion == "Defectuosa"
                                    ? "Pendiente"
                                    : "",

                            FechaReposicion = null,

                            Observacion = txtDevFabricaObservacion.Text.Trim()
                        });
                }
                else if (tipoArticulo == "Tenis")
                {
                    string talla = cmbDevFabricaTalla.SelectedItem?.ToString() ?? "";

                    var tenis = db.TenisCanjes
                        .FirstOrDefault(t =>
                            t.Talla == talla);

                    if (tenis == null)
                    {
                        MessageBox.Show("No se encontró la talla.");

                        return;
                    }

                    if (tenis.Existencia < cantidad)
                    {
                        MessageBox.Show(
                            $"No hay suficiente existencia.\n\n" +
                            $"Disponibles: {tenis.Existencia}");

                        return;
                    }

                    tenis.Existencia -= cantidad;

                    db.DevolucionesFabricaCanje.Add(
                        new DevolucionFabricaCanje
                        {
                            Fecha = DateTime.Now,
                            TipoDevolucion = tipoDevolucion,
                            TipoArticulo = "Tenis",
                            TenisCanjeId = tenis.Id,
                            Cantidad = cantidad,

                            EstadoReposicion =
                                tipoDevolucion == "Defectuosa"
                                    ? "Pendiente"
                                    : "",

                            FechaReposicion = null,

                            Observacion = txtDevFabricaObservacion.Text.Trim()
                        });
                }

                db.SaveChanges();
                transaccion.Commit();

                MessageBox.Show(
                    tipoDevolucion == "Defectuosa"
                        ? "Devolución defectuosa registrada. Quedó pendiente de reposición."
                        : "Devolución final registrada correctamente.",
                    "Devolución a fábrica",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                LimpiarDevolucionFabrica();

                CargarUniformes();
                CargarTenis();
                CargarComboEntregaTenis();
                CargarResumenCanjes();
            }
            catch (Exception ex)
            {
                transaccion.Rollback();

                MessageBox.Show(
                    ex.InnerException?.Message ?? ex.Message,
                    "Error al registrar devolución",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void LimpiarDevolucionFabrica()
        {
            cmbDevFabricaTipoDevolucion.SelectedIndex = -1;
            cmbDevFabricaTipoArticulo.SelectedIndex = -1;

            cmbDevFabricaTipo.ItemsSource = null;
            cmbDevFabricaColor.ItemsSource = null;
            cmbDevFabricaTalla.ItemsSource = null;

            txtDevFabricaCantidad.Text = "1";
            txtDevFabricaObservacion.Clear();
        }

        private void CargarTallasPorTipo(string tipo)
        {
            string[] tallas;

            switch (tipo)
            {
                case "Falda":
                    tallas = new[]
                    {
                        "6", "8", "10", "12", "14",
                        "16", "18/CH", "M", "G"
                    };
                    break;

                case "Camisa Blanca":
                case "Playera Blanca":
                    tallas = new[]
                    {
                        "4", "6", "8", "10", "12",
                        "14", "16", "18/CH", "M", "G"
                    };
                    break;

                case "Jumper":
                    tallas = new[]
                    {
                        "12", "14", "16", "18/CH"
                    };
                    break;

                case "Pantalón":
                    tallas = new[]
                    {
                        "4", "6", "8", "10", "12", "14",
                        "16", "18", "20", "22", "24", "26",
                        "30", "32", "34", "36", "38"
                    };
                    break;

                case "Short/Falda":
                    tallas = new[]
                    {
                        "4", "6", "8"
                    };
                    break;

                default:
                    tallas = Array.Empty<string>();
                    break;
            }

            cmbTallaUniforme.ItemsSource = tallas;
            cmbTallaUniforme.SelectedIndex = -1;
        }

        private void CargarColoresPorTipo(string tipo)
        {
            string[] colores;

            switch (tipo)
            {
                case "Falda":
                    colores = new[]
                    {
                        "Azul",
                        "Tinto"
                    };
                    break;

                case "Camisa Blanca":
                case "Playera Blanca":
                    colores = new[]
                    {
                        "Blanco"
                    };
                    break;

                case "Jumper":
                    colores = new[]
                    {
                        "Marino",
                        "Rosa",
                        "Celeste",
                        "Tinto"
                    };
                    break;

                case "Pantalón":
                    colores = new[]
                    {
                        "Azul",
                        "Tinto",
                        "Kaki"
                    };
                    break;

                case "Short/Falda":
                    colores = new[]
                    {
                        "Azul"
                    };
                    break;

                default:
                    colores = Array.Empty<string>();
                    break;
            }

            cmbColorUniforme.ItemsSource = colores;
            cmbColorUniforme.SelectedIndex =
                colores.Length == 1 ? 0 : -1;
        }

        private void CargarResumenCanjes()
        {
            using var db = new AppDbContext();

            int paquetesEntregados = db.PaquetesCanje
                .Sum(p => (int?)p.Entregados) ?? 0;

            int canjesUniforme = db.CanjeUniformes.Count();

            int conjuntosEntregados = db.CanjeUniformes
                .Sum(c => (int?)c.CantidadConjuntos) ?? 0;

            int prendasEntregadas = db.UniformesCanje
                .Sum(u => (int?)u.Entregados) ?? 0;

            int tenisEntregados = db.TenisCanjes
                .Sum(t => (int?)t.Entregados) ?? 0;

            int valesUniforme = db.DetalleCanjeUniformes
                .Count(d => d.Pendiente);

            int valesTenis = db.ValesTenisCanje
                .Count(v => v.Pendiente);

            int valesPendientes = valesUniforme + valesTenis;

            lblPaquetesEntregados.Text =
                paquetesEntregados.ToString();

            lblCanjesUniforme.Text =
                canjesUniforme.ToString();

            lblConjuntosEntregados.Text =
                conjuntosEntregados.ToString();

            lblPrendasEntregadas.Text =
                prendasEntregadas.ToString();

            lblTenisEntregados.Text =
                tenisEntregados.ToString();

            lblValesPendientes.Text =
                valesPendientes.ToString();

            lblValesPendientes.Foreground =
                valesPendientes > 0
                    ? Brushes.Red
                    : Brushes.Green;
            var resumenPaquete = db.PaquetesCanje
                .OrderBy(p => p.NumeroPaquete)
                .Select(p => new ResumenPaqueteGrid
                {
                    NumeroPaquete = p.NumeroPaquete,
                    Existencia = p.Existencia,
                    Entregados = p.Entregados
                })
                .ToList();

            dgResumenPaquetes.ItemsSource =
                resumenPaquete;

            var resumenUniformes = db.UniformesCanje
                .GroupBy(u => u.Tipo)
                .Select(grupo => new ResumenUniformeGrid
                {
                    Tipo = grupo.Key,
                    Existencia = grupo.Sum(u => u.Existencia),
                    Entregados = grupo.Sum(u =>u.Entregados)
                })
                .OrderBy(u => u.Tipo)
                .ToList();

            dgResumenUniformes.ItemsSource =
                resumenUniformes;

            var resumenTenis = db.TenisCanjes
                .OrderBy(t => t.Talla)
                .Select(t => new ResumenTenisGrid
                {
                    Talla = t.Talla,
                    Existencia = t.Existencia,
                    Entregados = t.Entregados
                })
                .ToList();

            dgResumenTenis.ItemsSource = resumenTenis;
        }

        private void cmbEntregaPaquete_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbEntregaPaquete.SelectedItem is PaqueteCanje paquete)
            {
                lblExistenciaPaquete.Text =
                    $"Disponibles: {paquete.Existencia}";
            }
            else
            {
                lblExistenciaPaquete.Text = "Disponibles: 0";
            }

            ValidarEntregaPaquete();
        }

        private void txtEntregaPaquete_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidarEntregaPaquete();
        }

        private void ValidarEntregaPaquete()
        {
            if (btnRegistrarEntregaPaquete == null)
                return;

            bool paqueteSeleccionado =
                cmbEntregaPaquete.SelectedItem is PaqueteCanje;

            bool cantidadValida =
                int.TryParse(txtEntregaPaquete.Text, out int cantidad) &&
                cantidad > 0;

            btnRegistrarEntregaPaquete.IsEnabled = paqueteSeleccionado && cantidadValida;

        }

        private void SoloEnteros_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(char.IsDigit);
        }
    }
}
