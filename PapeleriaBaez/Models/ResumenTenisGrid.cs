using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PapeleriaBaez.Models
{
    public class ResumenTenisGrid
    {
        public string Talla { get; set; } = "";

        public int Existencia { get; set; }

        public int Entregados { get; set; }

        public int TotalRecibido =>
            Existencia + Entregados;
    }
}
