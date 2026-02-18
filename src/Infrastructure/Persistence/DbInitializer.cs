using Domain.Entities;

namespace Infrastructure.Persistence
{
    public static class DbInitializer
    {
        public static void Seed(ApplicationDbContext context)
        {
            context.Database.EnsureCreated(); // Crea la DB si no existe

            if (context.Autores.Any()) return; // Si ya hay datos, no hace nada

            var autores = new List<Autor>
            {
                new Autor {
                    NombreCompleto = "Gabriel García Márquez",
                    FechaNacimiento = new DateTime(1927, 3, 6),
                    CiudadProcedencia = "Aracataca",
                    CorreoElectronico = "gabo@ejemplo.com"
                },
                new Autor {
                    NombreCompleto = "Miguel de Cervantes",
                    FechaNacimiento = new DateTime(1547, 9, 29),
                    CiudadProcedencia = "Alcalá de Henares",
                    CorreoElectronico = "miguel@ejemplo.com"
                }
            };

            context.Autores.AddRange(autores);
            context.SaveChanges();

            var libros = new List<Libro>
            {
                new Libro {
                    Titulo = "Cien años de soledad",
                    Anio = 1967,
                    Genero = "Realismo Mágico",
                    NumeroPaginas = 471,
                    AutorId = autores[0].Id
                },
                new Libro {
                    Titulo = "Don Quijote de la Mancha",
                    Anio = 1605,
                    Genero = "Novela",
                    NumeroPaginas = 1032,
                    AutorId = autores[1].Id
                }
            };

            context.Libros.AddRange(libros);
            context.SaveChanges();
        }
    }
}
