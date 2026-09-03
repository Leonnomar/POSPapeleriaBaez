using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PapeleriaBaez.Models
{
    public class DetalleSalidaHelados
    {
        public int Id { get; set; }

        public int SalidaHeladosId { get; set; }

        public int ProductoId { get; set; }

        public int CantidadSalida { get; set; }

        public int CantidadRegresada { get; set; }

        public int CantidadVendida { get; set; }

        public int CantidadFiada { get; set; }

        public SalidaHelados SalidaHelados { get; set; } = null!;

        public Producto Producto { get; set; } = null!;
    }
}
