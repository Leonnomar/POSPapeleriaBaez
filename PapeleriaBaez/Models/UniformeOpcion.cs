using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PapeleriaBaez.Models
{
    public class UniformeOpcion
    {
        public int Id { get; set; }

        public string Tipo { get; set; } = "";

        public string Color { get; set; } = "";

        public string Talla { get; set; } = "";

        public int Existencia { get; set; }

        public string Descripcion
        {
            get
            {
                string color = string.IsNullOrWhiteSpace(Color)
                    ? ""
                    : $" - {Color}";

                return $"{Tipo}{Color} - Talla {Talla} " +
                       $"(Disponibles: {Existencia})";
            }
        }
    }
}
