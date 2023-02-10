using HatTrick.API.Models;
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
    [Route("API/[controller]")]
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
        /// <param name="userInfoRequest">The information specifying what user data to retrieve.</param>
        /// <param name="stateAt">The date-time at which to observe the user. If omitted, current time is used.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>The response.</returns>
        /// <response code="200">The user information.</response>
        /// <response code="400">Request failed.</response>
        /// <response code="404">The user was not found.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(User))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        [HttpPost]
        public async Task<IActionResult> PostAsync(
            [FromBody] UserInfoRequestModel userInfoRequest,
            [FromQuery] DateTime? stateAt = null,
            CancellationToken cancellationToken = default
        ) =>
            await InvokeFuncAsync(
                () => _account.GetUserAsync(
                    userInfoRequest.UserId,
                    stateAt.GetValueOrDefault(
                        GetDefaultTime(HttpContext)
                    ),
                    userInfoRequest.IncludeTickets,
                    userInfoRequest.IncludeTicketSelections,
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
