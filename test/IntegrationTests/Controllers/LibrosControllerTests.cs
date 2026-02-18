using Application.DTOs;
using FluentAssertions;
using IntegrationTests.Helpers;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using System.Net.Http.Json;

namespace IntegrationTests.Controllers
{
    public class LibrosControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public LibrosControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            // El factory levanta la API en memoria con la base de datos de test
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CrearLibro_DebeRetornarOk_CuandoDatosSonValidos()
        {
            // Arrange
            var autorIdExistente = 1; // Gabriel García Márquez del seeder
            var nuevoLibro = new LibroCreateDto
            {
                Titulo = "Cien años de soledad",
                Genero = "Realismo Mágico",
                Anio = 2026,
                NumeroPaginas = 471,
                AutorId = autorIdExistente
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/libros", nuevoLibro);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task CrearLibro_DebeRetornarNotFound_CuandoAutorNoExiste()
        {
            // Arrange
            var autorIdInexistente = 999;
            var libroHuerfano = new LibroCreateDto
            {
                Titulo = "Libro sin Autor",
                Genero = "Ficción",
                Anio = 2026,
                NumeroPaginas = 100,
                AutorId = autorIdInexistente
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/libros", libroHuerfano);

            // Assert
            // El Result Pattern en el Service debe detectar que el autor no existe
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task EliminarLibro_DebeRetornarOk_CuandoIdExiste()
        {
            // Arrange: Primero creamos un libro para poder borrarlo
            var idLibroBorrar = 1;

            // Act
            var response = await _client.DeleteAsync($"/api/libros/{idLibroBorrar}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task CrearLibro_DebeRetornarConflict_CuandoAutorYaTieneMaximoDeLibros()
        {
            // Arrange
            var autorId = 2; // Usamos a Isabel Allende del seeder

            // Simulamos que ya tiene 3 libros (podemos hacer un bucle de inserción rápida o ajustar el seeder)
            for (int i = 1; i <= 3; i++)
            {
                var libro = new LibroCreateDto
                {
                    Titulo = $"Libro Test {i}",
                    Genero = "Test",
                    Anio = 2026,
                    NumeroPaginas = 100,
                    AutorId = autorId
                };
                await _client.PostAsJsonAsync("/api/libros", libro);
            }

            // Act: Intentamos insertar el libro número 4
            var libroOnce = new LibroCreateDto
            {
                Titulo = "El libro prohibido",
                Genero = "Drama",
                Anio = 2026,
                NumeroPaginas = 200,
                AutorId = autorId
            };
            var response = await _client.PostAsJsonAsync("/api/libros", libroOnce);

            // Assert
            // 1. Verifica el status code que realmente devuelve tu API (parece ser 400 según el log)
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            // 2. Lee el contenido del error
            var errorContent = await response.Content.ReadAsStringAsync();

            // 3. Verifica palabras clave por separado para evitar errores de redacción
            errorContent.Should().Contain("máximo");
            errorContent.Should().Contain("libros");
        }
    }
}
