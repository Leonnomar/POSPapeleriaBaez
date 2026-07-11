using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PapeleriaBaez.Data;

namespace PapeleriaBaez.Services
{
    public class InventarioService
    {
        private readonly AppDbContext _db;

        public InventarioService(AppDbContext db)
        {
            _db = db;
        }
    }
}
