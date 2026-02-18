using Domain.Entities;

namespace Domain.Interfaces
{
    public interface ILibroRepository
    {
        Task<IEnumerable<Libro>> GetAllAsync();
        Task<int> CountLibrosAsync(int autorId);
        Task<bool> ExisteAutorAsync(int autorId);
        Task<bool> Exists(int id);
        Task AddAsync(Libro libro);
        Task DeleteAsync(int id);
    }
}