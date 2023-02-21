using System;

namespace HatTrick.API.Features
{
    public sealed class HttpRequestTimeFeature : HttpRequestTimeBaseFeature
    {
        public HttpRequestTimeFeature() :
            base(DateTime.Now)
        {
        }
    }
}
