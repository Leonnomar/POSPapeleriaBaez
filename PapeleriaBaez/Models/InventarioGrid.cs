using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PapeleriaBaez.Models
{
    public class InventarioGrid
    {
        public int Id { get; set; }

        public string Codigo { get; set; } = "";

        public string Nombre { get; set; } = "";

        public string Categoria { get; set; } = "";

        public decimal Costo { get; set; }

        public decimal Precio { get; set; }

        public int Stock { get; set; }

        public int StockMinimo { get; set; }

        public decimal ValorInventario =>
            Costo * Stock;

        public string Estado
        {
            get
            {
                if (Stock <= 0)
                    return "🔴 Agotado";

                if (Stock <= StockMinimo)
                    return "🟡 Stock Bajo";

                return "🟢 Disponible";
            }
        }
    }
}
