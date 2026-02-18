using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IAutorRepository
    {
        Task<IEnumerable<Autor>> GetAllAsync();
        Task<IEnumerable<Autor>> GetAllWithLibrosAsync();
        Task<(IEnumerable<Autor> Items, int Total)> GetPaginatedsAsync(int page, int pageSize);
        Task<bool> Exists(int id); 
        Task AddAsync(Autor autor);
        void Update(Autor autor);
        Task DeleteAsync(int id);
        Task<Autor> GetById(int id);
    }
}