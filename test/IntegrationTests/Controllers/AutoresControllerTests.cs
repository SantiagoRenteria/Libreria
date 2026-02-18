using Application.DTOs;
using FluentAssertions;
using IntegrationTests.Helpers;
using System.Net;
using System.Net.Http.Json;

namespace IntegrationTests.Controllers
{
    public class AutoresControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public AutoresControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            // El factory levanta la API en memoria con la base de datos de test
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetById_DebeRetornarNotFound_CuandoAutorNoExiste()
        {
            // Arrange: Un ID que sabemos que no está en la BD en memoria vacía
            var idInexistente = 999;

            // Act: Llamada real al endpoint del API
            var response = await _client.GetAsync($"/api/autores/{idInexistente}");

            // Assert: Verificamos que el BaseApiController y el Result Pattern funcionen
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetById_DebeRetornarOk_CuandoAutorExiste()
        {
            // Arrange
            var autoresResponse = await _client.GetAsync("/api/autores");
            var autores = await autoresResponse.Content.ReadFromJsonAsync<List<AutorDto>>();
            // Usa el ID del primero que encuentres
            var idReal = autores.First().Id;

            // Act
            var response = await _client.GetAsync($"/api/autores/{idReal}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }


        [Fact]
        public async Task CrearAutor_DebeRetornarOk_CuandoDatosSonValidos()
        {
            // Arrange
            var nuevoAutor = new AutorDto
            {
                NombreCompleto = "Test Autor",
                CorreoElectronico = "test@biblioteca.com",
                FechaNacimiento = DateTime.Now.AddYears(-30),
                CiudadProcedencia = "Medellín"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/autores", nuevoAutor);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task ActualizarAutor_DebeRetornarOk_CuandoDatosSonValidos()
        {
            // Arrange
            var idExistente = 2; // Isabel Allende según nuestro Utilities
            var autorActualizado = new AutorDto
            {
                Id = idExistente,
                NombreCompleto = "Isabel Allende Actualizada",
                CorreoElectronico = "isabel_new@mail.com",
                FechaNacimiento = new DateTime(1942, 8, 2),
                CiudadProcedencia = "Chile"
            };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/autores", autorActualizado);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Verificación adicional: Consultar si el cambio se aplicó
            var getResponse = await _client.GetAsync($"/api/autores/{idExistente}");
            var autorEnDb = await getResponse.Content.ReadFromJsonAsync<AutorDto>();
            autorEnDb.NombreCompleto.Should().Be("Isabel Allende Actualizada");
        }

        [Fact]
        public async Task EliminarAutor_DebeRetornarOk_CuandoIdExiste()
        {
            // Arrange
            var idABorrar = 1; // Gabriel García Márquez

            // Act
            var response = await _client.DeleteAsync($"/api/autores/{idABorrar}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Verificación: Intentar obtener el autor borrado
            var getResponse = await _client.GetAsync($"/api/autores/{idABorrar}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task EliminarAutor_DebeRetornarNotFound_CuandoIdNoExiste()
        {
            // Arrange
            var idInexistente = 999;

            // Act
            var response = await _client.DeleteAsync($"/api/autores/{idInexistente}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task CrearAutor_DebeRetornarBadRequest_CuandoDatosSonInvalidos()
        {
            // Arrange: Intentamos crear un autor sin nombre (suponiendo que es obligatorio)
            // o con datos que violan las reglas de validación.
            var autorInvalido = new AutorDto
            {
                NombreCompleto = "", // Nombre vacío
                CorreoElectronico = "correo-no-valido", // Formato de email incorrecto
                FechaNacimiento = DateTime.Now.AddDays(1), // Fecha en el futuro
                CiudadProcedencia = "Medellín"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/autores", autorInvalido);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

    }
}