using HatTrick.API.Models;
using HatTrick.BLL;
using HatTrick.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace HatTrick.API.Controllers
{
    /// <summary>Provides endpoints for account journey.</summary>
    [Route("API/[controller]")]
    public class AccountController : InternalBaseController
    {
        protected Account Account =>
            (Account)_business;

        public AccountController(
            Account account,
            IMemoryCache cache,
            ILogger<AccountController> logger,
            bool disposeMembers = false
        ) :
            base(account, cache, logger, disposeMembers)
        {
        }

        /// <summary>Gets the information about a user.</summary>
        /// <param name="userId">The user id number.</param>
        /// <param name="includeTickets">If <c>true</c>, tickets shall be included in the user information.</param>
        /// <param name="includeTicketSelections">If <c>true</c>, ticket selections shall be included in the user information. If the <c><paramref name="includeTickets" /></c> parameter is not <c>true</c>, this parameter is ignored.</param>
        /// <param name="stateAt">The date-time at which to observe the user. If omitted, current time is used.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>The response.</returns>
        /// <response code="200">The user information.</response>
        /// <response code="400">Request failed.</response>
        /// <response code="404">The user was not found.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(User))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserAsync(
            int userId,
            [FromQuery] DateTime? stateAt = null,
            [FromQuery] bool? includeTickets = null,
            [FromQuery] bool? includeTicketSelections = null,
            CancellationToken cancellationToken = default
        ) =>
            await InvokeFuncAsync(
                () => Account.GetUserAsync(
                    userId,
                    stateAt.GetValueOrDefault(
                        GetDefaultTime(HttpContext)
                    ),
                    includeTickets.GetValueOrDefault(false),
                    includeTicketSelections.GetValueOrDefault(false),
                    cancellationToken
                )
            )
                .ConfigureAwait(false);

        /// <summary>Creates a new transaction.</summary>
        /// <param name="transactionRequest">The new transaction information.</param>
        /// <param name="time">The date-time at which to make the transaction. If omitted, current time is used.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>The response.</returns>
        /// <response code="201">The newly created transaction containing basic information.</response>
        /// <response code="400">Request failed.</response>
        /// <response code="404">The user was not found.</response>
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Transaction))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        [HttpPost]
        public async Task<IActionResult> PostTransactionAsync(
            [FromBody] TransactionRequestModel transactionRequest,
            [FromQuery] DateTime? time = null,
            CancellationToken cancellationToken = default
        )
        {
            if (!Enum.IsDefined(transactionRequest.Type))
            {
                ModelState.AddModelError(
                    nameof(TransactionRequestModel.Type),
                    "Type value must be from the defined range."
                );
            }
            if (transactionRequest.Type == TransactionRequestType.Unspecified)
            {
                ModelState.AddModelError(
                    nameof(TransactionRequestModel.Type),
                    "Type may not be unspecified."
                );
            }

            if (ModelState.ErrorCount != 0)
            {
                return BadRequest(ModelState);
            }

            return await InvokeFuncAsync(
                () => Account.MakeTransactionAsync(
                    time.GetValueOrDefault(GetDefaultTime(HttpContext)),
                    transactionRequest.UserId,
                    transactionRequest.Type.IsDebosit(),
                    transactionRequest.Amount,
                    cancellationToken
                ),
                HttpStatusCode.Created
            )
                .ConfigureAwait(false);
        }
    }
}
