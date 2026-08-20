using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PapeleriaBaez.Models
{
    public class DetalleApartado
    {
        public int Id { get; set; }

        public int ApartadoId { get; set; }

        public int ProductoId { get; set; }

        public int Cantidad { get; set; }

        public decimal Precio { get; set; }

        public decimal Importe { get; set; }

        public Apartado Apartado { get; set; } = null!;

        public Producto Producto { get; set; } = null!;
    }
}
