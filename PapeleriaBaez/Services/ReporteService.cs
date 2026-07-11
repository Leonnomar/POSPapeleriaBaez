using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PapeleriaBaez.Data;

namespace PapeleriaBaez.Services
{
    public class ReporteService
    {
        private readonly AppDbContext _db;

        public ReporteService(AppDbContext db)
        {
            _db = db;
        }
    }
}
