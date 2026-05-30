
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PapeleriaBaez.Models;

namespace PapeleriaBaez.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Categoria> Categorias => Set<Categoria>();

        public DbSet<Producto> Productos => Set<Producto>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=PapeleriaBaez.db");
        }
    }
}
