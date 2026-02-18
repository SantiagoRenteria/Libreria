using Domain.Common;
using Microsoft.AspNetCore.Mvc;
using Application.Common;

namespace Api.Controllers
{
    public class BaseApiController : ControllerBase
    {
        // Este método maneja resultados que devuelven DATOS (como una lista de autores)
        protected IActionResult HandleResult<T>(Result<T> result)
        {
            if (result == null) return NotFound();

            if (result.IsSuccess)
            {
                if (result.Value == null) return NoContent();
                return Ok(result.Value);
            }

            return MapErrorResult(result.Error);
        }

        // Este método maneja resultados de COMANDOS (como Registrar o Eliminar)
        protected IActionResult HandleResult(Result result)
        {
            if (result.IsSuccess) return Ok();
            return MapErrorResult(result.Error);
        }

        // El "Traductor": Convierte el Error de Dominio en una respuesta HTTP
        private IActionResult MapErrorResult(Error error)
        {
            var response = new ErrorResponse
            {
                Message = error.Message,
                StatusCode = GetStatusCode(error.Type)
            };

            return StatusCode(response.StatusCode, response);
        }

        private int GetStatusCode(ErrorType type) => type switch
        {
            ErrorType.NotFound => 404,
            ErrorType.Validation => 400,
            ErrorType.Conflict => 409,
            ErrorType.ServerFault => 500,
            _ => 500
        };
    }
}