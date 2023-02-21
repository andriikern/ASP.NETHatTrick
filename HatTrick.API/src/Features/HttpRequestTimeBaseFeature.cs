using System;

namespace HatTrick.API.Features
{
    public class HttpRequestTimeBaseFeature : IHttpRequestTimeFeature
    {
        private readonly DateTime _requestTime;

        public DateTime RequestTime =>
            _requestTime;

        private protected HttpRequestTimeBaseFeature(
            DateTime requestTime
        )
        {
            _requestTime = requestTime;
        }

        private HttpRequestTimeBaseFeature() :
            this(DateTime.UnixEpoch)
        {
        }
    }
}
