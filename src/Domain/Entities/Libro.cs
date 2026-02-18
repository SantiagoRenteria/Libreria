namespace Domain.Entities
{
    public class Libro
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty; // Obligatorio
        public int Anio { get; set; }
        public string Genero { get; set; } = string.Empty;
        public int NumeroPaginas { get; set; }

        public int AutorId { get; set; }
        public virtual Autor? Autor { get; set; }
    }
}
