using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PapeleriaBaez.Models
{
    public class SalidaHelados
    {
        public int Id { get; set; }

        public DateTime FechaSalida { get; set; }

        public DateTime? FechaCierre { get; set; }

        public string Responsable { get; set; } = "";

        public string Estado { get; set; } = "Pendiente";

        public ICollection<DetalleSalidaHelados> Detalles { get; set; }
            = new List<DetalleSalidaHelados>();
    }
}
