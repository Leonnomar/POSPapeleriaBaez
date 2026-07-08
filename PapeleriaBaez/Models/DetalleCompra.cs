using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PapeleriaBaez.Models
{
    public class DetalleCompra
    {
        public int Id { get; set; }

        public int CompraId { get; set; }

        public Compra Compra { get; set; } = null!;

        public int ProductoId { get; set; }

        public Producto Producto { get; set; } = null!;

        public int Cantidad { get; set; }

        public decimal Costo { get; set; }

        public decimal Importe { get; set; }
    }
}
