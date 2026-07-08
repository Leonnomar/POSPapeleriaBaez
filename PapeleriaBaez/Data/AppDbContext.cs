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

        public DbSet<Venta> Ventas => Set<Venta>();

        public DbSet<DetalleVenta> DetalleVentas => Set<DetalleVenta>();

        public DbSet<Compra> Compras => Set<Compra>();

        public DbSet<DetalleCompra> DetalleCompras => Set<DetalleCompra>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string ruta = @"C:\PapeleriaBaez\PapeleriaBaez.db";

            optionsBuilder.UseSqlite($"Data Source={ruta}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DetalleCompra>()
                .HasOne(d => d.Producto)
                .WithMany()
                .HasForeignKey(d => d.ProductoId);

            modelBuilder.Entity<DetalleCompra>()
                .HasOne(d => d.Compra)
                .WithMany(c => c.Detalles)
                .HasForeignKey(d => d.CompraId);
        }
    }
}
