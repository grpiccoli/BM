using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BiblioMit.Models;
using BiblioMit.Data;
using Microsoft.EntityFrameworkCore;

namespace BiblioMit.Services
{
    public class CentreService : ICentre
    {
        private readonly ApplicationDbContext _context;

        public CentreService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Centre> GetFilteredCentres(int page, int rpp, string searchQuery)
        {
            if (String.IsNullOrEmpty(searchQuery))
            {
                return( _context.Centre
                    .Where(c =>
                    c.Address.Contains(searchQuery) ||
                    c.Company.BsnssName.Contains(searchQuery) ||
                    c.Comuna.Name.Contains(searchQuery))
                    .OrderBy(c => c.Id)
                    .ToList()
                    .GetRange(page*rpp-1,rpp) );
            }
            else
            {
                return( _context.Centre
                    .OrderBy(c => c.Id)
                    .ToList()
                    .GetRange(page * rpp - 1, rpp) );
            }
        }
    }
}
