using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PapeleriaBaez.Models;

namespace PapeleriaBaez.Data
{
    public static class DbInitializer
    {
        public static void Seed(AppDbContext db)
        {
            if (db.Categorias.Any())
                return;

            var categorias = new List<Categoria>
            {
                new() { Nombre = "Papelería" },
                new() { Nombre = "Limpieza" },
                new() { Nombre = "Chucherías" },
                new() { Nombre = "Helados" },
                new() { Nombre = "Antojitos" },
                new() { Nombre = "Uniforme" },
                new() { Nombre = "Faldas de Mezclilla" },
                new() { Nombre = "Regalos" },
                new() { Nombre = "Servicios" },
                new() { Nombre = "Otros" }
            };

            db.Categorias.AddRange(categorias);
            db.SaveChanges();
        }
    }
}
