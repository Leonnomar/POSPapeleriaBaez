using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PapeleriaBaez.Models
{
    public class AbonoHelados
    {
        public int Id { get; set; }

        public DateTime Fecha { get; set; }

        public decimal Cantidad { get; set; }

        public string Observacion { get; set; } = "";
    }
}
