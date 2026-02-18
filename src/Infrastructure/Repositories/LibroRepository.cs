using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class LibroRepository : ILibroRepository
    {
        private readonly ApplicationDbContext _context;

        public LibroRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Lectura
        public async Task<IEnumerable<Libro>> GetAllAsync() =>
            await _context.Libros.Include(l => l.Autor).AsNoTracking().ToListAsync();

        public async Task<int> CountLibrosAsync(int autorId) =>
            await _context.Libros.Where(l => l.AutorId == autorId).CountAsync();

        public async Task<bool> ExisteAutorAsync(int autorId) =>
            await _context.Autores.AnyAsync(a => a.Id == autorId);

        public async Task<bool> Exists(int id) =>
            await _context.Libros.AnyAsync(l => l.Id == id);

        // Escritura (Sin SaveChanges)
        public async Task AddAsync(Libro libro)
        {
            await _context.Libros.AddAsync(libro);
            // El guardado se delega al UoW
        }

        public async Task DeleteAsync(int id)
        {
            var libro = await _context.Libros.FindAsync(id);

            if (libro != null)
            {
                _context.Libros.Remove(libro);
                // No llamamos a SaveChanges aquí para permitir Rollback si es parte de una operación mayor
            }
        }
    }
}