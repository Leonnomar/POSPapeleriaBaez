using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PapeleriaBaez.Models
{
    public class ProductoMasVendidoGrid
    {
        public string Codigo { get; set; } = "";

        public string Producto { get; set; } = "";

        public int Cantidad { get; set; }

        public decimal Ingresos { get; set; }
    }
}
