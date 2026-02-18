using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class AutorDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre completo es obligatorio")]
        public string NombreCompleto { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria")]
        public DateTime FechaNacimiento { get; set; }

        [Required(ErrorMessage = "La ciudad de procedencia es obligatoria")]
        public string CiudadProcedencia { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido")]
        public string CorreoElectronico { get; set; } = string.Empty;
    }
}
