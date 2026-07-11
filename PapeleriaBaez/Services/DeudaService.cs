using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PapeleriaBaez.Data;

namespace PapeleriaBaez.Services
{
    public class DeudaService
    {
        private readonly AppDbContext _db;

        public DeudaService(AppDbContext db)
        {
            _db = db;
        }
    }
}
