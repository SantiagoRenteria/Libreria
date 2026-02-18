namespace Application.DTOs
{
    public class LibroDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public int Anio { get; set; }
        public string Genero { get; set; } = string.Empty;
    }
}
