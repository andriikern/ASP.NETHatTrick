using HatTrick.BLL.Exceptions;
using HatTrick.DAL;
using HatTrick.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HatTrick.BLL
{
    public sealed class BettingShop : IDisposable, IAsyncDisposable
    {
        public const int MaxSelectionCount = 70;

        public const decimal PromoComboOddsThreshold = 1.10M;
        public const int MinPromoCombos = 5;

        public const decimal MinBetAmount = 0.25M;
        public const decimal MaxBetAmount = 250_000.00M;

        public const decimal ManipulativeCost = 0.05M;

        private static decimal Round(
            decimal odds
        ) =>
            decimal.Round(odds, 2);

        private static decimal CalculateActiveAmount(
            decimal payInAmount
        ) =>
            (decimal.One - ManipulativeCost) * payInAmount;

        private static decimal CalculateCostAmount(
            decimal payInAmount,
            decimal totalOdds
        ) =>
            Round(totalOdds * CalculateActiveAmount(payInAmount));

        private static decimal CalculateTax(
            IEnumerable<TaxGrade> taxGrades,
            decimal costAmount
        )
        {
            var tax = decimal.Zero;

            foreach (var taxGrade in taxGrades)
            {
                if (
                    taxGrade.LowerBound.HasValue &&
                    costAmount < taxGrade.LowerBound
                )
                {
                    continue;
                }

                var taxedAmount =
                    Math.Min(
                        costAmount,
                        taxGrade.UpperBound
                            .GetValueOrDefault(costAmount)
                    ) -
                        taxGrade.LowerBound
                            .GetValueOrDefault(decimal.Zero);

                tax += taxGrade.Rate * taxedAmount;
            }

            tax = Round(tax);

            return tax;
        }

        private static Task<TaxGrade[]> GetTaxGradesAsync(
            Context context,
            CancellationToken cancellationToken = default
        ) =>
            context.TaxGrades
                .ToArrayAsync(cancellationToken);

        private static Task<TicketStatus> GetTicketStatusByNameAsync(
            Context context,
            string name,
            CancellationToken cancellationToken = default
        ) =>
            context.TicketStatuses
                .Where(t => t.Name == name)
                .SingleAsync(cancellationToken);

        private static Task<TicketStatus> GetActiveTicketStatusAsync(
            Context context,
            CancellationToken cancellationToken = default
        ) =>
            GetTicketStatusByNameAsync(
                context,
                "Active",
                cancellationToken
            );

        private static Task<TransactionType> GetTransactionTypeByNameAsync(
            Context context,
            string name,
            CancellationToken cancellationToken = default
        ) =>
            context.TransactionTypes
                .Where(t => t.Name == name)
                .SingleAsync(cancellationToken);

        private static Task<TransactionType> GetPayInTransactionTypeAsync(
            Context context,
            CancellationToken cancellationToken = default
        ) =>
            GetTransactionTypeByNameAsync(
                context,
                "Pay-in",
                cancellationToken
            );

        private static Task<TransactionType> GetPayOutTransactionTypeAsync(
            Context context,
            CancellationToken cancellationToken = default
        ) =>
            GetTransactionTypeByNameAsync(
                context,
                "Pay-out",
                cancellationToken
            );

        private static void EnsureValidSelectionCount(
            ImmutableArray<int> selectionIds
        )
        {
            if (selectionIds.IsEmpty)
            {
                throw new InternalException(
                    $"No outcome is selected. At least {1} outcome must be selected."
                );
            }
            if (selectionIds.Length > MaxSelectionCount)
            {
                throw new InternalException(
                    $"Too many outcomes are selected. No more than {MaxSelectionCount} outcomes may be selected."
                );
            }
        }

        private static Outcome EnsureSelectionIsAvailable(
            IDictionary<int, Outcome> selections,
            int selectionId
        )
        {
            if (!selections.TryGetValue(selectionId, out var selection))
            {
                throw new InternalException(
                    $"An unavailable or non-existent outcome is selected."
                );
            }

            return selection;
        }

        private static void EnsureUniqueSelectionEvent(
            ISet<int> selectedEventIds,
            Outcome selection
        )
        {
            if (!selectedEventIds.Add(selection.Market.Fixture.Event.Id))
            {
                throw new InternalException(
                    "Duplicate events are selected. Each outcome must belong to a unique event; no event may be selected more than once."
                );
            }
        }

        private static void UpdateOdds(
            Outcome selection,
            ref decimal totalOdds,
            ref bool promoted,
            ref int promoCombinations
        )
        {
            if (selection.Market.Fixture.Type.IsPromoted)
            {
                promoted = true;
            }
            else if (selection.Odds > PromoComboOddsThreshold)
            {
                ++promoCombinations;
            }

            totalOdds *= (decimal)selection.Odds!;
        }

        private static void EnsureValidPromoSelections(
            bool promoted,
            int promoCombinations
        )
        {
            if (promoted && promoCombinations < MinPromoCombos)
            {
                throw new InternalException(
                    $"Invalid promotion combination selected. If a promoted fixture is selected, at least {MinPromoCombos} non-promoted outcomes of odds {PromoComboOddsThreshold} or higher must be selected, as well."
                );
            }
        }

        private static decimal EvaluateSelections(
            ImmutableArray<int> selectionIds,
            IDictionary<int, Outcome> selections
        )
        {
            EnsureValidSelectionCount(selectionIds);

            decimal totalOdds = decimal.One;

            bool promoted = false;
            var selectedEventIds = new HashSet<int>();
            int promoCombinations = 0;

            foreach (int selectionId in selectionIds)
            {
                var selection = EnsureSelectionIsAvailable(selections, selectionId);
                EnsureUniqueSelectionEvent(selectedEventIds, selection);

                UpdateOdds(
                    selection,
                    ref totalOdds,
                    ref promoted,
                    ref promoCombinations
                );
            }

            EnsureValidPromoSelections(promoted, promoCombinations);

            totalOdds = Round(totalOdds);

            return totalOdds;
        }

        private static decimal EnsureValidPayInAmount(
            decimal balance,
            decimal amount
        )
        {
            amount = Round(amount);

            if (amount < decimal.Zero)
            {
                throw new InternalException(
                    "Pay-in amount is negative."
                );
            }
            if (amount < MinBetAmount || amount > MaxBetAmount)
            {
                throw new InternalException(
                    $"Pay-in amount is out of range. Minimal allowed bet is {MinBetAmount:N2}, maximal allowed bet is {MaxBetAmount:N2}"
                );
            }
            if (amount > balance)
            {
                throw new InternalException(
                    $"Pay-in amount exceeds the current balance of {balance:N2}."
                );
            }

            return amount;
        }

        private readonly bool _disposeMembers;
        private readonly Context _context;
        private readonly ILogger<BettingShop> _logger;

        private bool disposed;

        public BettingShop(
            Context context,
            ILogger<BettingShop> logger,
            bool disposeMembers = true
        )
        {
            _disposeMembers = disposeMembers;
            _context = context;
            _logger = logger;

            disposed = false;
        }

        private Task<User> GetUserByIdAsync(
            int id,
            CancellationToken cancellationToken = default
        ) =>
            _context.Users
                .Where(t => t.Id == id)
                .SingleAsync(cancellationToken);

        private Task<Ticket> GetTicketByIdAsync(
            int id,
            CancellationToken cancellationToken = default
        ) =>
            _context.Tickets
                .Where(t => t.Id == id)
                .SingleAsync(cancellationToken);

        private Task<Dictionary<int, Outcome>> MapSelectionIdsAsync(
            DateTime selectedAt,
            ImmutableArray<int> selectionIds,
            CancellationToken cancellationToken = default
        ) =>
            _context.Outcomes
                .Include(s => s.Market)
                .ThenInclude(m => m.Fixture)
                .ThenInclude(f => f.Type)
                .Include(s => s.Market)
                .ThenInclude(m => m.Fixture)
                .ThenInclude(f => f.Event)
                .Where(
                    s =>
                        s.AvailableFrom <= selectedAt &&
                            s.AvailableUntil > selectedAt &&
                            selectionIds.Contains(s.Id) &&
                            s.Odds != null
                )
                .AsSplitQuery()
                .ToDictionaryAsync(o => o.Id, cancellationToken);

        private async Task<(Ticket ticket, Transaction transaction)> CreateNewBetAsync(
            User user,
            ICollection<Outcome> selections,
            DateTime placedAt,
            decimal amount,
            decimal totalOdds,
            CancellationToken cancellationToken = default
        )
        {
            var activeTicketStatus = await GetActiveTicketStatusAsync(
                    _context,
                    cancellationToken
                )
                    .ConfigureAwait(false);

            var transactionType = await GetPayInTransactionTypeAsync(
                _context,
                cancellationToken
            )
                .ConfigureAwait(false);

            var ticket = new Ticket()
            {
                User = user,
                Selections = selections,
                PayInAmount = amount,
                PayInTime = placedAt,
                TotalOdds = totalOdds,
                Status = activeTicketStatus,
                IsResolved = false
            };
            var transaction = new Transaction()
            {
                User = user,
                Type = transactionType,
                Ticket = ticket,
                Time = placedAt,
                Amount = amount
            };

            ticket.Transactions.Add(transaction);

            user.Tickets.Add(ticket);
            user.Transactions.Add(transaction);

            return (ticket, transaction);
        }

        private void SaveBet(
            User user,
            Transaction transaction
        )
        {
            user.Balance = Math.Max(
                user.Balance - transaction.Amount,
                decimal.Zero
            );
            _context.Users.Update(user);
        }

        public async Task<int[]> GetSelectionEventIdsAsync(
            DateTime selectedAt,
            ImmutableArray<int> selectionIds,
            CancellationToken cancellationToken = default
        )
        {
            _logger.LogDebug(
                "Fetching event ids of selections from the database... Selected at: {selectedAt}, selectionIds: {selectionIds}",
                    selectedAt,
                    selectionIds.AsEnumerable()
            );

            var eventIds = await _context.Outcomes
                .Where(
                    s =>
                        s.AvailableFrom <= selectedAt &&
                            s.AvailableUntil > selectedAt &&
                            selectionIds.Contains(s.Id)
                )
                .Select(s => s.Market.Fixture.Event.Id)
                .Distinct()
                .ToArrayAsync(cancellationToken)
                .ConfigureAwait(false);

            _logger.LogInformation(
                "Event ids of selections successfully fetched from the database... Selected at: {selectedAt}, selectionIds: {selectionIds}, event count: {count}",
                    selectedAt,
                    selectionIds.AsEnumerable(),
                    eventIds.Length
            );

            return eventIds;
        }

        public async Task<int> PlaceBetAsync(
            DateTime placedAt,
            int userId,
            ImmutableArray<int> selectionIds,
            decimal amount,
            CancellationToken cancellationToken = default
        )
        {
            int id;

            _logger.LogDebug(
                "Placing new bet into the database... Placed at: {placedAt}, user id: {userId}, selection ids: {@selectionIds}, amount: {amount:N2}",
                    placedAt,
                    userId,
                    selectionIds.AsEnumerable(),
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
                        throw new InternalException(
                            "The user does not exist.",
                            exception
                        );
                    }

                    // Ensure valid pay-in amount. It must be in the allowed
                    // range of [`MinBetAmount`, `MaxBetAmount`], and it may
                    // not exceed the current user's balance.
                    amount = EnsureValidPayInAmount(user.Balance, amount);

                    var selections = await MapSelectionIdsAsync(
                        placedAt,
                        selectionIds,
                        cancellationToken
                    )
                        .ConfigureAwait(false);

                    var totalOdds = EvaluateSelections(selectionIds, selections);

                    var (ticket, transaction) = await CreateNewBetAsync(
                        user,
                        selections.Values.ToList(),
                        placedAt,
                        amount,
                        totalOdds,
                        cancellationToken
                    )
                        .ConfigureAwait(false);

                    SaveBet(
                        user,
                        transaction
                    );

                    await _context.SaveChangesAsync(cancellationToken)
                        .ConfigureAwait(false);
                    await dbTxn.CommitAsync(cancellationToken)
                        .ConfigureAwait(false);

                    id = ticket.Id;
                }
            }
            catch (Exception exception)
                when (
                    exception is InvalidOperationException ||
                    exception is DbUpdateException ||
                    exception is InternalException
                )
            {
                _logger.LogError(
                    exception,
                    "Error while placing new bet into the database. Placed at: {placedAt}, user id: {userId}, selection ids: {@selectionIds}, amount: {amount:N2}",
                        placedAt,
                        userId,
                        selectionIds.AsEnumerable(),
                        amount
                );

                if (exception is not InternalException)
                {
                    throw new InternalException(null, exception);
                }

                throw;
            }

            _logger.LogInformation(
                "New bet successfully placed into the database. Placed at: {placedAt}, user id: {userId}, selection ids: {@selectionIds}, amount: {amount:N2}, ticket id: {ticketId}",
                    placedAt,
                    userId,
                    selectionIds.AsEnumerable(),
                    amount,
                    id
            );

            return id;
        }

        public async Task<(decimal costAmount, decimal winAmount, decimal tax)> CalculateTicketOutputAmountsAsync(
            int ticketId,
            CancellationToken cancellationToken = default
        )
        {
            decimal costAmount;
            decimal winAmount;
            decimal tax;

            _logger.LogDebug(
                "Calculating ticket cost amount, win amount, and tax... Ticket id: {ticketId}",
                    ticketId
            );

            try
            {
                var dbTxn = await _context.Database
                    .BeginTransactionAsync(cancellationToken)
                    .ConfigureAwait(false);
                await using (dbTxn.ConfigureAwait(false))
                {
                    Ticket ticket;
                    try
                    {
                        ticket = await GetTicketByIdAsync(
                            ticketId,
                            cancellationToken
                        )
                            .ConfigureAwait(false);
                    }
                    catch (InvalidOperationException exception)
                    {
                        throw new InternalException(
                            "The ticket does not exist.",
                            exception
                        );
                    }

                    var taxGrades = await GetTaxGradesAsync(_context, cancellationToken)
                        .ConfigureAwait(false);

                    costAmount = CalculateCostAmount(ticket.PayInAmount, ticket.TotalOdds);
                    tax = CalculateTax(taxGrades, costAmount);
                    winAmount = costAmount - tax;
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
                    "Error while calculating ticket cost amount, win amount, and tax. Ticket id: {ticketId}",
                        ticketId
                );

                if (exception is not InternalException)
                {
                    throw new InternalException(null, exception);
                }

                throw;
            }

            _logger.LogInformation(
                "Ticket cost amount, win amount, and tax successfully calculated. Ticket id: {ticketId}, cost amount: {costAmount:N2}, win amount: {winAmount:N2}, tax: {tax:N2}",
                    ticketId
            );

            return (costAmount, winAmount, tax);
        }

        private void Dispose(
            bool disposing
        )
        {
            if (disposing && !disposed)
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
            if (disposing && !disposed)
            {
                if (_disposeMembers)
                {
                    await _context.DisposeAsync()
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

        ~BettingShop()
        {
            Dispose(false);
        }
    }
}
