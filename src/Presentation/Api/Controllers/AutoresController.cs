using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/autores")]
    public class AutoresController : BaseApiController
    {
        private readonly IAutorService _autorService;

        public AutoresController(IAutorService autorService)
        {
            _autorService = autorService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _autorService.ObtenerTodosAsync();
            return HandleResult(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _autorService.ObtenerAutorAsync(id);
            return HandleResult(result);
        }

        [HttpGet("con-libros")]
        public async Task<IActionResult> GetAutoresLibros(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _autorService.ObtenerAutoresConLibrosPaginadoAsync(page, pageSize);
            return HandleResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AutorDto autorDto)
        {
            var result = await _autorService.RegistrarAutorAsync(autorDto);
            return HandleResult(result);
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] AutorDto autorDto)
        {
            var result = await _autorService.ActualizarAutorAsync(autorDto);
            return HandleResult(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _autorService.EliminarAutor(id);
            return HandleResult(result);
        }
    }
}