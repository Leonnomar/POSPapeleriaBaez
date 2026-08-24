using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PapeleriaBaez.Models
{
    public class DetalleApartadoCanje
    {
        public int Id { get; set; }

        public int ApartadoCanjeId { get; set; }

        public string Tipo { get; set; } = "";

        public int? PaqueteCanjeId { get; set; }

        public int? UniformeCanjeId { get; set; }

        public int? TenisCanjeId { get; set; }

        public int Cantidad { get; set; } = 1;

        public ApartadoCanje ApartadoCanje { get; set; } = null!;

        public PaqueteCanje? PaqueteCanje { get; set; }

        public UniformeCanje? UniformeCanje { get; set; }

        public TenisCanje? TenisCanje { get; set; }
    }
}
