using HatTrick.BLL.Exceptions;
using HatTrick.DAL;
using HatTrick.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HatTrick.BLL
{
    public sealed class Offer : Business
    {
        private static IQueryable<Event> IncludeSports(
            IQueryable<Event> eventsQuery
        ) =>
            eventsQuery.Include(e => e.Sport);

        private static IQueryable<Event> IncludeStatuses(
            IQueryable<Event> eventsQuery
        ) =>
            eventsQuery.Include(e => e.Status);

        private static IQueryable<Event> IncludeFixtures(
            IQueryable<Event> eventsQuery,
            DateTime? availableAt = null,
            bool? promoted = null
        ) =>
            eventsQuery.Include(
                e => e.Fixtures
                    .Where(
                        f =>
                            (
                                availableAt == null ||
                                    (
                                        f.AvailableFrom <= availableAt &&
                                            f.AvailableUntil > availableAt
                                    )
                            ) &&
                                (promoted == null || f.Type.IsPromoted == promoted)
                    )
                    .OrderBy(f => f.Type.Priority)
                    .ThenBy(f => f.Type.Name)
            );

        private static IQueryable<Event> IncludeFixtureTypes(
            IQueryable<Event> eventsQuery
        ) =>
            eventsQuery.Include(e => e.Fixtures)
                .ThenInclude(f => f.Type);

        private static IQueryable<Event> IncludeMarkets(
            IQueryable<Event> eventsQuery,
            DateTime? availableAt = null
        ) =>
            eventsQuery.Include(e => e.Fixtures)
                .ThenInclude(
                    f => f.Markets
                        .Where(
                            m =>
                                availableAt == null ||
                                    (
                                        m.AvailableFrom <= availableAt &&
                                            m.AvailableUntil > availableAt
                                    )
                        )
                        .OrderBy(m => m.Type.Priority)
                        .ThenBy(m => m.Type.Name)
                        .ThenBy(m => m.Value)
                        .ThenBy(m => m.AvailableUntil)
                        .ThenByDescending(m => m.AvailableFrom)
                );

        private static IQueryable<Event> IncludeMarketTypes(
            IQueryable<Event> eventsQuery
        ) =>
            eventsQuery.Include(e => e.Fixtures)
                .ThenInclude(f => f.Markets)
                .ThenInclude(m => m.Type);

        private static IQueryable<Event> IncludeOutcomes(
            IQueryable<Event> eventsQuery,
            DateTime? availableAt = null
        ) =>
            eventsQuery.Include(e => e.Fixtures)
                .ThenInclude(f => f.Markets)
                .ThenInclude(
                    m => m.Outcomes
                        .Where(
                            o =>
                                availableAt == null ||
                                    (
                                        o.AvailableFrom <= availableAt &&
                                            o.AvailableUntil > availableAt
                                    )
                        )
                        .OrderBy(o => o.Type.Priority)
                        .ThenBy(o => o.Type.Name)
                        .ThenBy(o => o.Value)
                        //.ThenByDescending(o => o.Odds) // Not supported in SQLite: http://learn.microsoft.com/en-gb/ef/core/providers/sqlite/limitations/#query-limitations
                        .ThenBy(o => o.AvailableUntil)
                        .ThenByDescending(o => o.AvailableFrom)
                );

        private static IQueryable<Event> IncludeOutcomeTypes(
            IQueryable<Event> eventsQuery
        ) =>
            eventsQuery.Include(e => e.Fixtures)
                .ThenInclude(f => f.Markets)
                .ThenInclude(m => m.Outcomes)
                .ThenInclude(o => o.Type);

        private static IQueryable<Event> FilterAndSort(
            IQueryable<Event> eventsQuery,
            DateTime? availableAt = null,
            bool? promoted = false,
            int skip = 0,
            int take = DefaultTakeN
        ) =>
            eventsQuery.Where(
                e =>
                    (availableAt == null || e.EndsAt > availableAt) &&
                        !_ignoreEventStatuses.Contains(e.Status.Name) &&
                        e.Fixtures.Any(
                            f =>
                                (
                                    availableAt == null ||
                                        (
                                            f.AvailableFrom <= availableAt &&
                                                f.AvailableUntil > availableAt
                                        )
                                ) &&
                                    (promoted == null || f.Type.IsPromoted == promoted)
                        )
            )
                .OrderBy(e => e.StartsAt.Date)
                .ThenBy(e => e.Priority)
                .ThenBy(e => e.Sport.Priority)
                .ThenBy(e => e.StartsAt)
                .ThenBy(e => e.Name)
                .ThenBy(e => e.EndsAt)
                .Skip(skip)
                .Take(take);

        public Offer(
            Context context,
            ILogger<Offer> logger,
            bool disposeMembers = false
        ) :
            base(context, logger, disposeMembers)
        {
        }

        public async Task<Event[]> GetEventsAsync(
            DateTime? availableAt = null,
            bool? promoted = null,
            int skip = 0,
            int take = DefaultTakeN,
            CancellationToken cancellationToken = default
        )
        {
            _logger.LogTrace(
                "Fetching events from the database... Available at: {availableAt}, promoted: {promoted}, skip: {skip}, take: {take}",
                    availableAt,
                    promoted,
                    skip,
                    take
            );

            Event[] events;

            try
            {
                var dbTxn = await _context.Database
                    .BeginTransactionAsync(cancellationToken)
                    .ConfigureAwait(false);
                await using (dbTxn.ConfigureAwait(false))
                {
                    // Initialise query.
                    IQueryable<Event> eventsQuery = _context.Events;

                    // Include all related entities.
                    eventsQuery = IncludeSports(eventsQuery);
                    eventsQuery = IncludeStatuses(eventsQuery);
                    eventsQuery = IncludeFixtures(eventsQuery, availableAt, promoted);
                    eventsQuery = IncludeFixtureTypes(eventsQuery);
                    eventsQuery = IncludeMarkets(eventsQuery, availableAt);
                    eventsQuery = IncludeMarketTypes(eventsQuery);
                    eventsQuery = IncludeOutcomes(eventsQuery, availableAt);
                    eventsQuery = IncludeOutcomeTypes(eventsQuery);

                    // Filter and sort.
                    eventsQuery = FilterAndSort(
                        eventsQuery,
                        availableAt,
                        promoted,
                        skip,
                        take
                    );

                    // Download events.
                    events = await eventsQuery.AsSplitQuery()
                        .AsNoTrackingWithIdentityResolution()
                        .ToArrayAsync(cancellationToken)
                        .ConfigureAwait(false);
                }
            }
            catch (Exception exception)
                when (
                    exception is InvalidOperationException ||
                    exception is DbException ||
                    exception is DbUpdateException ||
                    exception is InternalException
                )
            {
                _logger.LogError(
                    exception,
                    "Error while fetching events from the database. Available at: {availableAt}, promoted: {promoted}, skip: {skip}, take: {take}",
                        availableAt,
                        promoted,
                        skip,
                        take
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
                "Events successfully fetched from the database... Available at: {availableAt}, promoted: {promoted}, skip: {skip}, take: {take}, event count: {count}",
                    availableAt,
                    promoted,
                    skip,
                    take,
                    events.Length
            );

            return events;
        }
    }
}
