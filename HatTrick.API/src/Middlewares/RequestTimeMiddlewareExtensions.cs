using HatTrick.API.Features;
using Microsoft.AspNetCore.Builder;

namespace HatTrick.API.Middlewares
{
    public static class RequestTimeMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestTime<THttpRequestTimeFeature>(
            this IApplicationBuilder builder
        )
            where
                THttpRequestTimeFeature : IHttpRequestTimeFeature, new()
            =>
            builder.UseMiddleware<RequestTimeMiddleware<THttpRequestTimeFeature>>();
    }
}
