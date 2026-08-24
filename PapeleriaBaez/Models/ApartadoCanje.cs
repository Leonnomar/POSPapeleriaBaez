using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PapeleriaBaez.Models
{
    public class ApartadoCanje
    {
        public int Id { get; set; }

        public DateTime Fecha { get; set; }

        public DateTime? FechaEntrega { get; set; }

        public string Referencia { get; set; } = "";

        public string Estado { get; set; } = "Pendiente";

        public ICollection<DetalleApartadoCanje> Detalles { get; set; }
        = new List<DetalleApartadoCanje>();
    }
}
