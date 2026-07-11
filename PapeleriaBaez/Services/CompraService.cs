using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PapeleriaBaez.Data;
using PapeleriaBaez.Models;

namespace PapeleriaBaez.Services
{
    public class CompraService
    {
        private readonly AppDbContext _db;

        public CompraService(AppDbContext db)
        {
            _db = db;
        }

        public bool GuardarCompra(
            List<CompraItem> detalle,
            string lugar,
            string observaciones)
        {
            if (detalle.Count == 0)
                return false;

            var compra = new Compra
            {
                Fecha = DateTime.Now,
                LugarCompra = lugar,
                Observaciones = observaciones,
                Total = detalle.Sum(x => x.Importe)
            };

            _db.Compras.Add(compra);
            _db.SaveChanges();

            foreach (var item in detalle)
            {
                var producto = _db.Productos
                    .First(p => p.Id == item.ProductoId);

                decimal valorActual = producto.Stock * producto.Costo;
                decimal valorCompra = item.Cantidad * item.Costo;

                int nuevoStock = producto.Stock + item.Cantidad;

                decimal nuevoCosto = (valorActual + valorCompra) / nuevoStock;

                producto.Stock = nuevoStock;
                producto.Costo = Math.Round(nuevoCosto, 2);

                _db.DetalleCompras.Add(new DetalleCompra
                {
                    CompraId = compra.Id,
                    ProductoId = producto.Id,
                    Cantidad = item.Cantidad,
                    Costo = item.Costo,
                    Importe = item.Importe
                });
            }

            _db.SaveChanges();

            return true;
        }
    }
}
