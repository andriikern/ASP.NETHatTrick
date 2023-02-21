using Microsoft.AspNetCore.Builder;

namespace HatTrick.API.Middlewares
{
    public static class RequestUtcTimeMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestUtcTime(
            this IApplicationBuilder builder
        )
        {
            return builder.UseMiddleware<RequestUtcTimeMiddleware>();
        }
    }
}
