using Application.Common;
using Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace Api.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ha ocurrido un error no controlado capturado en el middelware");
                context.Response.ContentType = "application/json";

                var response = new ErrorResponse { Message = ex.Message };

                switch (ex)
                {
                    // Errores controlados (Límite de libros, autor no registrado)
                    case BusinessException:
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        response.StatusCode = 400;
                        break;

                    // Errores inesperados (Base de datos caída, etc)
                    default:
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        response.StatusCode = 500;
                        response.Message = "Ocurrió un error interno en el servidor.";
                        if (_env.IsDevelopment()) response.Details = ex.StackTrace;
                        break;
                }

                var json = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(json);
            }
        }
    }
}
