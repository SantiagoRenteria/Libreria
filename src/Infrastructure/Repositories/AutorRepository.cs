using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class AutorRepository : IAutorRepository
    {
        private readonly ApplicationDbContext _context;

        public AutorRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Operaciones de Escritura: Solo preparan el Change Tracker, NO guardan
        public async Task AddAsync(Autor autor)
        {
            await _context.Autores.AddAsync(autor);
        }

        public void Update(Autor autor)
        {
            _context.Autores.Update(autor);
        }

        public async Task DeleteAsync(int id)
        {
            // Cargamos autor con sus libros para manejar la eliminación en cascada manual
            var autor = await _context.Autores
                .Include(a => a.Libros)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (autor != null)
            {
                // Removemos primero los libros para asegurar la integridad física
                if (autor.Libros.Any())
                {
                    _context.Libros.RemoveRange(autor.Libros);
                }

                _context.Autores.Remove(autor);
            }
        }


        public async Task<IEnumerable<Autor>> GetAllAsync()
            => await _context.Autores.ToListAsync();

        public async Task<Autor> GetById(int id)
            => await _context.Autores.FirstOrDefaultAsync(a => a.Id == id);

        public async Task<IEnumerable<Autor>> GetAllWithLibrosAsync()
        {
            return await _context.Autores
                .Include(a => a.Libros)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<(IEnumerable<Autor> Items, int Total)> GetPaginatedsAsync(int page, int pageSize)
        {
            var query = _context.Autores
                .Include(a => a.Libros)
                .AsNoTracking();

            int total = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        public async Task<bool> Exists(int id) =>
            await _context.Autores.AnyAsync(a => a.Id == id);
    }
}