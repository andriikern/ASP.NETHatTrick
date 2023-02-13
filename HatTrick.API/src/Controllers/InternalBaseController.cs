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
using HatTrick.BLL;

namespace HatTrick.API.Controllers
{
    /// <summary>Provides fields, properties and methods common to all application's controllers.</summary>
    [Produces(MediaTypeNames.Application.Json, "text/json", MediaTypeNames.Text.Plain), Route("API"), ApiController]
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
        protected readonly Business _business;

        protected bool Disposed { get; private set; }

        private protected InternalBaseController(
            Business business,
            IMemoryCache cache,
            ILogger logger,
            bool disposeMembers = false
        )
        {
            _disposeMembers = disposeMembers;

            _business = business ??
                throw new ArgumentNullException(nameof(business));
            _cache = cache ??
                throw new ArgumentNullException(nameof(cache));
            _logger = logger ??
                throw new ArgumentNullException(nameof(logger));

            Disposed = false;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        protected IActionResult Null(
            HttpStatusCode statusCode = HttpStatusCode.OK,
            bool noContentIfNull = false
        )
        {
            if (noContentIfNull)
            {
                return NoContent();
            }

            Response.StatusCode = (int)statusCode;

            return Content(
                "null",
                GetAdequateContentType(Request.ContentType)
            );
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        protected IActionResult InvokeFunc<TResult>(
            Func<TResult> func,
            HttpStatusCode successStatusCode = HttpStatusCode.OK,
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
                if (exception.Reason.HasFlag(InternalExceptionReason.ServerError))
                {
                    return StatusCode(
                        (int)HttpStatusCode.InternalServerError
                    );
                }
                if (exception.Reason.HasFlag(InternalExceptionReason.NotFound))
                {
                    return NotFound(exception.Message);
                }
                if (exception.Reason.HasFlag(InternalExceptionReason.BadInput))
                {
                    return BadRequest(exception.Message);
                }

                return StatusCode(
                    (int)HttpStatusCode.InternalServerError
                );
            }
            catch (InvalidOperationException)
            {
                return StatusCode(
                    (int)errorStatusCode,
                    errorObject
                );
            }

            if (result is null)
            {
                return Null(successStatusCode, noContentIfNull);
            }

            return StatusCode(
                (int)successStatusCode,
                result
            );
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        protected async Task<IActionResult> InvokeFuncAsync<TResult>(
            Func<Task<TResult>> asyncFunc,
            HttpStatusCode successStatusCode = HttpStatusCode.OK,
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
                if (exception.Reason.HasFlag(InternalExceptionReason.ServerError))
                {
                    return StatusCode(
                        (int)HttpStatusCode.InternalServerError
                    );
                }
                if (exception.Reason.HasFlag(InternalExceptionReason.NotFound))
                {
                    return NotFound(exception.Message);
                }
                if (exception.Reason.HasFlag(InternalExceptionReason.BadInput))
                {
                    return BadRequest(exception.Message);
                }

                return StatusCode(
                    (int)HttpStatusCode.InternalServerError
                );
            }
            catch (InvalidOperationException)
            {
                return StatusCode(
                    (int)errorStatusCode,
                    errorObject
                );
            }

            if (result is null)
            {
                return Null(successStatusCode, noContentIfNull);
            }

            return StatusCode(
                (int)successStatusCode,
                result
            );
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        protected IActionResult InvokeAction(
            Action action,
            HttpStatusCode successStatusCode = HttpStatusCode.OK,
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
                successStatusCode,
                noContentIfNull,
                errorStatusCode,
                errorObject
            );

        [ApiExplorerSettings(IgnoreApi = true)]
        protected Task<IActionResult> InvokeActionAsync(
            Func<Task> asyncAction,
            HttpStatusCode successStatusCode = HttpStatusCode.OK,
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
                successStatusCode,
                noContentIfNull,
                errorStatusCode,
                errorObject
            );

        [ApiExplorerSettings(IgnoreApi = true)]
        protected virtual void Dispose(
            bool disposing
        )
        {
            if (disposing && !Disposed)
            {
                if (_disposeMembers)
                {
                    _business.Dispose();
                }
            }

            Disposed = true;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        protected virtual async ValueTask DisposeAsync(
            bool disposing
        )
        {
            if (disposing && !Disposed)
            {
                if (_disposeMembers)
                {
                    await _business.DisposeAsync()
                        .ConfigureAwait(false);
                }
            }

            Disposed = true;
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
