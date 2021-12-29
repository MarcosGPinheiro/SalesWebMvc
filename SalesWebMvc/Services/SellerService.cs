using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SalesWebMvc.Models;
using SalesWebMvc.Services.Exceptions;

namespace SalesWebMvc.Services
{
    public class SellerService
    {
        private readonly SalesWebMvcContext _context;

        public SellerService(SalesWebMvcContext context)
        {
            _context = context;
        }

        public async Task<List<Seller>> FindAllAsync()
        {
            return await _context.Sellers.ToListAsync();
        }

        public async Task<Seller> FindByIdAsync(int id)
        {
            return await _context.Sellers.Include(obj => obj.Department).FirstOrDefaultAsync(sl => sl.Id == id);
        }

        public async Task InsertAsync(Seller obj)
        {
            _context.Add(obj);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(int id)
        {
            try
            {
                var obj = await _context.Sellers.FindAsync(id);
                _context.Sellers.Remove(obj);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                throw new IntegrityException("Can´t delete seller because he/she has sales");
            }

        }

        public async Task UpdateAsync(Seller obj)
        {
            if (! await _context.Sellers.AnyAsync(x => x.Id == obj.Id))
                throw new NotFoundException("Id not found");
            try
            {
                _context.Sellers.Update(obj);
              await  _context.SaveChangesAsync();
            }
            catch (DbConcurrencyException e)
            {
                throw new DbConcurrencyException(e.Message);
            }
        }
    }
}
