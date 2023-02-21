using System;

namespace HatTrick.API.Features
{
    public sealed class HttpRequestUtcTimeFeature : HttpRequestTimeBaseFeature
    {
        public HttpRequestUtcTimeFeature() :
            base(DateTime.UtcNow)
        {
        }
    }
}
