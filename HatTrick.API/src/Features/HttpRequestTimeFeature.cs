using System;

namespace HatTrick.API.Features
{
    public sealed class HttpRequestTimeFeature : IHttpRequestTimeFeature
    {
        public DateTime RequestTime { get; init; }

        public HttpRequestTimeFeature()
        {
            RequestTime = DateTime.Now;
        }
    }
}
