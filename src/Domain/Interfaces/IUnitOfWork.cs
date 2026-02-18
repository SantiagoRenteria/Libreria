namespace Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IAutorRepository Autores { get; }
        ILibroRepository Libros { get; } 
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
        Task<int> SaveAsync();
    }
}
