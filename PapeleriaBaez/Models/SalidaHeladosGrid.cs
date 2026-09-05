using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PapeleriaBaez.Models
{
    public class SalidaHeladosGrid
    {
        public int Id { get; set; }

        public DateTime FechaSalida { get; set; }

        public int Productos { get; set; }

        public int Piezas { get; set; }

        public string Estado { get; set; } = "";

        public decimal Deuda { get; set; }
    }
}
