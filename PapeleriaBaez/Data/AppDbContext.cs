using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Office2019.Drawing.Model3D;
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

        public DbSet<PaqueteCanje> PaquetesCanje => Set<PaqueteCanje>();

        public DbSet<UniformeCanje> UniformesCanje => Set<UniformeCanje>();

        public DbSet<CanjeUniforme> CanjeUniformes =>
            Set<CanjeUniforme>();

        public DbSet<DetalleCanjeUniforme> DetalleCanjeUniformes =>
            Set<DetalleCanjeUniforme>();

        public DbSet<Deuda> Deudas { get; set; }

        public DbSet<AbonoDeuda> AbonosDeudas { get; set; }

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

            modelBuilder.Entity<DetalleVenta>()
                .HasOne(d => d.Venta)
                .WithMany(v => v.Detalles)
                .HasForeignKey(d => d.VentaId);

            modelBuilder.Entity<DetalleVenta>()
                .HasOne(d => d.Producto)
                .WithMany()
                .HasForeignKey(d => d.ProductoId);

            modelBuilder.Entity<DetalleCanjeUniforme>()
                .HasOne(d => d.CanjeUniforme)
                .WithMany(c => c.Detalles)
                .HasForeignKey(d => d.CanjeUniformeId);

            modelBuilder.Entity<DetalleCanjeUniforme>()
                .HasOne(d => d.UniformeCanje)
                .WithMany()
                .HasForeignKey(d => d.UniformeCanjeId);

            modelBuilder.Entity<AbonoDeuda>()
                .HasOne(a => a.Deuda)
                .WithMany(d => d.Abonos)
                .HasForeignKey(a => a.DeudaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Deuda>()
                .HasOne(d => d.Venta)
                .WithMany()
                .HasForeignKey(d => d.VentaId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
