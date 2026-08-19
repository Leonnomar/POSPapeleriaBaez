using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PapeleriaBaez.Models
{
    public class Deuda
    {
        public int Id { get; set; }

        public string Cliente { get; set; } = "";

        public string Concepto { get; set; } = "";

        public DateTime Fecha { get; set; }

        public decimal MontoOriginal { get; set; }

        public decimal SaldoPendiente { get; set; }

        public bool Pagada { get; set; }

        public int? VentaId { get; set; }

        public Venta? Venta { get; set; }

        [NotMapped]
        public string Estado =>
            Pagada ? "Pagada" : "Pendiente";

        public ICollection<AbonoDeuda> Abonos { get; set; }
            = new List<AbonoDeuda>();
    }
}
