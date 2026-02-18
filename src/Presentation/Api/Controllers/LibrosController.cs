using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/libros")]
    public class LibrosController : BaseApiController
    {
        private readonly ILibroService _libroService;

        public LibrosController(ILibroService libroService)
        {
            _libroService = libroService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] LibroCreateDto libroDto)
        {
            var result = await _libroService.RegistrarLibroAsync(libroDto);
            return HandleResult(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _libroService.EliminarLibroAsync(id);
            return HandleResult(result);
        }
    }
}