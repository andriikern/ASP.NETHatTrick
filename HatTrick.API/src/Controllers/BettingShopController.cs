using HatTrick.API.Models;
using HatTrick.BLL;
using HatTrick.BLL.Exceptions;
using HatTrick.BLL.Models;
using HatTrick.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Immutable;
using System.Net;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;

namespace HatTrick.API.Controllers
{
    /// <summary>Provides endpoints for placing bets.</summary>
    [Produces(MediaTypeNames.Application.Json, "text/json", MediaTypeNames.Text.Plain), Route("API/[controller]"), ApiController]
    public sealed class BettingShopController : ControllerBase, IDisposable, IAsyncDisposable
    {
        private readonly bool _disposeMembers;
        private readonly IMemoryCache _cache;
        private readonly ILogger _logger;
        private readonly BettingShop _bettingShop;

        private bool disposed;

        public BettingShopController(
            BettingShop bettingShop,
            IMemoryCache cache,
            ILogger<AccountController> logger,
            bool disposeMembers = false
        )
        {
            _disposeMembers = disposeMembers;

            _bettingShop = bettingShop ??
                throw new ArgumentNullException(nameof(bettingShop));
            _cache = cache ??
                throw new ArgumentNullException(nameof(cache));
            _logger = logger ??
                throw new ArgumentNullException(nameof(logger));

            disposed = false;
        }

        /// <summary>Places a new bet (ticket).</summary>
        /// <param name="ticketRequest">The new ticket information.</param>
        /// <param name="placedAt">The date-time at which to place the bet. If omitted, current time is used.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>The response.</returns>
        /// <response code="201">The newly created ticket containing basic information.</response>
        /// <response code="400">Request failed.</response>
        /// <response code="404">The user was not found.</response>
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Ticket))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        [HttpPost]
        public async Task<IActionResult> PostTicketAsync(
            [FromBody] TicketRequestModel ticketRequest,
            [FromQuery] DateTime? placedAt = null,
            CancellationToken cancellationToken = default
        )
        {
            Ticket ticket;

            try
            {
                ticket = await _bettingShop.PlaceBetAsync(
                    placedAt.GetValueOrDefault(
                        ControllerBaseExtensions.GetDefaultTime(this)
                    ),
                    ticketRequest.UserId,
                    ticketRequest.SelectionIds.ToImmutableArray(),
                    ticketRequest.Amount,
                    cancellationToken
                )
                    .ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                return NoContent();
            }
            catch (InternalNotFoundException exception)
            {
                return NotFound(exception.Message);
            }
            catch (InternalBadInputException exception)
            {
                return BadRequest(exception.Message);
            }
            catch (InternalException)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return Created($"API/BettingShop/{ticket.Id}", ticket);
        }

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
        public async Task<IActionResult> GetTicketAsync(
            int ticketId,
            [FromQuery] DateTime? stateAt = null,
            [FromQuery] bool? includeSelections = null,
            CancellationToken cancellationToken = default
        )
        {
            Ticket ticket;

            try
            {
                ticket = await _bettingShop.GetTicketAsync(
                    ticketId,
                    stateAt.GetValueOrDefault(
                        ControllerBaseExtensions.GetDefaultTime(this)
                    ),
                    includeSelections.GetValueOrDefault(false),
                    cancellationToken
                )
                    .ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                return NoContent();
            }
            catch (InternalNotFoundException exception)
            {
                return NotFound(exception.Message);
            }
            catch (InternalBadInputException exception)
            {
                return BadRequest(exception.Message);
            }
            catch (InternalException)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return Ok(ticket);
        }

        /// <summary>Gets the ticket selections.</summary>
        /// <param name="ticketId">The ticket id number.</param>
        /// <param name="stateAt">The date-time at which to observe the ticket collection. If omitted, current time is used.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>The response.</returns>
        /// <remarks>
        ///     <para>The parameter <c><paramref name="stateAt" /></c> is used only for filtering tickets having been payed-in before <c><paramref name="stateAt" /></c>, but it otherwise does not affect statueses, availability, or any other information about selections</para>
        /// </remarks>
        /// <response code="200">The ticket selections from the event point of view.</response>
        /// <response code="400">Request failed.</response>
        /// <response code="404">The ticket was not found.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Event[]))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        [HttpGet("{ticketId}/Selections")]
        public async Task<IActionResult> GetTicketSelectionsAsync(
            int ticketId,
            [FromQuery] DateTime? stateAt = null,
            CancellationToken cancellationToken = default
        )
        {
            Event[] events;

            try
            {
                events = await _bettingShop.GetTicketSelectionsAsync(
                    ticketId,
                    stateAt.GetValueOrDefault(
                        ControllerBaseExtensions.GetDefaultTime(this)
                    ),
                    cancellationToken
                )
                    .ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                return NoContent();
            }
            catch (InternalNotFoundException exception)
            {
                return NotFound(exception.Message);
            }
            catch (InternalBadInputException exception)
            {
                return BadRequest(exception.Message);
            }
            catch (InternalException)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return Ok(events);
        }

        /// <summary>Gets the manipulative cost rate.</summary>
        /// <returns>The response.</returns>
        /// <response code="200">The manipulative cost rate.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(decimal))]
        [HttpGet("ManipulativeCostRate")]
        public IActionResult GetDefaultManipulativeCostRate() =>
            Ok(Business.ManipulativeCostRate);

        /// <summary>Gets the information about the ticket's financial amounts.</summary>
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
        public async Task<IActionResult> GetTicketFinancialAmountsAsync(
            int ticketId,
            [FromQuery] DateTime? stateAt = null,
            CancellationToken cancellationToken = default
        )
        {
            TicketFinancialAmounts amounts;

            try
            {
                amounts = await _bettingShop.CalculateTicketFinancialAmountsAsync(
                    ticketId,
                    stateAt.GetValueOrDefault(
                        ControllerBaseExtensions.GetDefaultTime(this)
                    ),
                    cancellationToken
                )
                    .ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                return NoContent();
            }
            catch (InternalNotFoundException exception)
            {
                return NotFound(exception.Message);
            }
            catch (InternalBadInputException exception)
            {
                return BadRequest(exception.Message);
            }
            catch (InternalException)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return Ok(amounts);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        private void Dispose(
            bool disposing
        )
        {
            if (disposing && !disposed)
            {
                if (_disposeMembers)
                {
                    _bettingShop.Dispose();
                }
            }

            disposed = true;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        private async ValueTask DisposeAsync(
            bool disposing
        )
        {
            if (disposing && !disposed)
            {
                if (_disposeMembers)
                {
                    await _bettingShop.DisposeAsync()
                        .ConfigureAwait(false);
                }
            }

            disposed = true;
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

        ~BettingShopController()
        {
            Dispose(false);
        }
    }
}
