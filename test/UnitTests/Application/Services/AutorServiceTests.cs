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
    public class AutorServiceTests
    {
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<IAutorRepository> _autorRepoMock;
        private readonly Mock<ILogger<AutorService>> _loggerMock;
        private readonly AutorService _service;

        public AutorServiceTests()
        {
            _uowMock = new Mock<IUnitOfWork>();
            _autorRepoMock = new Mock<IAutorRepository>();
            _loggerMock = new Mock<ILogger<AutorService>>();

            // Configuramos el UoW para que al pedir .Autores devuelva nuestro mock del repo
            _uowMock.Setup(u => u.Autores).Returns(_autorRepoMock.Object);

            // Pasamos el UoW mockeado al servicio
            _service = new AutorService(_uowMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetById_DebeRetornarNotFound_CuandoAutorNoExiste()
        {
            // Arrange: Configuramos el REPOSITORIO para que devuelva null
             _autorRepoMock.Setup(r => r.GetById(It.IsAny<int>()))
                          .ReturnsAsync((Autor)null);

            // Act: Llamamos al método (asegúrate de que el nombre coincida en tu AutorService)
            var result = await _service.ObtenerAutorAsync(999);

            // Assert
            result.IsSuccess.Should().BeFalse();

            result.Error.Type.Should().Be(ErrorType.NotFound);

            result.Error.Message.Should().Be("El autor solicitado no existe o ha sido eliminado.");
        }

        [Fact]
        public async Task ObtenerAutoresConLibrosPaginadoAsync_DebeRetornarExitoConDatos()
        {
            // Arrange
            var autores = new List<Autor>
            {
                new Autor {
                    Id = 1,
                    NombreCompleto = "Test",
                    Libros = new List<Libro> { new Libro { Id = 1, Titulo = "Libro 1" } }
                }
            };

            // Mockea la tupla (List<Autor>, int total)
            _autorRepoMock.Setup(r => r.GetPaginatedsAsync(1, 10))
                          .ReturnsAsync((autores, 1));

            // Act
            var result = await _service.ObtenerAutoresConLibrosPaginadoAsync(1, 10);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Items.Should().HaveCount(1);
            result.Value.Items.First().Libros.Should().HaveCount(1);
            result.Value.TotalItems.Should().Be(1);
        }

        [Fact]
        public async Task RegistrarAutorAsync_DebeRetornarServerFault_CuandoElRepositorioFalla()
        {
            // Arrange
            var autorDto = new AutorDto { NombreCompleto = "Error Test", CorreoElectronico = "test@mail.com" };

            // Forzamos al repositorio a lanzar una excepción
            _autorRepoMock.Setup(r => r.AddAsync(It.IsAny<Autor>()))
                          .ThrowsAsync(new Exception("Error de conexión a la base de datos"));

            // Act
            var result = await _service.RegistrarAutorAsync(autorDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Type.Should().Be(ErrorType.ServerFault);
            result.Error.Message.Should().Contain("No se pudo registrar");
        }

        [Fact]
        public async Task EliminarAutor_DebeLlamarRollback_CuandoAutorNoExiste()
        {
            // Arrange
            int idInexistente = 99;
            _autorRepoMock.Setup(r => r.Exists(idInexistente)).ReturnsAsync(false);

            // Act
            await _service.EliminarAutor(idInexistente);

            // Assert
            // Verificamos que se llamó al Rollback y NUNCA al Commit
            _uowMock.Verify(u => u.RollbackAsync(), Times.Once);
            _uowMock.Verify(u => u.CommitAsync(), Times.Never);
        }

    }
}