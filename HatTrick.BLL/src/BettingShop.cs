using HatTrick.BLL.Exceptions;
using HatTrick.BLL.Models;
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
    public sealed class BettingShop : Business
    {
        private static void EnsureValidSelectionCount(
            ImmutableArray<int> selectionIds
        )
        {
            if (selectionIds.IsEmpty)
            {
                throw new InternalBadInputException(
                    $"No outcome is selected. At least {1:D} outcome must be selected."
                );
            }
            if (selectionIds.Length > MaxSelectionCount)
            {
                throw new InternalBadInputException(
                    $"Too many outcomes are selected. No more than {MaxSelectionCount:D} outcomes may be selected."
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
                throw new InternalBadInputException(
                    "An unavailable or non-existent outcome is selected."
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
                throw new InternalBadInputException(
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
            else if (selection.Odds >= PromoComboOddsThreshold)
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
                throw new InternalBadInputException(
                    $"Invalid promotion combination selected. If a promoted fixture is selected, at least {MinPromoCombos:D} non-promoted outcomes of odds greater than or equal to {PromoComboOddsThreshold:N2} must be selected, as well."
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
                throw new InternalBadInputException(
                    "Pay-in amount is negative."
                );
            }
            if (amount < MinBetAmount || amount > MaxBetAmount)
            {
                throw new InternalBadInputException(
                    $"Pay-in amount is out of range. Minimal allowed bet is {MinBetAmount:N2}, maximal allowed bet is {MaxBetAmount:N2}."
                );
            }
            if (amount > balance)
            {
                throw new InternalBadInputException(
                    $"Pay-in amount exceeds the current balance of {balance:N2}."
                );
            }

            return amount;
        }

        private static IQueryable<Ticket> IncludeSelections(
            IQueryable<Ticket> ticketsQuery
        ) =>
            ticketsQuery.Include(t => t.Selections);

        private static IQueryable<Ticket> IncludeSelectionTypes(
            IQueryable<Ticket> ticketsQuery
        ) =>
            ticketsQuery.Include(t => t.Selections)
                .ThenInclude(s => s.Type);

        private static IQueryable<Ticket> IncludeMarkets(
            IQueryable<Ticket> ticketsQuery
        ) =>
            ticketsQuery.Include(t => t.Selections)
                .ThenInclude(s => s.Market);

        private static IQueryable<Ticket> IncludeMarketTypes(
            IQueryable<Ticket> ticketsQuery
        ) =>
            ticketsQuery.Include(t => t.Selections)
                .ThenInclude(s => s.Market)
                .ThenInclude(m => m.Type);

        private static IQueryable<Ticket> IncludeFixtures(
            IQueryable<Ticket> ticketsQuery
        ) =>
            ticketsQuery.Include(t => t.Selections)
                .ThenInclude(s => s.Market)
                .ThenInclude(m => m.Fixture);

        private static IQueryable<Ticket> IncludeFixtureTypes(
            IQueryable<Ticket> ticketsQuery
        ) =>
            ticketsQuery.Include(t => t.Selections)
                .ThenInclude(s => s.Market)
                .ThenInclude(m => m.Fixture)
                .ThenInclude(f => f.Type);

        private static IQueryable<Ticket> IncludeEvents(
            IQueryable<Ticket> ticketsQuery
        ) =>
            ticketsQuery.Include(t => t.Selections)
                .ThenInclude(s => s.Market)
                .ThenInclude(m => m.Fixture)
                .ThenInclude(f => f.Event);

        private static IQueryable<Ticket> IncludeEventStatuses(
            IQueryable<Ticket> ticketsQuery
        ) =>
            ticketsQuery.Include(t => t.Selections)
                .ThenInclude(s => s.Market)
                .ThenInclude(m => m.Fixture)
                .ThenInclude(f => f.Event)
                .ThenInclude(e => e.Status);

        private static IQueryable<Ticket> IncludeSports(
            IQueryable<Ticket> ticketsQuery
        ) =>
            ticketsQuery.Include(t => t.Selections)
                .ThenInclude(s => s.Market)
                .ThenInclude(m => m.Fixture)
                .ThenInclude(f => f.Event)
                .ThenInclude(e => e.Sport);

        private static IQueryable<Ticket> Filter(
            IQueryable<Ticket> tickets,
            DateTime? stateAt = null,
            int? ticketId = null
        ) =>
            tickets.Where(
                t =>
                    (stateAt == null || t.PayInTime <= stateAt) &&
                        (ticketId == null || t.Id == ticketId)
            );

        public BettingShop(
            Context context,
            ILogger<BettingShop> logger,
            bool disposeMembers = false
        ) :
            base(context, logger, disposeMembers)
        {
        }

        private Task<Ticket> GetTicketByIdAsync(
            int id,
            DateTime? stateAt = null,
            CancellationToken cancellationToken = default
        ) =>
            _context.Tickets
                .Where(
                    t =>
                        (stateAt == null || t.PayInTime <= stateAt) &&
                            t.Id == id
                )
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

            user.Balance -= transaction.Amount;

            _context.Users.Update(user);

            return (ticket, transaction);
        }

        public async Task<Ticket> PlaceBetAsync(
            DateTime placedAt,
            int userId,
            ImmutableArray<int> selectionIds,
            decimal amount,
            CancellationToken cancellationToken = default
        )
        {
            Ticket ticket;

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
                        throw new InternalNotFoundException(
                            "The user does not exist.",
                            exception
                        );
                    }

                    // Ensure valid pay-in amount. It must be in the allowed
                    // range of [`MinBetAmount`, `MaxBetAmount`], and it may
                    // not exceed the user's current balance.
                    amount = EnsureValidPayInAmount(user.Balance, amount);

                    // Map selection ids to actual outcome entries in the
                    // database.
                    var selections = await MapSelectionIdsAsync(
                        placedAt,
                        selectionIds,
                        cancellationToken
                    )
                        .ConfigureAwait(false);

                    // Evaluate selections.
                    var totalOdds = EvaluateSelections(selectionIds, selections);

                    // Create new ticket and the corresponding transaction.
                    (ticket, var transaction) = await CreateNewBetAsync(
                        user,
                        selections.Values.ToList(),
                        placedAt,
                        amount,
                        totalOdds,
                        cancellationToken
                    )
                        .ConfigureAwait(false);

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
                    throw new InternalServerErrorException(
                        null,
                        exception
                    );
                }

                throw;
            }

            _logger.LogInformation(
                "New bet successfully placed into the database. Placed at: {placedAt}, user id: {userId}, selection ids: {@selectionIds}, amount: {amount:N2}, ticket id: {id}",
                    placedAt,
                    userId,
                    selectionIds.AsEnumerable(),
                    amount,
                    ticket.Id
            );

            return ticket;
        }

        public async Task<Ticket> GetTicketAsync(
            int id,
            DateTime? stateAt = null,
            bool includeSelections = false,
            CancellationToken cancellationToken = default
        )
        {
            Ticket ticket;

            _logger.LogDebug(
                "Fetching ticket from the database... Id: {id}, state at: {stateAt}, include selections: {includeSelections}",
                    id,
                    stateAt,
                    includeSelections
            );

            try
            {
                var dbTxn = await _context.Database
                    .BeginTransactionAsync(cancellationToken)
                    .ConfigureAwait(false);
                await using (dbTxn.ConfigureAwait(false))
                {
                    // Initialise query.
                    IQueryable<Ticket> ticketQuery = _context.Tickets;

                    // Include all requested and related entities.
                    ticketQuery = ticketQuery.Include(t => t.Status);
                    if (includeSelections)
                    {
                        ticketQuery = ticketQuery.Include(
                            t => t.Selections
                                .OrderBy(s => s.Type.Priority)
                                .ThenBy(s => s.Type.Name)
                                .ThenBy(s => s.Value)
                                //.ThenByDescending(s => s.Odds) // Not supported in SQLite: http://learn.microsoft.com/en-gb/ef/core/providers/sqlite/limitations#query-limitations
                                .ThenBy(s => s.AvailableUntil)
                                .ThenByDescending(s => s.AvailableFrom)
                        );
                    }

                    // Filter.
                    ticketQuery = ticketQuery.Where(
                        t =>
                            (stateAt == null || t.PayInTime <= stateAt) &&
                                t.Id == id
                    );

                    // Download ticket.
                    try
                    {
                        ticket = await ticketQuery.AsSplitQuery()
                            .AsNoTrackingWithIdentityResolution()
                            .SingleAsync(cancellationToken)
                            .ConfigureAwait(false);
                    }
                    catch (InvalidOperationException exception)
                    {
                        throw new InternalNotFoundException(
                            "The ticket does not exist.",
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
                    "Error while fetching ticket from the database. Id: {id}, state at: {stateAt}, include selections: {includeSelections}",
                        id,
                        stateAt,
                        includeSelections
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
                "Ticket successfully fetched from the database... Id: {id}, state at: {stateAt}, include selections: {includeTicketSelections}, ticket status: {status}",
                    id,
                    stateAt,
                    includeSelections,
                    ticket.Status.Name
            );

            return ticket;
        }

        public async Task<Event[]> GetTicketSelectionsAsync(
            int ticketId,
            DateTime? stateAt = null,
            CancellationToken cancellationToken = default
        )
        {
            Event[] events;

            _logger.LogDebug(
                "Fetching ticket selections from the database... Ticket id: {id}, state at: {stateAt}",
                    ticketId,
                    stateAt
            );

            try
            {
                var dbTxn = await _context.Database
                        .BeginTransactionAsync(cancellationToken)
                        .ConfigureAwait(false);
                await using (dbTxn.ConfigureAwait(false))
                {
                    var exists = await _context.Tickets
                        .Where(
                            t =>
                                (stateAt == null || t.PayInTime <= stateAt) &&
                                    t.Id == ticketId
                        )
                        .AnyAsync(cancellationToken)
                        .ConfigureAwait(false);

                    if (!exists)
                    {
                        throw new InternalNotFoundException(
                            "The ticket does not exist."
                        );
                    }

                    // Initialise query.
                    IQueryable<Ticket> ticketQuery = _context.Tickets;

                    // Include all related entities.
                    ticketQuery = IncludeSelections(ticketQuery);
                    ticketQuery = IncludeSelectionTypes(ticketQuery);
                    ticketQuery = IncludeMarkets(ticketQuery);
                    ticketQuery = IncludeMarketTypes(ticketQuery);
                    ticketQuery = IncludeFixtures(ticketQuery);
                    ticketQuery = IncludeFixtureTypes(ticketQuery);
                    ticketQuery = IncludeEvents(ticketQuery);
                    ticketQuery = IncludeEventStatuses(ticketQuery);
                    ticketQuery = IncludeSports(ticketQuery);

                    // Filter.
                    ticketQuery = Filter(ticketQuery, stateAt, ticketId);

                    // Sort and download data.
                    var selections = ticketQuery.SelectMany(t => t.Selections)
                        .OrderByDescending(s => s.Market.Fixture.Event.StartsAt)
                        .ThenBy(s => s.Market.Fixture.Event.EndsAt)
                        .ThenBy(s => s.Market.Fixture.Event.Priority)
                        .ThenBy(s => s.Market.Fixture.Event.Sport.Priority)
                        .ThenBy(s => s.Market.Fixture.Event.Name)
                        .AsSplitQuery()
                        .AsNoTrackingWithIdentityResolution()
                        .AsAsyncEnumerable();

                    // Format data from top-level being event.
                    events = await selections.Select(s => s.Market.Fixture.Event)
                        .ToArrayAsync(cancellationToken)
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
                    "Error while fetching ticket selections from the database. Ticket id: {id}, state at: {stateAt}",
                        ticketId,
                        stateAt
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
                "Ticket selections successfully fetched from the database... Ticket id: {id}, state at: {stateAt}, selection count: {count}",
                    ticketId,
                    stateAt,
                    events.Length
            );

            return events;
        }

        public async Task<TicketFinancialAmounts> CalculateTicketFinancialAmountsAsync(
            int ticketId,
            DateTime? stateAt = null,
            CancellationToken cancellationToken = default
        )
        {
            decimal payInAmount;
            decimal activeAmount;
            decimal totalOdds;
            decimal grossPotentialWinAmount;
            decimal tax;
            decimal netPotentialWinAmount;

            _logger.LogDebug(
                "Calculating ticket financial amounts... Ticket id: {ticketId}, state at: {stateAt}",
                    ticketId,
                    stateAt
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
                            stateAt,
                            cancellationToken
                        )
                            .ConfigureAwait(false);
                    }
                    catch (InvalidOperationException exception)
                    {
                        throw new InternalNotFoundException(
                            "The ticket does not exist.",
                            exception
                        );
                    }

                    var taxGrades = await GetTaxGradesAsync(_context, cancellationToken)
                        .ConfigureAwait(false);

                    payInAmount = ticket.PayInAmount;
                    totalOdds = ticket.TotalOdds;
                    grossPotentialWinAmount = CalculateCostAmount(payInAmount, totalOdds, out activeAmount);
                    tax = CalculateTax(taxGrades, grossPotentialWinAmount);
                    netPotentialWinAmount = grossPotentialWinAmount - tax;
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
                    "Error while calculating ticket financial amounts. Ticket id: {ticketId}, state at: {stateAt}",
                        ticketId,
                        stateAt
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

            var amounts = new TicketFinancialAmounts()
            {
                PayInAmount = payInAmount,
                ActiveAmount = activeAmount,
                TotalOdds = totalOdds,
                GrossPotentialWinAmount = grossPotentialWinAmount,
                Tax = tax,
                NetPotentialWinAmount = netPotentialWinAmount
            };

            _logger.LogInformation(
                "Ticket financial amounts successfully calculated. Ticket id: {ticketId}, state at: {stateAt}, amounts: {@amounts}",
                    ticketId,
                    stateAt,
                    amounts
            );

            return amounts;
        }
    }
}
