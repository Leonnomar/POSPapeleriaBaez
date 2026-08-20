using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PapeleriaBaez.Models
{
    public class Apartado
    {
        public int Id { get; set; }

        public string Cliente { get; set; } = "";

        public DateTime Fecha { get; set; }

        public DateTime? FechaEntrega { get; set; }

        public decimal Total { get; set; }

        public decimal Pagado { get; set; }

        public decimal SaldoPendiente { get; set; }

        public string Estado { get; set; } = "Pendiente";

        public ICollection<DetalleApartado> Detalles { get; set; }
            = new List<DetalleApartado>();

        public ICollection<AbonoApartado> Abonos { get; set; }
            = new List<AbonoApartado>();
    }
}
