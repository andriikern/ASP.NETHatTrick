using HatTrick.BLL.Exceptions;
using HatTrick.DAL;
using HatTrick.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HatTrick.BLL
{
    public sealed class Account : IDisposable, IAsyncDisposable
    {
        private static IQueryable<User> IncludeAdditionalInformation(
            IQueryable<User> usersQuery,
            DateTime? stateAt = null,
            bool includeTickets = false,
            bool includeTicketSelections = false,
            bool includeTransactions = false
        ) =>
            (includeTickets, includeTicketSelections, includeTransactions) switch
            {
                (true, true, true) =>
                    usersQuery.Include(
                        u => u.Tickets
                            .Where(
                                t =>
                                    stateAt == null ||
                                        t.PayInTime <= stateAt
                            )
                            .OrderBy(t => t.PayInTime)
                            .ThenByDescending(t => t.TotalOdds)
                            .ThenByDescending(t => t.PayInAmount)
                    )
                        .ThenInclude(t => t.Status)
                        .Include(
                            u => u.Tickets
                                .Where(
                                    t =>
                                        stateAt == null ||
                                            t.PayInTime <= stateAt
                                )
                                .OrderBy(t => t.PayInTime)
                                .ThenByDescending(t => t.TotalOdds)
                                .ThenByDescending(t => t.PayInAmount)
                        )
                        .ThenInclude(t => t.Selections)
                        .Include(
                            u => u.Tickets
                                .Where(
                                    t =>
                                        stateAt == null ||
                                            t.PayInTime <= stateAt
                                )
                                .OrderBy(t => t.PayInTime)
                                .ThenByDescending(t => t.TotalOdds)
                                .ThenByDescending(t => t.PayInAmount)
                        )
                        .ThenInclude(t => t.Transactions)
                        .Include(
                            u => u.Transactions
                                .Where(
                                    t =>
                                        stateAt == null ||
                                            t.Time <= stateAt
                                )
                                .OrderBy(t => t.Time)
                                .ThenByDescending(t => t.Amount)
                        )
                        .ThenInclude(t => t.Type)
                        .Include(
                            u => u.Transactions
                                .Where(
                                    t =>
                                        stateAt == null ||
                                            t.Time <= stateAt
                                )
                                .OrderBy(t => t.Time)
                                .ThenByDescending(t => t.Amount)
                        )
                        .ThenInclude(t => t.Ticket),
                (true, true, false) =>
                    usersQuery.Include(
                        u => u.Tickets
                            .Where(
                                t =>
                                    stateAt == null ||
                                        t.PayInTime <= stateAt
                            )
                            .OrderBy(t => t.PayInTime)
                            .ThenByDescending(t => t.TotalOdds)
                            .ThenByDescending(t => t.PayInAmount)
                    )
                        .ThenInclude(t => t.Status)
                        .Include(
                            u => u.Tickets
                                .Where(
                                    t =>
                                        stateAt == null ||
                                            t.PayInTime <= stateAt
                                )
                                .OrderBy(t => t.PayInTime)
                                .ThenByDescending(t => t.TotalOdds)
                                .ThenByDescending(t => t.PayInAmount)
                        )
                        .ThenInclude(t => t.Selections),
                (false, _, true) =>
                    usersQuery.Include(
                        u => u.Transactions
                            .Where(
                                t =>
                                    stateAt == null ||
                                        t.Time <= stateAt
                            )
                            .OrderBy(t => t.Time)
                            .ThenByDescending(t => t.Amount)
                    )
                        .ThenInclude(t => t.Type),
                _ => usersQuery
            };

        private static IQueryable<User> FilterAndSort(
            IQueryable<User> usersQuery,
            DateTime? stateAt = null,
            int? id = null
        ) =>
            usersQuery.Where(
                u =>
                    (
                        stateAt == null ||
                            (
                                u.RegisteredOn <= stateAt &&
                                    (
                                        u.DeactivatedOn == null ||
                                            u.DeactivatedOn > stateAt
                                    )
                            )
                    ) &&
                        u.Id == id
            )
                .OrderBy(u => u.Username)
                .ThenBy(u => u.RegisteredOn)
                .ThenByDescending(u => u.DeactivatedOn);

        private readonly bool _disposeMembers;
        private readonly Context _context;
        private readonly ILogger<Account> _logger;

        private bool disposed;

        public Account(
            Context context,
            ILogger<Account> logger,
            bool disposeMembers = true
        )
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context));
            _logger = logger ??
                throw new ArgumentNullException(nameof(logger));
            _disposeMembers = disposeMembers;

            disposed = false;
        }

        public async Task<User> GetUserAsync(
            int id,
            DateTime? stateAt = null,
            bool includeTickets = false,
            bool includeTicketSelections = false,
            bool includeTransactions = false,
            CancellationToken cancellationToken = default
        )
        {
            User user;

            _logger.LogDebug(
                "Fetching user from the database... Id: {id}, state at: {stateAt}, include tickets: {includeTickets}, include ticket selections: {includeTicketSelections}, include transactions: {includeTransactions}",
                    id,
                    stateAt,
                    includeTickets,
                    includeTicketSelections,
                    includeTransactions
            );

            try
            {
                var dbTxn = await _context.Database
                    .BeginTransactionAsync(cancellationToken)
                    .ConfigureAwait(false);
                await using (dbTxn.ConfigureAwait(false))
                {
                    // Initialise query.
                    IQueryable<User> userQuery = _context.Users;

                    // Include all requested and related entities.
                    userQuery = IncludeAdditionalInformation(
                        userQuery,
                        stateAt,
                        includeTickets,
                        includeTicketSelections,
                        includeTransactions
                    );

                    // Filter.
                    userQuery = FilterAndSort(
                        userQuery,
                        stateAt,
                        id
                    );

                    // Download user.
                    try
                    {
                        user = await userQuery.AsSplitQuery()
                            .AsNoTrackingWithIdentityResolution()
                            .SingleAsync(cancellationToken)
                            .ConfigureAwait(false);
                    }
                    catch (InvalidOperationException exception)
                    {
                        throw new InternalException(
                            "The user does not exist.",
                            exception
                        );
                    }
                }
            }
            catch (Exception exception)
                when (
                    exception is InvalidOperationException ||
                    exception is InternalException
                )
            {
                _logger.LogError(
                    exception,
                    "Error while fetching user from the database. Id: { id}, state at: { stateAt}, include tickets: { includeTickets}, include ticket selections: { includeTicketSelections}, include transactions: { includeTransactions}",
                        id,
                        stateAt,
                        includeTickets,
                        includeTicketSelections,
                        includeTransactions
                );

                if (exception is not InternalException)
                {
                    throw new InternalException(null, exception);
                }

                throw;
            }

            _logger.LogInformation(
                "User successfully fetched from the database... Id: {id}, state at: {stateAt}, include tickets: {includeTickets}, include ticket selections: {includeTicketSelections}, include transactions: {includeTransactions}, user: {@user}",
                    id,
                    stateAt,
                    includeTickets,
                    includeTicketSelections,
                    includeTransactions,
                    user
            );

            return user;
        }

        private void Dispose(
            bool disposing
        )
        {
            if (!disposed && disposing)
            {
                if (_disposeMembers)
                {
                    _context.Dispose();
                }
            }

            disposed = true;
        }

        private async ValueTask DisposeAsync(
            bool disposing
        )
        {
            if (!disposed && disposing)
            {
                if (_disposeMembers)
                {
                    await _context
                        .DisposeAsync()
                        .ConfigureAwait(false);
                }
            }

            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsync(true)
                .ConfigureAwait(false);

            GC.SuppressFinalize(this);
        }

        ~Account()
        {
            Dispose(false);
        }
    }
}
