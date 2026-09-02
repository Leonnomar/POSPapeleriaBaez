using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PapeleriaBaez.Models
{
    public class DevolucionFabricaGrid
    {
        public int Id { get; set; }

        public DateTime Fecha { get; set; }

        public string Descripcion { get; set; } = "";

        public int Cantidad { get; set; }

        public int CantidadRepuesta { get; set; }

        public int CantidadPendiente { get; set; }

        public string EstadoReposicion { get; set; } = "";
    }
}
