using HatTrick.API.Features;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace HatTrick.API.Middlewares
{
    public sealed class RequestTimeMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestTimeMiddleware(
            RequestDelegate next
        )
        {
            _next = next ??
                throw new ArgumentNullException(nameof(next));
        }

        public Task InvokeAsync(
            HttpContext context
        )
        {
            context.Features.Set<IHttpRequestTimeFeature>(
                new HttpRequestTimeFeature()
            );

            return _next(context);
        }
    }
}
