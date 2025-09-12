using ContentCreator.Application.Interfaces;
using System.Text.Json;

namespace ContentCreator.Api.Middlewares
{
    public class JwtTokenValidationMiddleware
    {
        private readonly RequestDelegate _next;
        public JwtTokenValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
        {
            // Resolve the scoped dependency at runtime

            using (var scope = serviceProvider.CreateScope())
            {
                var tokenRevocationConfig = scope.ServiceProvider.GetRequiredService<ITokenRevocationConfig>();

                var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                if (!string.IsNullOrEmpty(token))
                {
                    bool isTokenRevoked = await tokenRevocationConfig.IsTokenRevoked(token);

                    if (isTokenRevoked)
                    {
                        await HandleUnauthorized(context, "Token is expired.");

                        return;
                    }
                }
            }
            await _next(context);
        }

        private async Task HandleUnauthorized(HttpContext context, string message)
        {
            context.Response.StatusCode = 401;

            context.Response.ContentType = "application/json";

            var responseData = new { StatusCode = 401, Message = message };

            await context.Response.WriteAsync(JsonSerializer.Serialize(responseData));
        }
    }
    public static class JwtTokenValidationMiddlewareExtensions
    {
        public static IApplicationBuilder UseJwtTokenValidation(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<JwtTokenValidationMiddleware>();
        }
    }
}