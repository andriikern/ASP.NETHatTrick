using Microsoft.AspNetCore.Builder;

namespace HatTrick.API.Middlewares
{
    public static class RequestTimeMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestTime(
            this IApplicationBuilder builder
        )
        {
            return builder.UseMiddleware<RequestTimeMiddleware>();
        }
    }
}
