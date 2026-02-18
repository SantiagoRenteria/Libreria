using Domain.Entities;
using Infrastructure.Persistence;

namespace IntegrationTests.Helpers
{
    public static class Utilities
    {
        public static void InitializeDbForTests(ApplicationDbContext db)
        {
            // Limpiamos por si acaso
            db.Autores.RemoveRange(db.Autores);
            db.Libros.RemoveRange(db.Libros);

            // 1. Agregamos Autores
            var gabo = new Autor { Id = 1, NombreCompleto = "Gabriel García Márquez", CiudadProcedencia = "Aracataca", CorreoElectronico = "gabo@mail.com", FechaNacimiento = new DateTime(1927, 3, 6) };
            var isabel = new Autor { Id = 2, NombreCompleto = "Isabel Allende", CiudadProcedencia = "Lima", CorreoElectronico = "isabel@mail.com", FechaNacimiento = new DateTime(1942, 8, 2) };

            db.Autores.AddRange(gabo, isabel);

            // 2. Agregamos Libros asociados a esos Autores
            db.Libros.AddRange(
                new Libro { Id = 1, Titulo = "Cien años de soledad", Genero = "Realismo Mágico", Anio = 1967, NumeroPaginas = 471, AutorId = 1 },
                new Libro { Id = 2, Titulo = "El amor en los tiempos del cólera", Genero = "Romance", Anio = 1985, NumeroPaginas = 368, AutorId = 1 },
                new Libro { Id = 3, Titulo = "La casa de los espíritus", Genero = "Ficción", Anio = 1982, NumeroPaginas = 432, AutorId = 2 }
            );

            db.SaveChanges();
        }
    }
}