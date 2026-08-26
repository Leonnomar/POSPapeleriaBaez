using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PapeleriaBaez.Models
{
    public class ApartadoCanjeGrid
    {
        public int Id { get; set; }

        public DateTime Fecha { get; set; }

        public string Referencia { get; set; } = "";

        public string Estado { get; set; } = "";

        public int CantidadArticulos { get; set; }
    }
}
