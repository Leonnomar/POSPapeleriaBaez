using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PapeleriaBaez.Models
{
    public class CanjeUniforme
    {
        public int Id { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;

        public int CantidadConjuntos { get; set; }

        public string Observaciones { get; set; } = "";

        public ICollection<DetalleCanjeUniforme> Detalles { get; set; }
            = new List<DetalleCanjeUniforme>();
    }
}
