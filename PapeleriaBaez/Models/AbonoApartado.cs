using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PapeleriaBaez.Models
{
    public class AbonoApartado
    {
        public int Id { get; set; }

        public int ApartadoId { get; set; }

        public DateTime Fecha { get; set; }

        public decimal Monto { get; set; }

        public Apartado Apartado { get; set; } = null!;
    }
}
