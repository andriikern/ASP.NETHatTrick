using HatTrick.API.Features;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace HatTrick.API.Middlewares
{
    public sealed class RequestTimeMiddleware<THttpRequestTimeFeature>
        where
            THttpRequestTimeFeature : IHttpRequestTimeFeature, new()
    {
        public static Type HttpRequestTimeFeatureType =>
            typeof(THttpRequestTimeFeature);

        private readonly RequestDelegate? _next;

        public RequestTimeMiddleware(
            RequestDelegate? next
        )
        {
            _next = next;
        }

        public RequestTimeMiddleware() :
            this(null)
        {
        }

        public Task InvokeAsync(
            HttpContext context
        )
        {
            context.Features.Set<IHttpRequestTimeFeature>(
                new THttpRequestTimeFeature()
            );

            return _next?.Invoke(context) ?? Task.CompletedTask;
        }
    }
}
