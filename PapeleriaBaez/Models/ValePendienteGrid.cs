using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PapeleriaBaez.Models
{
    public class ValePendienteGrid
    {
        public int DetalleId { get; set; }

        public DateTime Fecha { get; set; }

        public int NumeroConjunto { get; set; }

        public string Tipo { get; set; } = "";

        public string Color { get; set; } = "";

        public string Talla { get; set; } = "";

    }
}
