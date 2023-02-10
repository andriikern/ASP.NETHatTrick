using HatTrick.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace HatTrick.API.DatabaseInitialisation
{
    internal static class Initialiser
    {
        public static async Task InitialiseAsync(
            Context dbContext,
            ILogger? logger = null,
            CancellationToken cancellationToken = default
        )
        {
            logger?.LogDebug("Checking if the database is created; creating if needed...");

            await dbContext.Database
                .EnsureCreatedAsync(cancellationToken)
                .ConfigureAwait(false);

            logger?.LogDebug("Checking if the database is filled with data...");

            var dbTxn = await dbContext.Database
                .BeginTransactionAsync(cancellationToken)
                .ConfigureAwait(false);
            await using (dbTxn.ConfigureAwait(false))
            {
                // The `Events` table is chosen as the reference table to
                // check if there are any data in the database.  However,
                // almost any other table could have been chosen instead, as
                // it is expected that either all or none of the sample data
                // were inserted during initialisation.
                bool hasData = await dbContext.Events
                    .AnyAsync(cancellationToken)
                    .ConfigureAwait(false);

                if (hasData)
                {
                    logger?.LogDebug("Database is already filled.");

                    return;
                }

                logger?.LogDebug("Inserting sample data into the database...");

                await dbContext.Sports
                    .AddRangeAsync(SampleData.Sports, cancellationToken)
                    .ConfigureAwait(false);

                await dbContext.EventStatuses
                    .AddRangeAsync(SampleData.EventStatuses, cancellationToken)
                    .ConfigureAwait(false);

                await dbContext.FixtureTypes
                    .AddRangeAsync(SampleData.FixtureTypes, cancellationToken)
                    .ConfigureAwait(false);

                await dbContext.MarketTypes
                    .AddRangeAsync(SampleData.MarketTypes, cancellationToken)
                    .ConfigureAwait(false);

                await dbContext.OutcomeTypes
                    .AddRangeAsync(SampleData.OutcomeTypes, cancellationToken)
                    .ConfigureAwait(false);

                await dbContext.TicketStatuses
                    .AddRangeAsync(SampleData.TicketStatuses, cancellationToken)
                    .ConfigureAwait(false);

                await dbContext.TransactionTypes
                    .AddRangeAsync(SampleData.TransactionTypes, cancellationToken)
                    .ConfigureAwait(false);

                await dbContext.Users
                    .AddRangeAsync(SampleData.Users, cancellationToken)
                    .ConfigureAwait(false);

                await dbContext.Events
                    .AddRangeAsync(SampleData.Events, cancellationToken)
                    .ConfigureAwait(false);

                await dbContext.Fixtures
                    .AddRangeAsync(SampleData.Fixtures, cancellationToken)
                    .ConfigureAwait(false);

                await dbContext.Markets
                    .AddRangeAsync(SampleData.Markets, cancellationToken)
                    .ConfigureAwait(false);

                await dbContext.Outcomes
                    .AddRangeAsync(SampleData.Outcomes, cancellationToken)
                    .ConfigureAwait(false);

                await dbContext.SaveChangesAsync(cancellationToken)
                    .ConfigureAwait(false);
                await dbTxn.CommitAsync(cancellationToken)
                    .ConfigureAwait(false);
            }

            logger?.LogDebug("Sample data successfully inserted into the database.");
        }
    }
}
