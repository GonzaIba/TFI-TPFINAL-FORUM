using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiForums.Middleware.Helpers
{
    public static class RequestAnalize
    {
        internal static bool IsControllerPath(string path)
        {
            const string controllerPattern = "/api/v";

            // Verificar si la ruta sigue el patrón "/api/v{versión}"
            return path.StartsWith(controllerPattern, StringComparison.OrdinalIgnoreCase) &&
                   path.Length > controllerPattern.Length &&
                   char.IsDigit(path[controllerPattern.Length]);
        }

        internal static bool IsHangfirePath(string path)
        {
            const string hangfirePattern = "/hangfire";

            // Verificar si la ruta sigue el patrón "/hangfire"
            return (path == hangfirePattern);
        }

        internal static bool ValidateAuthorizationEndpoint(HttpContext context)
        {
            // Aquí implementa la lógica para validar el token JWT
            // Puedes utilizar la biblioteca System.IdentityModel.Tokens.Jwt

            var endpoint = context.GetEndpoint();

            if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
            {
                return true;
            }

            var token = ObtainTokenFromRequest(context.Request);

            if (string.IsNullOrEmpty(token))
            {
                // Token no presente, manejar según tus necesidades
                return false;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("6c770bda-6c89-4667-9cc8-33ef7407c2a6")), // Reemplaza con tu clave secreta
                ValidateIssuer = false, // Puedes configurar esto según tu entorno
                ValidateAudience = false, // Puedes configurar esto según tu entorno
                ClockSkew = TimeSpan.Zero // No permitir desviación de tiempo
            };

            try
            {
                ClaimsPrincipal claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out _);
                context.User = claimsPrincipal;
                return true;
            }
            catch (SecurityTokenValidationException)
            {
                // Token no válido
                return false;
            }
        }

        internal static string ObtainTokenFromRequest(HttpRequest request)
        {
            // Implementa lógica para obtener el token de la solicitud, por ejemplo, desde los encabezados
            return request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        }

        internal static string GetHttpStatusMessage(int statusCode)
        {
            return statusCode switch
            {
                StatusCodes.Status200OK => "OK",
                StatusCodes.Status204NoContent => "No Content",
                StatusCodes.Status400BadRequest => "Bad Request",
                StatusCodes.Status401Unauthorized => "No Authorized",
                StatusCodes.Status404NotFound => "Not Found",
                // Agregar más según sea necesario
                _ => "No Handle Exception",
            };
        }
    }
}
