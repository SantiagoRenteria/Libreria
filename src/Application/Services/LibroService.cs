using Application.DTOs;
using Application.Interfaces;
using Domain.Common;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class LibroService : ILibroService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<LibroService> _logger;
        private const int MAX_PERMITIDO = 3;

        public LibroService(IUnitOfWork unitOfWork, ILogger<LibroService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<bool>> RegistrarLibroAsync(LibroCreateDto libroDto)
        {
            try
            {
                // 1. Validación de existencia del Autor (Error de Cliente - 404/400)
                var existeAutor = await _unitOfWork.Libros.ExisteAutorAsync(libroDto.AutorId);
                if (!existeAutor)
                {
                    _logger.LogWarning("Intento de registrar libro con AutorId {AutorId} que no existe.", libroDto.AutorId);
                    return Result<bool>.Failure(ErrorType.NotFound, "El autor no está registrado.");
                }
                    
                // 2. Validación de regla de negocio (Error de Validación - 400)
                var totalLibros = await _unitOfWork.Libros.CountLibrosAsync(libroDto.AutorId);
                if (totalLibros >= MAX_PERMITIDO)
                {
                    _logger.LogWarning("Intento de registrar libro para AutorId {AutorId} que ya tiene {TotalLibros} libros registrados.", libroDto.AutorId, totalLibros);
                    return Result<bool>.Failure(ErrorType.Validation, $"Se ha alcanzado el máximo permitido de {MAX_PERMITIDO} libros.");
                }
                    
                var nuevoLibro = new Libro
                {
                    Titulo = libroDto.Titulo,
                    Anio = libroDto.Anio,
                    Genero = libroDto.Genero,
                    NumeroPaginas = libroDto.NumeroPaginas,
                    AutorId = libroDto.AutorId
                };

                await _unitOfWork.Libros.AddAsync(nuevoLibro);
                await _unitOfWork.SaveAsync();

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al registrar el libro con título {Titulo}.", libroDto.Titulo);
                return Result<bool>.Failure(ErrorType.ServerFault, "Ocurrió un error inesperado al registrar el libro.");
            }
        }

        public async Task<Result<bool>> EliminarLibroAsync(int id)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // 1. Validación de existencia (Error de Cliente - 404)
                var existe = await _unitOfWork.Libros.Exists(id);
                if (!existe)
                {
                    await _unitOfWork.RollbackAsync();
                    _logger.LogWarning("Intento de eliminar libro con Id {Id} que no existe.", id);
                    return Result<bool>.Failure(ErrorType.NotFound, "El libro que intenta eliminar no existe.");
                }

                await _unitOfWork.Libros.DeleteAsync(id);
                await _unitOfWork.CommitAsync();

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError(ex, "Error inesperado al eliminar el libro con Id {Id}.", id);
                return Result<bool>.Failure(ErrorType.ServerFault, "Error interno del sistema al procesar la eliminación.");
            }
        }
    }
}