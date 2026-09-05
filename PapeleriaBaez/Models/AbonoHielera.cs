using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PapeleriaBaez.Models
{
    public class AbonoHielera
    {
        public int Id { get; set; }

        public int SalidaHeladosId { get; set; }

        public DateTime Fecha { get; set; }

        public decimal Cantidad { get; set; }

        public SalidaHelados SalidaHelados { get; set; } = null!;
    }
}
