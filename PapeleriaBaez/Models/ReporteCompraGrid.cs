using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PapeleriaBaez.Models
{
    public class ReporteCompraGrid
    {
        public int Id { get; set; }

        public DateTime Fecha { get; set; }

        public int Productos { get; set; }

        public decimal Total { get; set; }
    }
}
