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
    /// <summary>Provides endpoints for account journey.</summary>
    [Route("[controller]")]
    public class AccountController : InternalBaseController
    {
        protected readonly Account _account;

        public AccountController(
            Account account,
            IMemoryCache cache,
            ILogger<AccountController> logger,
            bool disposeMembers = false
        ) :
            base(cache, logger, disposeMembers)
        {
            _account = account ??
                throw new ArgumentNullException(nameof(account));
        }

        /// <summary>Gets the information about a user.</summary>
        /// <param name="userId">The user's id number.</param>
        /// <param name="stateAt">The date-time at which to observe the user. If omitted, current time is used.</param>
        /// <param name="includeTickets">If <c>true</c>, ticket information will be included, as well.</param>
        /// <param name="includeTicketSelections">If <c>true</c>, selection information will be included in the ticket information, as well. If <c><paramref name="includeTickets" /></c> is not <c>true</c>, this parameter is ignored.</param>
        /// <param name="includeTransactions">If <c>true</c>, transaction information will be included, as well.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>The response.</returns>
        /// <response code="200">The bet was placed successfully.</response>
        /// <response code="400">Request failed.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(User))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        public async Task<IActionResult> PostAsync(
            [FromBody] int userId,
            [FromQuery] DateTime? stateAt = null,
            [FromQuery] bool? includeTickets = null,
            [FromQuery] bool? includeTicketSelections = null,
            [FromQuery] bool? includeTransactions = null,
            CancellationToken cancellationToken = default
        ) =>
            await InvokeFuncAsync(
                () => _account.GetUserAsync(
                    userId,
                    stateAt.GetValueOrDefault(
                        GetDefaultTime(HttpContext)
                    ),
                    includeTickets.GetValueOrDefault(false),
                    includeTicketSelections.GetValueOrDefault(false),
                    includeTransactions.GetValueOrDefault(false),
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
                    _account.Dispose();
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
                    await _account.DisposeAsync()
                        .ConfigureAwait(false);
                }
            }

            await base.DisposeAsync(disposing)
                .ConfigureAwait(false);
        }
    }
}
