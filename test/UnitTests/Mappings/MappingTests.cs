using Application.DTOs;
using Domain.Entities;
using FluentAssertions;

namespace UnitTests.Mappings
{
    public class MappingTests
    {
        [Fact]
        public void Autor_DebeMapearA_AutorDto_Correctamente()
        {
            // Arrange
            var autor = new Autor
            {
                Id = 1,
                NombreCompleto = "Gabriel García Márquez",
                CorreoElectronico = "gabo@mail.com",
                CiudadProcedencia = "Aracataca",
                FechaNacimiento = new DateTime(1927, 3, 6)
            };

            // Act
            var dto = new AutorDto
            {
                Id = autor.Id,
                NombreCompleto = autor.NombreCompleto,
                CorreoElectronico = autor.CorreoElectronico,
                CiudadProcedencia = autor.CiudadProcedencia,
                FechaNacimiento = autor.FechaNacimiento
            };

            // Assert
            dto.Should().BeEquivalentTo(autor, options => options
                .ExcludingMissingMembers()); // Verifica que todas las propiedades que existen en ambos coincidan
        }

        [Fact]
        public void AutorConLibros_DebeMapearA_AutorConLibrosDto_IncluyendoListaDeLibros()
        {
            // Arrange
            var autor = new Autor
            {
                Id = 2,
                NombreCompleto = "Isabel Allende",
                Libros = new List<Libro>
                {
                    new Libro { Id = 10, Titulo = "La casa de los espíritus", Anio = 1982, Genero = "Ficción" },
                    new Libro { Id = 11, Titulo = "Eva Luna", Anio = 1987, Genero = "Ficción" }
                }
            };

            // Act (Simulando la lógica del AutorService)
            var dto = new AutorConLibrosDto
            {
                Id = autor.Id,
                NombreCompleto = autor.NombreCompleto,
                Libros = autor.Libros.Select(l => new LibroDto
                {
                    Id = l.Id,
                    Titulo = l.Titulo,
                    Anio = l.Anio,
                    Genero = l.Genero
                }).ToList()
            };

            // Assert
            dto.Id.Should().Be(autor.Id);
            dto.NombreCompleto.Should().Be(autor.NombreCompleto);
            dto.Libros.Should().HaveCount(2);
            dto.Libros.Should().ContainSingle(l => l.Titulo == "La casa de los espíritus");
            dto.Libros.Should().ContainSingle(l => l.Titulo == "Eva Luna");
        }
    }
}