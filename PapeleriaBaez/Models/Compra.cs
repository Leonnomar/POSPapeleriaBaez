using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PapeleriaBaez.Models
{
    public class Compra
    {
        public int Id { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;

        public string LugarCompra { get; set; } = "";

        public string Observaciones { get; set; } = "";

        public decimal Total { get; set; }

        public ICollection<DetalleCompra> Detalles { get; set; }
            = new List<DetalleCompra>();
    }
}
