using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PapeleriaBaez.Models
{
    public class ValeTenisCanje
    {
        public int Id { get; set; }

        public DateTime Fecha { get; set; }

        public int TenisCanjeId { get; set; }

        public bool Pendiente { get; set; }

        public DateTime? FechaEntrega { get; set; }

        public TenisCanje TenisCanje { get; set; } = null!;
    }
}
