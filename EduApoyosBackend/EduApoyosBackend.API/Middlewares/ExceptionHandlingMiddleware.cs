using System.Net;
using System.Text.Json;

namespace EduApoyosBackend.API.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); // Continúa con el flujo normal de la petición HTTP
            }
            catch (Exception ex)
            {
                // Registramos el error completo en los logs de la aplicación con su traza
                _logger.LogError(ex, "Ocurrió un error no controlado durante la petición: {Message}", ex.Message);

                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var statusCode = HttpStatusCode.InternalServerError; // Por defecto 500
            var message = "Ocurrió un error interno en el servidor.";

            // Mapeamos excepciones conocidas a códigos de estado HTTP específicos
            switch (exception)
            {
                case InvalidOperationException: // Ej: Correo ya registrado
                    statusCode = HttpStatusCode.BadRequest;
                    message = exception.Message;
                    break;

                case UnauthorizedAccessException: // Ej: Credenciales incorrectas en el login
                    statusCode = HttpStatusCode.Unauthorized;
                    message = exception.Message;
                    break;

                case KeyNotFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    message = exception.Message;
                    break;
            }

            context.Response.StatusCode = (int)statusCode;

            var response = new
            {
                status = (int)statusCode,
                mensaje = message
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
