using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        
        private IDbContextTransaction? _transaction;
        public IAutorRepository Autores { get; }
        public ILibroRepository Libros { get; }

        public UnitOfWork(ApplicationDbContext context, IAutorRepository autores, ILibroRepository libros)
        {
            _context = context;
            Autores = autores;
            Libros = libros;
        }

        public async Task BeginTransactionAsync()
        {
            // Solo inicia la transacción si no estamos en una base de datos en memoria
            if (_context.Database.IsRelational())
            {
                await _context.Database.BeginTransactionAsync();
            }
        }

        public async Task CommitAsync()
        {
            try
            {
                await _context.SaveChangesAsync(); // Guarda todo lo pendiente de los repos
                if (_transaction != null) await _transaction.CommitAsync();
            }
            catch
            {
                await RollbackAsync();
                throw;
            }
        }

        public async Task RollbackAsync()
        {
            if (_transaction != null) await _transaction.RollbackAsync();
        }

        public async Task<int> SaveAsync() => await _context.SaveChangesAsync();

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
