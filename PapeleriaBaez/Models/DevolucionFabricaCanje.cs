using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PapeleriaBaez.Models
{
    public class DevolucionFabricaCanje
    {
        public int Id { get; set; }

        public DateTime Fecha { get; set; }

        public string TipoDevolucion { get; set; } = "";

        public string TipoArticulo { get; set; } = "";

        public int? PaqueteCanjeId { get; set; }

        public int? UniformeCanjeId { get; set; }

        public int? TenisCanjeId { get; set; }

        public int Cantidad { get; set; } = 1;

        public string EstadoReposicion { get; set; } = "";

        public DateTime? FechaReposicion { get; set; }

        public string Observacion { get; set; } = "";

        public PaqueteCanje? PaqueteCanje { get; set; }

        public UniformeCanje? UniformeCanje { get; set; }

        public TenisCanje? TenisCanje { get; set; }
    }
}
