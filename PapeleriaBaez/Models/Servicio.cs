using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PapeleriaBaez.Models
{
    public class Servicio
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string Categoria { get; set; } = string.Empty;

        public string Descripcion { get; set; } = string.Empty;

        public decimal PrecioBase { get; set; }

        public bool PermitePrecioManual { get; set; }

        public bool Activo { get; set; } = true;

        public ICollection<DetalleServicio> Detalles { get; set; }
        = new List<DetalleServicio>();
    }
}
