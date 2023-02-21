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
    public class Business : IDisposable, IAsyncDisposable
    {
        public const int DefaultTakeN = 1000;

        public const int MaxSelectionCount = 70;

        public const decimal PromoComboOddsThreshold = 1.10M;
        public const int MinPromoCombos = 5;

        public const decimal MinBetAmount = 0.25M;
        public const decimal MaxBetAmount = 250_000.00M;

        public const decimal MinTransactionAmount = 1.00M;
        public const decimal MaxTransactionAmount = 250_000.00M;

        public const decimal ManipulativeCostRate = 0.05M;

        protected static readonly ImmutableArray<string> _ignoreEventStatuses =
            ImmutableArray.Create(
                "Rescheduled",
                "Cancelled"
            );

        protected static decimal Round(
            decimal value
        ) =>
            decimal.Round(value, 2);

        protected static decimal CalculateActiveAmount(
            decimal payInAmount
        ) =>
            (decimal.One - ManipulativeCostRate) * payInAmount;

        protected static decimal CalculateCostAmount(
            decimal payInAmount,
            decimal totalOdds,
            out decimal activeAmount
        )
        {
            activeAmount = CalculateActiveAmount(payInAmount);

            return Round(totalOdds * CalculateActiveAmount(payInAmount));
        }

        protected static decimal CalculateCostAmount(
            decimal payInAmount,
            decimal totalOdds
        ) =>
            CalculateCostAmount(payInAmount, totalOdds, out var _);

        protected static decimal CalculateTax(
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

        protected static Task<TaxGrade[]> GetTaxGradesAsync(
            Context context,
            CancellationToken cancellationToken = default
        ) =>
            context.TaxGrades
                .ToArrayAsync(cancellationToken);

        protected static Task<TicketStatus> GetTicketStatusByIdAsync(
            Context context,
            int id,
            CancellationToken cancellationToken = default
        ) =>
            context.TicketStatuses
                .Where(t => t.Id == id)
                .SingleAsync(cancellationToken);

        protected static Task<TicketStatus> GetTicketStatusByNameAsync(
            Context context,
            string name,
            CancellationToken cancellationToken = default
        ) =>
            context.TicketStatuses
                .Where(t => t.Name == name)
                .SingleAsync(cancellationToken);

        protected static Task<TicketStatus> GetActiveTicketStatusAsync(
            Context context,
            CancellationToken cancellationToken = default
        ) =>
            GetTicketStatusByIdAsync(
                context,
                1,
                cancellationToken
            );

        protected static Task<TransactionType> GetTransactionTypeByIdAsync(
            Context context,
            int id,
            CancellationToken cancellationToken = default
        ) =>
            context.TransactionTypes
                .Where(t => t.Id == id)
                .SingleAsync(cancellationToken);

        protected static Task<TransactionType> GetTransactionTypeByNameAsync(
            Context context,
            string name,
            CancellationToken cancellationToken = default
        ) =>
            context.TransactionTypes
                .Where(t => t.Name == name)
                .SingleAsync(cancellationToken);

        protected static Task<TransactionType> GetDepositTransactionTypeAsync(
            Context context,
            CancellationToken cancellationToken = default
        ) =>
            GetTransactionTypeByIdAsync(
                context,
                1,
                cancellationToken
            );

        protected static Task<TransactionType> GetWithdrawalTransactionTypeAsync(
            Context context,
            CancellationToken cancellationToken = default
        ) =>
            GetTransactionTypeByIdAsync(
                context,
                2,
                cancellationToken
            );
        protected static Task<TransactionType> GetPayInTransactionTypeAsync(
            Context context,
            CancellationToken cancellationToken = default
        ) =>
            GetTransactionTypeByIdAsync(
                context,
                3,
                cancellationToken
            );

        protected static Task<TransactionType> GetPayOutTransactionTypeAsync(
            Context context,
            CancellationToken cancellationToken = default
        ) =>
            GetTransactionTypeByIdAsync(
                context,
                4,
                cancellationToken
            );

        protected readonly bool _disposeMembers;
        protected readonly Context _context;
        protected readonly ILogger _logger;

        protected bool Disposed { get; private set; }

        private protected Business(
            Context context,
            ILogger logger,
            bool disposeMembers = true
        )
        {
            _disposeMembers = disposeMembers;

            _context = context ??
                throw new ArgumentNullException(nameof(context));
            _logger = logger ??
                throw new ArgumentNullException(nameof(logger));

            Disposed = false;
        }

        protected Task<User> GetUserByIdAsync(
            int id,
            CancellationToken cancellationToken = default
        ) =>
            _context.Users
                .Where(t => t.Id == id)
                .SingleAsync(cancellationToken);

        protected Task<User> GetUserByUsernameAsync(
            string username,
            CancellationToken cancellationToken = default
        ) =>
            _context.Users
                .Where(t => t.Username == username)
                .SingleAsync(cancellationToken);

        protected virtual void Dispose(
            bool disposing
        )
        {
            if (!Disposed && disposing)
            {
                if (_disposeMembers)
                {
                    _context.Dispose();
                }
            }

            Disposed = true;
        }

        protected virtual async ValueTask DisposeAsync(
            bool disposing
        )
        {
            if (!Disposed && disposing)
            {
                if (_disposeMembers)
                {
                    await _context
                        .DisposeAsync()
                        .ConfigureAwait(false);
                }
            }

            Disposed = true;
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

        ~Business()
        {
            Dispose(false);
        }
    }
}
