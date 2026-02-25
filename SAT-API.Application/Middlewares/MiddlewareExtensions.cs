using Microsoft.AspNetCore.Builder;

namespace SAT_API.Application.Middlewares
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder ExceptionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionMiddleware>();
        }

        public static IApplicationBuilder ExceptionEnvironmentMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionEnvironmentMiddleware>();
        }

        public static IApplicationBuilder ValidationMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ValidationMiddleware>();
        }

        public static IApplicationBuilder CultureMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CultureMiddleware>();
        }
    }
}

