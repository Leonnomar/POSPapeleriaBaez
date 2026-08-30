using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PapeleriaBaez.Models
{
    public class DevolucionClienteCanje
    {
        public int Id { get; set; }

        public DateTime Fecha { get; set; }

        public string Observacion { get; set; } = "";

        public ICollection<DetalleDevolucionClienteCanje> Detalles { get; set; }
            = new List<DetalleDevolucionClienteCanje>();
    }
}
