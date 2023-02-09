using HatTrick.BLL;
using HatTrick.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HatTrick.API.Controllers
{
    /// <summary>Provides endpoints for accessing the information about the betting offer.</summary>
    [Route("[controller]")]
    public class OfferController : InternalBaseController
    {
        protected readonly Offer _offer;

        public OfferController(
            Offer offer,
            IMemoryCache cache,
            ILogger<OfferController> logger,
            bool disposeMembers = false
        ) :
            base(cache, logger, disposeMembers)
        {
            _offer = offer ??
                throw new ArgumentNullException(nameof(offer));
        }

        /// <summary>Gets the complete offer.</summary>
        /// <param name="availableAt">The date-time at which to observe the offer. If omitted, current time is used.</param>
        /// <param name="promoted">If specified, events and fixtures are also filtered by the appropriate promotion status.</param>
        /// <param name="skip">The number of events to skip for pagination purposses.</param>
        /// <param name="take">The number of events to take for pagination purposses.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>The response.</returns>
        /// <remarks>
        ///     <para>Even if pagination is not explicitly used (i. e. parameters <c><paramref name="skip" /></c> and <c><paramref name="take" /></c> are omitted), there is an upper limit on how much events can be retrieved at once. To make sure all events are retrieved, call the endpoint multiple times, each time skipping the cumulative sum of the event counts from the previous calls, until the endpoint returns an empty collection of events.</para>
        /// </remarks>
        /// <response code="200">The list of events, fixtures, markets and outcomes available at <c><paramref name="availableAt" /></c>.</response>
        /// <response code="400">Query failed due to bad user request.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Event[]))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet]
        public async Task<IActionResult> GetAsync(
            [FromQuery] DateTime? availableAt = null,
            [FromQuery] bool? promoted = null,
            [FromQuery] int? skip = null,
            [FromQuery] int? take = null,
            CancellationToken cancellationToken = default
        )
        {
            if (skip.HasValue && skip.Value < 0)
            {
                ModelState.AddModelError(
                    nameof(skip),
                    "Number must be non-negative."
                );
            }
            if (take.HasValue && take.Value < 0)
            {
                ModelState.AddModelError(
                    nameof(take),
                    "Number must be non-negative."
                );
            }

            if (ModelState.ErrorCount != 0)
            {
                return BadRequest(ModelState);
            }

            return await InvokeFuncAsync(
                () => _offer.GetEventsAsync(
                    availableAt.GetValueOrDefault(
                        GetDefaultTime(HttpContext)
                    ),
                    null,
                    promoted,
                    null,
                    skip.GetValueOrDefault(0),
                    take.GetValueOrDefault(Offer.DefaultTakeN),
                    cancellationToken
                )
            )
                .ConfigureAwait(false);
        }

        protected override void Dispose(
            bool disposing
        )
        {
            if (disposing && !Disposed)
            {
                if (_disposeMembers)
                {
                    _offer.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        protected override async ValueTask DisposeAsync(
            bool disposing
        )
        {
            if (disposing && !Disposed)
            {
                if (_disposeMembers)
                {
                    await _offer.DisposeAsync()
                        .ConfigureAwait(false);
                }
            }

            await base.DisposeAsync(disposing)
                .ConfigureAwait(false);
        }
    }
}
