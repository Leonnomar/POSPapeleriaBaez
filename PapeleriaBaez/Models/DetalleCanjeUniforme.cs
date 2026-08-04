using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PapeleriaBaez.Models
{
    public class DetalleCanjeUniforme
    {
        public int Id { get; set; }

        public int CanjeUniformeId { get; set; }

        public CanjeUniforme CanjeUniforme { get; set; } = null!;

        public int UniformeCanjeId { get; set; }

        public UniformeCanje UniformeCanje { get; set; } = null!;

        public int NumeroConjunto { get; set; }

        public int Cantidad { get; set; } = 1;

        public bool Pendiente { get; set; }

    }
}
