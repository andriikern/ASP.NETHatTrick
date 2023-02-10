using System;

namespace HatTrick.API.Features
{
    public sealed class HttpRequestUtcTimeFeature : IHttpRequestTimeFeature
    {
        public DateTime RequestTime { get; init; }

        public HttpRequestUtcTimeFeature()
        {
            RequestTime = DateTime.UtcNow;
        }
    }
}
