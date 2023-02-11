using HatTrick.API.Models;
using HatTrick.BLL;
using HatTrick.BLL.Models;
using HatTrick.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Immutable;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace HatTrick.API.Controllers
{
    /// <summary>Provides endpoints for placing bets.</summary>
    [Route("API/[controller]")]
    public class BettingShopController : InternalBaseController
    {
        protected readonly BettingShop _bettingShop;

        public BettingShopController(
            BettingShop bettingShop,
            IMemoryCache cache,
            ILogger<BettingShopController> logger,
            bool disposeMembers = false
        ) :
            base(cache, logger, disposeMembers)
        {
            _bettingShop = bettingShop ??
                throw new ArgumentNullException(nameof(bettingShop));
        }

        /// <summary>Places a new bet (ticket).</summary>
        /// <param name="ticketRequest">The new ticket information.</param>
        /// <param name="placedAt">The date-time at which to place the bet. If omitted, current time is used.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>The response.</returns>
        /// <response code="200">The id number of the ticket.</response>
        /// <response code="400">Request failed.</response>
        /// <response code="404">The user was not found.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        [HttpPost]
        public async Task<IActionResult> PostAsync(
            [FromBody] TicketRequestModel ticketRequest,
            [FromQuery] DateTime? placedAt = null,
            CancellationToken cancellationToken = default
        ) =>
            await InvokeFuncAsync(
                () => _bettingShop.PlaceBetAsync(
                    placedAt.GetValueOrDefault(
                        GetDefaultTime(HttpContext)
                    ),
                    ticketRequest.UserId,
                    ticketRequest.SelectionIds.ToImmutableArray(),
                    ticketRequest.Amount,
                    cancellationToken
                ),
                HttpStatusCode.Created
            )
                .ConfigureAwait(false);

        /// <summary>Gets the information about a ticket.</summary>
        /// <param name="ticketId">The ticket id number.</param>
        /// <param name="stateAt">The date-time at which to observe the ticket. If omitted, current time is used.</param>
        /// <param name="includeSelections">If <c>true</c>, basic information about ticket selections shall be returned, as well.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>The response.</returns>
        /// <remarks>
        ///     <para>Although it is possible to provide the <c><paramref name="stateAt" /></c> parameter to specify at which time to pbserve the ticket, this does not affect ticket status and ticket resolution information. The ticket might be indicated as resolved although its resolution time is after the specified <c><paramref name="stateAt" /></c> time point.</para>
        /// </remarks>
        /// <response code="200">The ticket information.</response>
        /// <response code="400">Request failed.</response>
        /// <response code="404">The ticket was not found.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Ticket))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        [HttpGet("{ticketId}")]
        public async Task<IActionResult> GetAsync(
            int ticketId,
            [FromQuery] DateTime? stateAt = null,
            [FromQuery] bool? includeSelections = null,
            CancellationToken cancellationToken = default
        ) =>
            await InvokeFuncAsync(
                () => _bettingShop.GetTicketAsync(
                    ticketId,
                    stateAt.GetValueOrDefault(
                        GetDefaultTime(HttpContext)
                    ),
                    includeSelections.GetValueOrDefault(false),
                    cancellationToken
                )
            )
                .ConfigureAwait(false);

        /// <summary>Gets the information about ticket's financial amounts.</summary>
        /// <param name="ticketId">The ticket id number.</param>
        /// <param name="stateAt">The date-time at which to observe the ticket. If omitted, current time is used.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>The response.</returns>
        /// <remarks>
        ///     <para>Regardless of the <c><paramref name="stateAt" /></c> parameter, potential wins are calculated always. The parameter is used only for filtering tickets having been payed-in before <c><paramref name="stateAt" /></c>.</para>
        /// </remarks>
        /// <response code="200">The information about ticket's financial amounts.</response>
        /// <response code="400">Request failed.</response>
        /// <response code="404">The ticket was not found.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TicketFinancialAmounts))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        [HttpGet("{ticketId}/Amounts")]
        public async Task<IActionResult> Get1Async(
            int ticketId,
            [FromQuery] DateTime? stateAt = null,
            CancellationToken cancellationToken = default
        ) =>
            await InvokeFuncAsync(
                () => _bettingShop.CalculateTicketFinancialAmountsAsync(
                    ticketId,
                    stateAt.GetValueOrDefault(
                        GetDefaultTime(HttpContext)
                    ),
                    cancellationToken
                )
            )
                .ConfigureAwait(false);

        protected override void Dispose(
            bool disposing
        )
        {
            if (disposing && !Disposed)
            {
                if (_disposeMembers)
                {
                    _bettingShop.Dispose();
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
                    await _bettingShop.DisposeAsync()
                        .ConfigureAwait(false);
                }
            }

            await base.DisposeAsync(disposing)
                .ConfigureAwait(false);
        }
    }
}
