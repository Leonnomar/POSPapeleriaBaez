using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PapeleriaBaez.Models
{
    public class ApartadoCanjeItem
    {
        public string Tipo { get; set; } = "";

        public string Descripcion { get; set; } = "";

        public int Cantidad { get; set; }

        public int? PaqueteCanjeId { get; set; }

        public int? UniformeCanjeId { get; set; }

        public int? TenisCanjeId { get; set; }
    }
}
