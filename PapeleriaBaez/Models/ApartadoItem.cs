using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PapeleriaBaez.Models
{
    public class ApartadoItem
    {
        public int ProductoId { get; set; }

        public string Codigo { get; set; } = "";

        public string Nombre { get; set; } = "";

        public int Cantidad { get; set; } = 1;

        public decimal Precio { get; set; }

        public decimal Importe =>
            Cantidad * Precio;
    }
}
