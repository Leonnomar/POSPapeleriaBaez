using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace PapeleriaBaez.Models
{
    public class Venta
    {
        public int Id { get; set; }

        public DateTime Fecha { get; set; }

        public ICollection<DetalleVenta> Detalles { get; set; }
            = new List<DetalleVenta>();
    }
}
