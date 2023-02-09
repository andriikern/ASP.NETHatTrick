using HatTrick.DAL;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace HatTrick.BLL
{
    public sealed class Account : IDisposable, IAsyncDisposable
    {
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
