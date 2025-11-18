using Microsoft.EntityFrameworkCore;
using Appetite_App.Data;
using Appetite_App.Models;

namespace Appetite_App.Repositories
{
    public class OrdenRepository : IOrdenRepository
    {
        private readonly AppetiteContext _context;

        public OrdenRepository(AppetiteContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PreOrden>> GetAllAsync()
        {
            return await _context.Ordenes.Include(o => o.Usuario).ToListAsync();
        }

        public async Task<PreOrden?> GetByIdAsync(int id)
        {
            return await _context.Ordenes
                                 .Include(o => o.Detalles)
                                 .FirstOrDefaultAsync(o => o.IdOrden == id);
        }

        public async Task AddAsync(PreOrden orden)
        {
            _context.Ordenes.Add(orden);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(PreOrden orden)
        {
            _context.Ordenes.Update(orden);
            await _context.SaveChangesAsync();
        }
    }
}
