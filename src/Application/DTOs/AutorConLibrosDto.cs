using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class AutorConLibrosDto
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public List<LibroDto> Libros { get; set; } = new();
    }
}
