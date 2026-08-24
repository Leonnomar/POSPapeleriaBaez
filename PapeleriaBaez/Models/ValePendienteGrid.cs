using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PapeleriaBaez.Models
{
    public class ValePendienteGrid
    {
        public int? DetalleUniformeId { get; set; }

        public int? ValeTenisId { get; set; }

        public DateTime Fecha { get; set; }

        public string Origen { get; set; } = "";

        public string Referencia { get; set; } = "";

        public string Tipo { get; set; } = "";

        public string Color { get; set; } = "";

        public string Talla { get; set; } = "";

    }
}
