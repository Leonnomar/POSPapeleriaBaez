using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PapeleriaBaez.Models
{
    public class VentaItem
    {
        public int ProductoId { get; set; }

        public string Codigo { get; set; } = "";

        public string Nombre { get; set; } = "";

        public decimal Precio { get; set; }

        public int Cantidad { get; set; }

        public decimal Descuento { get; set; }

        public decimal Importe
        {
            get
            {
                return (Precio * Cantidad) - Descuento;
            }
        }
    }
}
