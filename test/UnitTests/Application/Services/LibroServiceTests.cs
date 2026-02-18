using Application.DTOs;
using Application.Services;
using Domain.Common;
using Domain.Entities;
using Domain.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests.Application.Services
{
    public class LibroServiceTests
    {
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<ILibroRepository> _libroRepoMock;
        private readonly Mock<ILogger<LibroService>> _loggerMock;
        private readonly LibroService _service;

        public LibroServiceTests()
        {
            _uowMock = new Mock<IUnitOfWork>();
            _libroRepoMock = new Mock<ILibroRepository>();
            _loggerMock = new Mock<ILogger<LibroService>>();

            // Configurar el UoW para que devuelva el repositorio de libros
            _uowMock.Setup(u => u.Libros).Returns(_libroRepoMock.Object);

            _service = new LibroService(_uowMock.Object, _loggerMock.Object);
        }

        #region Pruebas de Registro

        [Fact]
        public async Task RegistrarLibroAsync_DebeRetornarNotFound_CuandoAutorNoExiste()
        {
            // Arrange
            var dto = new LibroCreateDto { AutorId = 99, Titulo = "Test" };
            _libroRepoMock.Setup(r => r.ExisteAutorAsync(dto.AutorId)).ReturnsAsync(false);

            // Act
            var result = await _service.RegistrarLibroAsync(dto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Type.Should().Be(ErrorType.NotFound);
            result.Error.Message.Should().Contain("autor no está registrado");
        }

        [Fact]
        public async Task RegistrarLibroAsync_DebeRetornarValidation_CuandoExcedeLimiteDeLibros()
        {
            // Arrange
            var dto = new LibroCreateDto { AutorId = 1, Titulo = "El cuarto libro" };
            _libroRepoMock.Setup(r => r.ExisteAutorAsync(dto.AutorId)).ReturnsAsync(true);

            // Simulamos que ya tiene 3 libros
            _libroRepoMock.Setup(r => r.CountLibrosAsync(dto.AutorId)).ReturnsAsync(3);

            // Act
            var result = await _service.RegistrarLibroAsync(dto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Type.Should().Be(ErrorType.Validation);
            result.Error.Message.Should().Contain("máximo permitido");
        }

        [Fact]
        public async Task RegistrarLibroAsync_DebeRetornarSuccess_CuandoDatosSonValidos()
        {
            // Arrange
            var dto = new LibroCreateDto { AutorId = 1, Titulo = "Libro Válido" };
            _libroRepoMock.Setup(r => r.ExisteAutorAsync(dto.AutorId)).ReturnsAsync(true);
            _libroRepoMock.Setup(r => r.CountLibrosAsync(dto.AutorId)).ReturnsAsync(0);

            // Act
            var result = await _service.RegistrarLibroAsync(dto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _libroRepoMock.Verify(r => r.AddAsync(It.IsAny<Libro>()), Times.Once);
            _uowMock.Verify(u => u.SaveAsync(), Times.Once);
        }

        #endregion

        #region Pruebas de Eliminación

        [Fact]
        public async Task EliminarLibroAsync_DebeLlamarRollback_CuandoLibroNoExiste()
        {
            // Arrange
            int idInexistente = 500;
            _libroRepoMock.Setup(r => r.Exists(idInexistente)).ReturnsAsync(false);

            // Act
            var result = await _service.EliminarLibroAsync(idInexistente);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Type.Should().Be(ErrorType.NotFound);

            // Verificación crucial de transacciones
            _uowMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
            _uowMock.Verify(u => u.RollbackAsync(), Times.Once);
            _uowMock.Verify(u => u.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task EliminarLibroAsync_DebeRetornarServerFault_CuandoOcurreExcepcion()
        {
            // Arrange
            int id = 1;
            _libroRepoMock.Setup(r => r.Exists(id)).ReturnsAsync(true);
            _libroRepoMock.Setup(r => r.DeleteAsync(id)).ThrowsAsync(new Exception("DB Crash"));

            // Act
            var result = await _service.EliminarLibroAsync(id);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Type.Should().Be(ErrorType.ServerFault);
            _uowMock.Verify(u => u.RollbackAsync(), Times.Once);
        }

        #endregion
    }
}