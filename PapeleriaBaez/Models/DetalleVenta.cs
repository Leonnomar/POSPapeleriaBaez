using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PapeleriaBaez.Models
{
    public class DetalleVenta
    {
        public int Id { get; set; }

        public int VentaId { get; set; }

        public Venta Venta { get; set; } = null!;

        public int ProductoId { get; set; }

        public Producto Producto { get; set; } = null!;

        public int Cantidad { get; set; }

        public decimal Precio { get; set; }

        public decimal Importe { get; set; }
    }
}
