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

        public DbSet<Apartado> Apartados { get; set; }

        public DbSet<DetalleApartado> detalleApartados { get; set; }

        public DbSet<AbonoApartado> abonoApartados { get; set; }

        public DbSet<TenisCanje> TenisCanjes { get; set; }

        public DbSet<ValeTenisCanje> ValesTenisCanje { get; set; }

        public DbSet<ApartadoCanje> ApartadosCanje { get; set; }

        public DbSet<DetalleApartadoCanje> DetalleApartadosCanjes { get; set; }

        public DbSet<DevolucionClienteCanje> DevolucionesClienteCanje { get; set; }

        public DbSet<DevolucionFabricaCanje> DevolucionesFabricaCanje { get; set; }

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

            modelBuilder.Entity<DetalleApartado>()
                .HasOne(d => d.Apartado)
                .WithMany(a => a.Detalles)
                .HasForeignKey(d => d.ApartadoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DetalleApartado>()
                .HasOne(d => d.Producto)
                .WithMany()
                .HasForeignKey(d => d.ProductoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AbonoApartado>()
                .HasOne(a => a.Apartado)
                .WithMany(a => a.Abonos)
                .HasForeignKey(a => a.ApartadoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ValeTenisCanje>()
                .HasOne(v => v.TenisCanje)
                .WithMany()
                .HasForeignKey(v => v.TenisCanjeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DetalleApartadoCanje>()
                .HasOne(d => d.ApartadoCanje)
                .WithMany(a => a.Detalles)
                .HasForeignKey(d => d.ApartadoCanjeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DetalleApartadoCanje>()
                .HasOne(d => d.PaqueteCanje)
                .WithMany()
                .HasForeignKey(d => d.PaqueteCanjeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DetalleApartadoCanje>()
                .HasOne(d => d.UniformeCanje)
                .WithMany()
                .HasForeignKey(d => d.UniformeCanjeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DetalleApartadoCanje>()
                .HasOne(d => d.TenisCanje)
                .WithMany()
                .HasForeignKey(d => d.TenisCanjeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DevolucionClienteCanje>()
                .HasOne(d => d.UniformeDevuelto)
                .WithMany()
                .HasForeignKey(d => d.UniformeDevueltoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DevolucionClienteCanje>()
                .HasOne(d => d.UniformeEntregado)
                .WithMany()
                .HasForeignKey(d => d.UniformeEntregadoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DevolucionClienteCanje>()
                .HasOne(d => d.TenisDevuelto)
                .WithMany()
                .HasForeignKey(d => d.TenisDevueltoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DevolucionClienteCanje>()
                .HasOne(d => d.TenisEntregado)
                .WithMany()
                .HasForeignKey(d => d.TenisEntregadoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DevolucionFabricaCanje>()
                .HasOne(d => d.PaqueteCanje)
                .WithMany()
                .HasForeignKey(d => d.PaqueteCanjeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DevolucionFabricaCanje>()
                .HasOne(d => d.UniformeCanje)
                .WithMany()
                .HasForeignKey(d => d.UniformeCanjeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DevolucionFabricaCanje>()
                .HasOne(d => d.TenisCanje)
                .WithMany()
                .HasForeignKey(d => d.TenisCanjeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
