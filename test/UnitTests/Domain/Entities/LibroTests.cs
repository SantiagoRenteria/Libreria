using Domain.Entities;
using FluentAssertions;

namespace UnitTests.Domain.Entities
{
    public class LibroTests
    {
        [Fact]
        public void CrearLibro_DebeAsignarPropiedadesCorrectamente()
        {
            // Arrange
            var titulo = "El coronel no tiene quien le escriba";
            var anio = 1961;
            var paginas = 92;
            var autorId = 1;

            // Act
            var libro = new Libro
            {
                Id = 1,
                Titulo = titulo,
                Anio = anio,
                NumeroPaginas = paginas,
                AutorId = autorId,
                Genero = "Novela"
            };

            // Assert
            libro.Titulo.Should().Be(titulo);
            libro.Anio.Should().Be(anio);
            libro.NumeroPaginas.Should().Be(paginas);
            libro.AutorId.Should().Be(autorId);
        }

        [Fact]
        public void Libro_DebePoderTenerAutorNulo_AntesDeSerCargado()
        {
            // Esta prueba valida que la propiedad de navegación sea opcional 
            // en el objeto, lo cual es vital para evitar errores de referencia nula.

            // Arrange & Act
            var libro = new Libro { Titulo = "Relato de un náufrago" };

            // Assert
            libro.Autor.Should().BeNull();
        }
    }
}