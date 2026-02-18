using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class LibroCreateDto
    {
        [Required(ErrorMessage = "El título es obligatorio")]
        public string Titulo { get; set; } = string.Empty;

        [Required]
        public int Anio { get; set; }

        public string Genero { get; set; } = string.Empty;

        public int NumeroPaginas { get; set; }

        [Required(ErrorMessage = "Debe asignar un autor")]
        public int AutorId { get; set; }
    }
}
