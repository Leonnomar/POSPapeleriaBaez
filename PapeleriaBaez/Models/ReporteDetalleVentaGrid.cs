using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PapeleriaBaez.Models
{
    public class ReporteDetalleVentaGrid
    {
        public string Codigo { get; set; } = "";

        public string Producto { get; set; } = "";

        public int Cantidad { get; set; }

        public decimal Precio { get; set; }

        public decimal Importe { get; set; }
    }
}
