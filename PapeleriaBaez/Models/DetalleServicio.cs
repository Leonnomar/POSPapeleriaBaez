using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PapeleriaBaez.Models
{
    public class DetalleServicio
    {
        public int Id { get; set; }

        public int ServicioId { get; set; }

        public Servicio Servicio { get; set; } = null!;

        public int? ProductoId { get; set; }

        public Producto? Producto { get; set; }

        public int Cantidad { get; set; } = 1;
    }
}
