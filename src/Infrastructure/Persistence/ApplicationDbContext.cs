using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Libro> Libros { get; set; }
        public DbSet<Autor> Autores { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuración de validaciones (Asteriscos en el documento)
            modelBuilder.Entity<Autor>(entity =>
            {
                entity.Property(e => e.NombreCompleto).IsRequired().HasMaxLength(150); // Obligatorio *
            });

            modelBuilder.Entity<Libro>(entity =>
            {
                entity.Property(e => e.Titulo).IsRequired().HasMaxLength(150); // Obligatorio *
                // Relación 1 a muchos: Un autor tiene muchos libros
                entity.HasOne(d => d.Autor)
                      .WithMany(p => p.Libros)
                      .HasForeignKey(d => d.AutorId);
            });
        }
    }
}
