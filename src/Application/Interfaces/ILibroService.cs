using Application.DTOs;
using Domain.Common;

namespace Application.Interfaces
{
    public interface ILibroService
    {
        Task<Result<bool>> RegistrarLibroAsync(LibroCreateDto libroDto);

        Task<Result<bool>> EliminarLibroAsync(int id);
    }
}
