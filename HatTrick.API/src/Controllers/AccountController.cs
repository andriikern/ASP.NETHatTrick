using HatTrick.API.Models;
using HatTrick.BLL;
using HatTrick.BLL.Exceptions;
using HatTrick.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;

namespace HatTrick.API.Controllers
{
    /// <summary>Provides endpoints for account journey.</summary>
    [Produces(MediaTypeNames.Application.Json, "text/json", MediaTypeNames.Text.Plain), Route("API/[controller]"), ApiController]
    public sealed class AccountController : ControllerBase, IDisposable, IAsyncDisposable
    {
        private readonly bool _disposeMembers;
        private readonly IMemoryCache _cache;
        private readonly ILogger _logger;
        private readonly Account _account;

        private bool disposed;

        public AccountController(
            Account account,
            IMemoryCache cache,
            ILogger<AccountController> logger,
            bool disposeMembers = false
        )
        {
            _disposeMembers = disposeMembers;

            _account = account ??
                throw new ArgumentNullException(nameof(account));
            _cache = cache ??
                throw new ArgumentNullException(nameof(cache));
            _logger = logger ??
                throw new ArgumentNullException(nameof(logger));

            disposed = false;
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
        )
        {
            User user;

            try
            {
                user = await _account.GetUserAsync(
                    userId,
                    stateAt.GetValueOrDefault(
                        ControllerBaseExtensions.GetDefaultTime(this)
                    ),
                    includeTickets.GetValueOrDefault(false),
                    includeTicketSelections.GetValueOrDefault(false),
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

            return Ok(user);
        }

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

            Transaction transaction;

            try
            {
                transaction = await _account.MakeTransactionAsync(
                    time.GetValueOrDefault(
                        ControllerBaseExtensions.GetDefaultTime(this)
                    ),
                    transactionRequest.UserId,
                    transactionRequest.Type.IsDebosit(),
                    transactionRequest.Amount,
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

            return Created(string.Empty, transaction);
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
                    _account.Dispose();
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
                    await _account.DisposeAsync()
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

        ~AccountController()
        {
            Dispose(false);
        }
    }
}
