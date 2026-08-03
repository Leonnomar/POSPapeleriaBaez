using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PapeleriaBaez.Models
{
    public class PaqueteCanje
    {
        public int Id { get; set; }

        public int NumeroPaquete { get; set; }

        public int Existencia { get; set; }

        public int Entregados { get; set; }
    }
}
