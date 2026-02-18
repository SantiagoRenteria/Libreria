namespace Domain.Entities
{
    public class Autor
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; } = string.Empty; // Obligatorio
        public DateTime FechaNacimiento { get; set; }
        public string CiudadProcedencia { get; set; } = string.Empty;
        public string CorreoElectronico { get; set; } = string.Empty;

        // Propiedad de navegación para integridad 
        public virtual ICollection<Libro> Libros { get; set; } = new List<Libro>();
    }
}
