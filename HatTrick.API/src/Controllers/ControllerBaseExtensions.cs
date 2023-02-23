using HatTrick.API.Features;
using Microsoft.AspNetCore.Mvc;
using System;

namespace HatTrick.API.Controllers
{
    /// <summary>Provides auxiliary extension methods for the <see cref="ControllerBase" /> class.</summary>
    internal static class ControllerBaseExtensions
    {
        public static DateTime GetDefaultTime(
            this ControllerBase controller,
            DateTime? defaultDateTime = null
        ) =>
            controller.HttpContext
                ?.Features.Get<IHttpRequestTimeFeature>()
                ?.RequestTime ??
                    defaultDateTime.GetValueOrDefault(DateTime.Now);
    }
}
