using System;

namespace HatTrick.API.Features
{
    public interface IHttpRequestTimeFeature
    {
        DateTime RequestTime { get; }
    }
}
