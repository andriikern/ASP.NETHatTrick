using HatTrick.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;

namespace HatTrick.DAL
{
    public sealed class Context : DbContext
    {
        private readonly ILogger<Context> _logger;

        public DbSet<TaxGrade> TaxGrades { get; private set; }
        public DbSet<Sport> Sports { get; private set; }
        public DbSet<EventStatus> EventStatuses { get; private set; }
        public DbSet<FixtureType> FixtureTypes { get; private set; }
        public DbSet<MarketType> MarketTypes { get; private set; }
        public DbSet<OutcomeType> OutcomeTypes { get; private set; }
        public DbSet<TicketStatus> TicketStatuses { get; private set; }
        public DbSet<TransactionType> TransactionTypes { get; private set; }
        public DbSet<SportMarket> SportMarkets { get; private set; }
        public DbSet<User> Users { get; private set; }
        public DbSet<Event> Events { get; private set; }
        public DbSet<Fixture> Fixtures { get; private set; }
        public DbSet<Market> Markets { get; private set; }
        public DbSet<Outcome> Outcomes { get; private set; }
        public DbSet<Ticket> Tickets { get; private set; }
        public DbSet<TicketSelection> TicketSelections { get; private set; }
        public DbSet<Transaction> Transactions { get; private set; }

#nullable disable

        public Context(
            DbContextOptions options,
            ILogger<Context> logger
        ) :
            base(options)
        {
            _logger = logger ??
                throw new ArgumentNullException(nameof(logger));
        }

#nullable restore

        protected override void OnModelCreating(
            ModelBuilder modelBuilder
        )
        {
            if (modelBuilder is null)
            {
                throw new ArgumentNullException(nameof(modelBuilder));
            }

            _logger.LogDebug("Creating database model...");

            //modelBuilder.Entity<Sport>()
            //    .HasIndex(s => s.Name)
            //    .IsUnique();

            //modelBuilder.Entity<EventStatus>()
            //    .HasIndex(e => e.Name)
            //    .IsUnique();

            //modelBuilder.Entity<FixtureType>()
            //    .HasIndex(f => f.Name)
            //    .IsUnique();

            //modelBuilder.Entity<MarketType>()
            //    .HasIndex(f => f.Name)
            //    .IsUnique();

            //modelBuilder.Entity<OutcomeType>()
            //    .HasIndex("MarketId", nameof(OutcomeType.Name))
            //    .IsUnique();

            //modelBuilder.Entity<TicketStatus>()
            //    .HasIndex(t => t.Name)
            //    .IsUnique();

            //modelBuilder.Entity<TransactionType>()
            //    .HasIndex(t => t.Name)
            //    .IsUnique();

            modelBuilder.Entity<SportMarket>()
                .HasKey("SportId", "MarketId");
            modelBuilder.Entity<SportMarket>()
                .HasOne(sm => sm.Sport)
                .WithMany()
                .IsRequired();
            modelBuilder.Entity<SportMarket>()
                .HasOne(sm => sm.Market)
                .WithMany()
                .IsRequired();
            modelBuilder.Entity<Sport>()
                .HasMany(s => s.Markets)
                .WithMany()
                .UsingEntity<SportMarket>();
            modelBuilder.Entity<MarketType>()
                .HasMany(m => m.Sports)
                .WithMany()
                .UsingEntity<SportMarket>();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();
            modelBuilder.Entity<User>()
                .HasAlternateKey(u => u.Username);

            modelBuilder.Entity<TicketSelection>()
                .HasKey("TicketId", "SelectionId");
            modelBuilder.Entity<TicketSelection>()
                .HasOne(ts => ts.Ticket)
                .WithMany()
                .IsRequired();
            modelBuilder.Entity<TicketSelection>()
                .HasOne(ts => ts.Selection)
                .WithMany()
                .IsRequired();
            modelBuilder.Entity<Ticket>()
                .HasMany(t => t.Selections)
                .WithMany()
                .UsingEntity<TicketSelection>();
            modelBuilder.Entity<Outcome>()
                .HasMany(s => s.Tickets)
                .WithMany()
                .UsingEntity<TicketSelection>();

            _logger.LogInformation("Database model created successfully.");

            base.OnModelCreating(modelBuilder);
        }
    }
}
