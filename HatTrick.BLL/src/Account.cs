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
    public sealed class Account : Business
    {
        private static decimal EnsureValidTransactionAmount(
            bool deposit,
            decimal balance,
            decimal amount
        )
        {
            amount = Round(amount);

            if (amount < decimal.Zero)
            {
                throw new InternalBadInputException(
                    "Transaction amount is negative."
                );
            }

            if (amount < MinTransactionAmount || amount > MaxTransactionAmount)
            {
                throw new InternalBadInputException(
                    $"Transaction amount is out of range. Minimal allowed transaction is {MinTransactionAmount:N2}, maximal allowed single transaction is {MaxTransactionAmount:N2}."
                );
            }

            if (!deposit && amount > balance)
            {
                throw new InternalBadInputException(
                    $"Withdrawal amount exceeds the current balance of {balance:N2}."
                );
            }

            return amount;
        }

        private static IQueryable<User> IncludeAdditionalInformation(
            IQueryable<User> usersQuery,
            DateTime? stateAt = null,
            bool includeTickets = false,
            bool includeTicketSelections = false
        ) =>
            (includeTickets, includeTicketSelections) switch
            {
                (true, true) =>
                    usersQuery.Include(
                        u => u.Tickets
                            .Where(
                                t =>
                                    stateAt == null ||
                                        t.PayInTime <= stateAt
                            )
                            .OrderBy(t => t.PayInTime)
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
                        )
                        .ThenInclude(t => t.Selections),
                (true, false) =>
                    usersQuery.Include(
                        u => u.Tickets
                            .Where(
                                t =>
                                    stateAt == null ||
                                        t.PayInTime <= stateAt
                            )
                            .OrderBy(t => t.PayInTime)
                    )
                        .ThenInclude(t => t.Status),
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

        public Account(
            Context context,
            ILogger<Account> logger,
            bool disposeMembers = false
        ) :
            base(context, logger, disposeMembers)
        {
        }

        private async Task<Transaction> CreateNewTransactionAsync(
            User user,
            DateTime time,
            bool deposit,
            decimal amount,
            CancellationToken cancellationToken
        )
        {
            TransactionType type;

            if (deposit)
            {
                type = await GetDepositTransactionTypeAsync(
                    _context,
                    cancellationToken
                )
                    .ConfigureAwait(false);

                user.Balance += amount;
            }
            else
            {
                type = await GetWithdrawalTransactionTypeAsync(
                    _context,
                    cancellationToken
                )
                    .ConfigureAwait(false);

                user.Balance -= amount;
            }

            var transaction = new Transaction()
            {
                User = user,
                Type = type,
                Time = time,
                Amount = amount
            };

            user.Transactions.Add(transaction);

            _context.Users.Update(user);

            return transaction;
        }

        public async Task<User> GetUserAsync(
            int id,
            DateTime? stateAt = null,
            bool includeTickets = false,
            bool includeTicketSelections = false,
            CancellationToken cancellationToken = default
        )
        {
            User user;

            _logger.LogDebug(
                "Fetching user from the database... Id: {id}, state at: {stateAt}, include tickets: {includeTickets}, include ticket selections: {includeTicketSelections}",
                    id,
                    stateAt,
                    includeTickets,
                    includeTicketSelections
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
                        includeTicketSelections
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
                        throw new InternalNotFoundException(
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
                    "Error while fetching user from the database. Id: { id}, state at: { stateAt}, include tickets: { includeTickets}, include ticket selections: { includeTicketSelections}",
                        id,
                        stateAt,
                        includeTickets,
                        includeTicketSelections
                );

                if (exception is not InternalException)
                {
                    throw new InternalServerErrorException(
                        null,
                        exception
                    );
                }

                throw;
            }

            _logger.LogInformation(
                "User successfully fetched from the database... Id: {id}, state at: {stateAt}, include tickets: {includeTickets}, include ticket selections: {includeTicketSelections}, username: {username}",
                    id,
                    stateAt,
                    includeTickets,
                    includeTicketSelections,
                    user.Username
            );

            return user;
        }

        public async Task<Transaction> MakeTransactionAsync(
            DateTime time,
            int userId,
            bool deposit,
            decimal amount,
            CancellationToken cancellationToken = default
        )
        {
            Transaction transaction;

            _logger.LogDebug(
                "Making new transaction in the database... Made at: {time}, user id: {userId}, deposit: {deposit}, amount: {amount:N2}",
                    time,
                    userId,
                    deposit,
                    amount
            );

            try
            {
                var dbTxn = await _context.Database
                    .BeginTransactionAsync(cancellationToken)
                    .ConfigureAwait(false);
                await using (dbTxn.ConfigureAwait(false))
                {
                    // Download user.
                    User user;
                    try
                    {
                        user = await GetUserByIdAsync(
                            userId,
                            cancellationToken
                        )
                            .ConfigureAwait(false);
                    }
                    catch (InvalidOperationException exception)
                    {
                        throw new InternalNotFoundException(
                            "The user does not exist.",
                            exception
                        );
                    }

                    // Ensure valid transaction amount. It must be in the allowed
                    // range of
                    // [`MinTransactionAmount`, `MaxTransactionAmount`], and,
                    // if it a withdrawal, it may not exceed the user's
                    // current balance.
                    amount = EnsureValidTransactionAmount(
                        deposit,
                        user.Balance,
                        amount
                    );

                    // Create new transaction.
                    transaction = await CreateNewTransactionAsync(
                        user,
                        time,
                        deposit,
                        amount,
                        cancellationToken
                    ).ConfigureAwait(false);

                    // Save changes into the database.
                    await _context.SaveChangesAsync(cancellationToken)
                        .ConfigureAwait(false);
                    await dbTxn.CommitAsync(cancellationToken)
                        .ConfigureAwait(false);
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
                    "Error while making new transaction in the database. Made at: {time}, user id: {userId}, deposit: {deposit}, amount: {amount:N2}",
                        time,
                        userId,
                        deposit,
                        amount
                );

                if (exception is not InternalException)
                {
                    throw new InternalServerErrorException(
                        null,
                        exception
                    );
                }

                throw;
            }

            _logger.LogInformation(
                "New transaction successfully made in the database... Made at: {time}, user id: {userId}, deposit: {deposit}, amount: {amount:N2}, transaction id: {id}",
                    time,
                    userId,
                    deposit,
                    amount,
                    transaction.Id
            );

            return transaction;
        }
    }
}
