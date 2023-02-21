using HatTrick.API.Features;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace HatTrick.API.Middlewares
{
    public sealed class RequestTimeMiddleware
    {
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
                new HttpRequestTimeFeature()
            );

            return _next?.Invoke(context) ?? Task.CompletedTask;
        }
    }
}
