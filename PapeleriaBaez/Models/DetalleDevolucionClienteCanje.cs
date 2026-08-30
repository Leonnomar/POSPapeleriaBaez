using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PapeleriaBaez.Models
{
    public class DetalleDevolucionClienteCanje
    {
        public int Id { get; set; }

        public int DevolucionClienteCanjeId { get; set; }

        public string TipoCambio { get; set; } = "";

        public string TipoArticulo { get; set; } = "";

        public int? UniformeDevueltoId { get; set; }

        public int? UniformeEntregadoId { get; set; }

        public int? TenisDevueltoId { get; set; }

        public int? TenisEntregadoId { get; set; }

        public DevolucionClienteCanje DevolucionClienteCanje { get; set; } = null!;

        public UniformeCanje? UniformeDevuelto {  get; set; }

        public UniformeCanje? UniformeEntregado { get; set; }

        public TenisCanje? TenisDevuelto { get; set; }

        public TenisCanje? TenisEntregado { get; set; }
    }
}