using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mime;
using System;
using System.Threading.Tasks;
using HatTrick.BLL.Exceptions;
using Microsoft.AspNetCore.Http;
using HatTrick.API.Features;

namespace HatTrick.API.Controllers
{
    /// <summary>Provides fields, properties and methods common to all application's controllers.</summary>
    [Produces(MediaTypeNames.Application.Json, "text/json", MediaTypeNames.Text.Plain), Route("null"), ApiController]
    public class InternalBaseController : ControllerBase, IDisposable, IAsyncDisposable
    {
        private static string GetAdequateContentType(
            string? requestContentType
        ) =>
            requestContentType is not null &&
                requestContentType.StartsWith(
                    "text/",
                    StringComparison.InvariantCultureIgnoreCase
                ) ?
                    MediaTypeNames.Text.Plain :
                    MediaTypeNames.Application.Json;

        protected static DateTime GetDefaultTime(
            HttpContext? context = null,
            DateTime? defaultDateTime = null
        ) =>
            context?.Features.Get<IHttpRequestTimeFeature>()?.RequestTime ??
                defaultDateTime.GetValueOrDefault(DateTime.Now);

        protected readonly bool _disposeMembers;
        protected readonly IMemoryCache _cache;
        protected readonly ILogger _logger;

        protected bool Disposed { get; private set; }

        public InternalBaseController(
            IMemoryCache cache,
            ILogger logger,
            bool disposeMembers = false
        )
        {
            _cache = cache ??
                throw new ArgumentNullException(nameof(cache));
            _logger = logger ??
                throw new ArgumentNullException(nameof(logger));
            _disposeMembers = disposeMembers;

            Disposed = false;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        protected IActionResult Null(
            bool noContentIfNull
        )
        {
            if (noContentIfNull)
            {
                return NoContent();
            }

            Response.StatusCode = (int)HttpStatusCode.OK;

            return Content(
                "null",
                GetAdequateContentType(Request.ContentType)
            );
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        protected IActionResult InvokeAction(
            Action action,
            object? okObject = null,
            bool noContentIfNull = false,
            HttpStatusCode errorStatusCode = HttpStatusCode.BadRequest,
            object? errorObject = null
        ) =>
            InvokeFunc(
                () =>
                {
                    action();

                    return okObject;
                },
                noContentIfNull,
                errorStatusCode,
                errorObject
            );

        [ApiExplorerSettings(IgnoreApi = true)]
        protected Task<IActionResult> InvokeActionAsync(
            Func<Task> asyncAction,
            object? okObject = null,
            bool noContentIfNull = false,
            HttpStatusCode errorStatusCode = HttpStatusCode.BadRequest,
            object? errorObject = null
        ) =>
            InvokeFuncAsync(
                async () =>
                {
                    await asyncAction()
                        .ConfigureAwait(false);

                    return okObject;
                },
                noContentIfNull,
                errorStatusCode,
                errorObject
            );

        [ApiExplorerSettings(IgnoreApi = true)]
        protected IActionResult InvokeFunc<TResult>(
            Func<TResult> func,
            bool noContentIfNull = false,
            HttpStatusCode errorStatusCode = HttpStatusCode.BadRequest,
            object? errorObject = null
        )
        {
            TResult result;

            try
            {
                result = func();
            }
            catch (InternalException exception)
            {
                return string.IsNullOrEmpty(exception.Message) ?
                    StatusCode((int)HttpStatusCode.InternalServerError) :
                    BadRequest(exception.Message);
            }
            catch (InvalidOperationException)
            {
                return StatusCode((int)errorStatusCode, errorObject);
            }

            if (result is null)
            {
                return Null(noContentIfNull);
            }

            return Ok(result);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        protected async Task<IActionResult> InvokeFuncAsync<TResult>(
            Func<Task<TResult>> asyncFunc,
            bool noContentIfNull = false,
            HttpStatusCode errorStatusCode = HttpStatusCode.BadRequest,
            object? errorObject = null
        )
        {
            TResult result;

            try
            {
                result = await asyncFunc()
                    .ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                return NoContent();
            }
            catch (InternalException exception)
            {
                return string.IsNullOrEmpty(exception.Message) ?
                    StatusCode((int)HttpStatusCode.InternalServerError) :
                    BadRequest(exception.Message);
            }
            catch (InvalidOperationException)
            {
                return StatusCode((int)errorStatusCode, errorObject);
            }

            if (result is null)
            {
                return Null(noContentIfNull);
            }

            return Ok(result);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        protected virtual void Dispose(
            bool disposing
        )
        {
            if (disposing && !Disposed)
            {
                if (_disposeMembers)
                {
                    // ...
                }
            }

            Disposed = true;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        protected virtual ValueTask DisposeAsync(
            bool disposing
        )
        {
            if (disposing && !Disposed)
            {
                if (_disposeMembers)
                {
                    // ...
                }
            }

            Disposed = true;

            return ValueTask.CompletedTask;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public async ValueTask DisposeAsync()
        {
            await DisposeAsync(true)
                .ConfigureAwait(false);

            GC.SuppressFinalize(this);
        }

        ~InternalBaseController()
        {
            Dispose(false);
        }
    }
}
