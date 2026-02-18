using Application.Common;
using Application.DTOs;
using Application.Interfaces;
using Domain.Common;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class AutorService : IAutorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AutorService> _logger;

        public AutorService(IUnitOfWork unitOfWork, ILogger<AutorService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<PagedResult<AutorConLibrosDto>>> ObtenerAutoresConLibrosPaginadoAsync(int page, int pageSize)
        {
            try
            {
                var (autores, total) = await _unitOfWork.Autores.GetPaginatedsAsync(page, pageSize);

                var autoresDto = autores.Select(a => new AutorConLibrosDto
                {
                    Id = a.Id,
                    NombreCompleto = a.NombreCompleto,
                    Libros = a.Libros.Select(l => new LibroDto
                    {
                        Id = l.Id,
                        Titulo = l.Titulo,
                        Anio = l.Anio,
                        Genero = l.Genero
                    }).ToList()
                }).ToList();

                var pagedResult = new PagedResult<AutorConLibrosDto>
                {
                    Items = autoresDto,
                    TotalItems = total,
                    PageNumber = page,
                    PageSize = pageSize
                };

                return Result<PagedResult<AutorConLibrosDto>>.Success(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Consulta fallida: Error al obtener la lista paginada de autores.");
                return Result<PagedResult<AutorConLibrosDto>>.Failure(ErrorType.ServerFault, "Error al obtener la lista paginada de autores.");
            }
        }

        public async Task<Result<IEnumerable<AutorDto>>> ObtenerTodosAsync()
        {
            try
            {
                var autoresEntidad = await _unitOfWork.Autores.GetAllAsync();

                var autoresDto = autoresEntidad.Select(a => new AutorDto
                {
                    Id = a.Id,
                    NombreCompleto = a.NombreCompleto,
                    FechaNacimiento = a.FechaNacimiento,
                    CiudadProcedencia = a.CiudadProcedencia,
                    CorreoElectronico = a.CorreoElectronico
                }).ToList();

                return Result<IEnumerable<AutorDto>>.Success(autoresDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Consulta fallida: Error al obtener la lista paginada de autores.");
                return Result<IEnumerable<AutorDto>>.Failure(ErrorType.ServerFault, "Error al obtener la lista de autores.");
            }
        }

        public async Task<Result<AutorDto>> ObtenerAutorAsync(int id)
        {
            try
            {
                // 1. Obtenemos la entidad única
                var autor = await _unitOfWork.Autores.GetById(id);

                // 2. Validamos si existe (y si está activo, si ya aplicaste borrado lógico)
                if (autor is null)
                {
                    _logger.LogWarning("Consulta fallida: El autor con ID {AutorId} no existe.", id);
                    return Result<AutorDto>.Failure(ErrorType.NotFound, "El autor solicitado no existe o ha sido eliminado.");
                }

                // 3. Mapeo directo de Objeto a DTO (Sin Select, porque no es una lista)
                var autorDto = new AutorDto
                {
                    Id = autor.Id,
                    NombreCompleto = autor.NombreCompleto,
                    FechaNacimiento = autor.FechaNacimiento,
                    CiudadProcedencia = autor.CiudadProcedencia,
                    CorreoElectronico = autor.CorreoElectronico
                };

                return Result<AutorDto>.Success(autorDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Consulta fallida: Error crítico al obtener el autor con ID {AutorId}", id);
                return Result<AutorDto>.Failure(ErrorType.ServerFault, "Ocurrió un error interno al recuperar los datos del autor.");
            }
        }

        public async Task<Result> RegistrarAutorAsync(AutorDto autorDto)
        {
            try
            {
                var autor = new Autor
                {
                    NombreCompleto = autorDto.NombreCompleto,
                    FechaNacimiento = autorDto.FechaNacimiento,
                    CiudadProcedencia = autorDto.CiudadProcedencia,
                    CorreoElectronico = autorDto.CorreoElectronico
                };

                await _unitOfWork.Autores.AddAsync(autor);
                await _unitOfWork.SaveAsync();

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Creaccion fallida: Error crítico al guardar el autor");
                return Result.Failure(ErrorType.ServerFault, "No se pudo registrar al autor debido a un error interno.");
            }
        }

        public async Task<Result> ActualizarAutorAsync(AutorDto autorDto)
        {
            try
            {
                // Validación de Negocio: ¿Existe el autor que queremos actualizar?
                var existe = await _unitOfWork.Autores.Exists(autorDto.Id);
                if (!existe)
                {
                    _logger.LogWarning("Consulta fallida: El autor con ID {AutorId} no existe.", autorDto.Id);
                    return Result.Failure(ErrorType.NotFound, "El autor que intenta actualizar no existe.");
                }
                    

                var autor = new Autor
                {
                    Id = autorDto.Id,
                    NombreCompleto = autorDto.NombreCompleto,
                    FechaNacimiento = autorDto.FechaNacimiento,
                    CiudadProcedencia = autorDto.CiudadProcedencia,
                    CorreoElectronico = autorDto.CorreoElectronico
                };

                _unitOfWork.Autores.Update(autor);
                await _unitOfWork.SaveAsync();

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Actualizacion fallida: Error crítico al actualizar el autor con ID {AutorId}", autorDto.Id);
                return Result.Failure(ErrorType.ServerFault, "Error interno al actualizar los datos del autor.");
            }
        }

        public async Task<Result> EliminarAutor(int id)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Validación de Negocio
                var existe = await _unitOfWork.Autores.Exists(id);
                if (!existe)
                {
                    await _unitOfWork.RollbackAsync();
                    _logger.LogWarning("Consulta fallida: El autor con ID {AutorId} no existe.", id);
                    return Result.Failure(ErrorType.NotFound, "El autor que intenta eliminar no existe.");
                }

                await _unitOfWork.Autores.DeleteAsync(id);
                await _unitOfWork.CommitAsync();

                return Result.Success();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError(ex, "Eliminacion fallida: Error crítico al eliminar el autor con ID {AutorId}", id);
                return Result.Failure(ErrorType.ServerFault, "Error interno al procesar la eliminación del autor y sus libros.");
            }
        }
    }
}