using HatTrick.API.Features;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace HatTrick.API.Middlewares
{
    public sealed class RequestUtcTimeMiddleware
    {
        private readonly RequestDelegate? _next;

        public RequestUtcTimeMiddleware(
            RequestDelegate? next
        )
        {
            _next = next;
        }

        public RequestUtcTimeMiddleware() :
            this(null)
        {
        }

        public Task InvokeAsync(
            HttpContext context
        )
        {
            context.Features.Set<IHttpRequestTimeFeature>(
                new HttpRequestUtcTimeFeature()
            );

            return _next?.Invoke(context) ?? Task.CompletedTask;
        }
    }
}
