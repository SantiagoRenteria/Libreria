using Application.Common;
using Application.DTOs;
using Domain.Common;

namespace Application.Interfaces
{
    public interface IAutorService
    {
        Task<Result<IEnumerable<AutorDto>>> ObtenerTodosAsync();

        Task<Result<AutorDto>> ObtenerAutorAsync(int id);

        Task<Result<PagedResult<AutorConLibrosDto>>> ObtenerAutoresConLibrosPaginadoAsync(int page, int pageSize);

        Task<Result> RegistrarAutorAsync(AutorDto autorDto);

        Task<Result> ActualizarAutorAsync(AutorDto autorDto);

        Task<Result> EliminarAutor(int id);
    }
}