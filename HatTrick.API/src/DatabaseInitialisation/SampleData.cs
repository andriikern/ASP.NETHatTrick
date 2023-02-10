using HatTrick.Models;
using System;

namespace HatTrick.API.DatabaseInitialisation
{
    internal static class SampleData
    {
        public static TaxGrade[] TaxGrades { get; } =
            new TaxGrade[]
            {
                new TaxGrade()
                {
                    LowerBound = decimal.Zero,
                    UpperBound = 10_000.0M,
                    Rate = 0.10M
                },
                new TaxGrade()
                {
                    LowerBound = 10_000.0M,
                    UpperBound = 30_000.0M,
                    Rate = 0.15M
                },
                new TaxGrade()
                {
                    LowerBound = 30_000.0M,
                    UpperBound = null,
                    Rate = 0.30M
                }
            };

        public static Sport[] Sports { get; } =
            new Sport[]
            {
                new Sport()
                {
                    Name = "American football",
                    Priority = 1
                },
                new Sport()
                {
                    Name = "Basketball",
                    Priority = 1
                },
                new Sport()
                {
                    Name = "Boxing",
                    Priority = 2
                },
                new Sport()
                {
                    Name = "Cricket",
                    Priority = 3
                },
                new Sport()
                {
                    Name = "Cycling",
                    Priority = 2
                },
                new Sport()
                {
                    Name = "Football",
                    Priority = 0
                },
                new Sport()
                {
                    Name = "Formula 1",
                    Priority = 2
                },
                new Sport()
                {
                    Name = "Golf",
                    Priority = 2
                },
                new Sport()
                {
                    Name = "Hockey",
                    Priority = 2
                },
                new Sport()
                {
                    Name = "Horse riding",
                    Priority = 2
                },
                new Sport()
                {
                    Name = "MMA",
                    Priority = 2
                },
                new Sport()
                {
                    Name = "Rugby",
                    Priority = 3
                },
                new Sport()
                {
                    Name = "Tennis",
                    Priority = 1
                }
            };

        public static EventStatus[] EventStatuses { get; } =
            new EventStatus[]
            {
                new EventStatus()
                {
                    Name = "Active"
                },
                new EventStatus()
                {
                    Name = "Rescheduled"
                },
                new EventStatus()
                {
                    Name = "Cancelled"
                },
                new EventStatus()
                {
                    Name = "In play"
                },
                new EventStatus()
                {
                    Name = "Finished"
                }
            };

        public static FixtureType[] FixtureTypes { get; } =
            new FixtureType[]
            {
                new FixtureType()
                {
                    Name = "Prematch",
                    IsPromoted = false,
                    Priority = 3
                },
                new FixtureType()
                {
                    Name = "Live",
                    IsPromoted = false,
                    Priority = 2
                },
                new FixtureType()
                {
                    Name = "Promoted prematch",
                    IsPromoted = true,
                    Priority = 1
                },
                new FixtureType()
                {
                    Name = "Promoted live",
                    IsPromoted = true,
                    Priority = 0
                }
            };

        public static MarketType[] MarketTypes { get; } =
            new MarketType[]
            {
                new MarketType()
                {
                    Name = "Winner of match",
                    Priority = 0
                },
                new MarketType()
                {
                    Name = "Winner of half",
                    Priority = 1
                },
                new MarketType()
                {
                    Name = "Winner of third",
                    Priority = 1
                },
                new MarketType()
                {
                    Name = "Winner of quarter",
                    Priority = 2
                }
            };

        public static OutcomeType[] OutcomeTypes { get; } =
            new OutcomeType[]
            {
                new OutcomeType()
                {
                    Market = MarketTypes[0],
                    Name = "1",
                    Priority = 0
                },
                new OutcomeType()
                {
                    Market = MarketTypes[0],
                    Name = "X",
                    Priority = 1
                },
                new OutcomeType()
                {
                    Market = MarketTypes[0],
                    Name = "2",
                    Priority = 2
                },
                new OutcomeType()
                {
                    Market = MarketTypes[0],
                    Name = "1X",
                    Priority = 3
                },
                new OutcomeType()
                {
                    Market = MarketTypes[0],
                    Name = "X2",
                    Priority = 4
                },
                new OutcomeType()
                {
                    Market = MarketTypes[0],
                    Name = "12",
                    Priority = 5
                },
                new OutcomeType()
                {
                    Market = MarketTypes[1],
                    Name = "1",
                    Priority = 0
                },
                new OutcomeType()
                {
                    Market = MarketTypes[1],
                    Name = "X",
                    Priority = 1
                },
                new OutcomeType()
                {
                    Market = MarketTypes[1],
                    Name = "2",
                    Priority = 2
                },
                new OutcomeType()
                {
                    Market = MarketTypes[1],
                    Name = "1X",
                    Priority = 3
                },
                new OutcomeType()
                {
                    Market = MarketTypes[1],
                    Name = "X2",
                    Priority = 4
                },
                new OutcomeType()
                {
                    Market = MarketTypes[1],
                    Name = "12",
                    Priority = 5
                }
            };

        public static TicketStatus[] TicketStatuses { get; } =
            new TicketStatus[]
            {
                new TicketStatus()
                {
                    Name = "Active"
                },
                new TicketStatus()
                {
                    Name = "Rejected"
                },
                new TicketStatus()
                {
                    Name = "Cancelled"
                },
                new TicketStatus()
                {
                    Name = "Cashed out"
                },
                new TicketStatus()
                {
                    Name = "Won"
                },
                new TicketStatus()
                {
                    Name = "Lost"
                }
            };

        public static TransactionType[] TransactionTypes { get; } =
            new TransactionType[]
            {
                new TransactionType()
                {
                    Name = "Deposit"
                },
                new TransactionType()
                {
                    Name = "Withdrawal"
                },
                new TransactionType()
                {
                    Name = "Pay-in"
                },
                new TransactionType()
                {
                    Name = "Pay-out"
                }
            };

        public static User[] Users { get; } =
            new User[]
            {
                new User()
                {
                    Username = "hat-trick",
                    Name = "Leo",
                    Surname = "Jones",
                    Sex = "M",
                    Email = "leo.jones@hattrickbetting.com",
                    Address = "394 Fulham Road",
                    City = "London",
                    Country = "United Kingdom",
                    Phone = "+44 7700 986960",
                    Birthdate = new DateTime(2001, 1, 1),
                    RegisteredOn = new DateTime(2023, 1, 1),
                    Balance = 1000.0M
                }
            };

        public static Event[] Events { get; } =
            new Event[]
            {
                new Event()
                {
                    Name = "Ac.Viseu - FC Porto",
                    Sport = Sports[5],
                    StartsAt = new DateTime(2023, 2, 8, 21, 45, 0),
                    EndsAt = new DateTime(2023, 2, 8, 23, 45, 0),
                    Status = EventStatuses[0],
                    Priority = 0
                },
                new Event()
                {
                    Name = "Marseille - Paris SG",
                    Sport = Sports[5],
                    StartsAt = new DateTime(2023, 2, 8, 21, 10, 0),
                    EndsAt = new DateTime(2023, 2, 8, 23, 10, 0),
                    Status = EventStatuses[0],
                    Priority = 0
                },
                new Event()
                {
                    Name = "Wolf J.J. - Albot R.",
                    Sport = Sports[12],
                    StartsAt = new DateTime(2023, 2, 8, 21, 10, 0),
                    EndsAt = new DateTime(2023, 2, 8, 23, 10, 0),
                    Status = EventStatuses[0],
                    Priority = 0
                },
                new Event()
                {
                    Name = "Santos FC - Sao Bento",
                    Sport = Sports[5],
                    StartsAt = new DateTime(2023, 2, 9, 1, 35, 0),
                    EndsAt = new DateTime(2023, 2, 9, 3, 35, 0),
                    Status = EventStatuses[0],
                    Priority = 0
                },
                new Event()
                {
                    Name = "Toronto Raptors - San Antonio Spurs",
                    Sport = Sports[1],
                    StartsAt = new DateTime(2023, 2, 9, 1, 30, 0),
                    EndsAt = new DateTime(2023, 2, 9, 3, 30, 0),
                    Status = EventStatuses[0],
                    Priority = 0
                },
                new Event()
                {
                    Name = "LD Alajuelense - Pérez Zeledón",
                    Sport = Sports[5],
                    StartsAt = new DateTime(2023, 2, 9, 3, 0, 0),
                    EndsAt = new DateTime(2023, 2, 9, 5, 0, 0),
                    Status = EventStatuses[0],
                    Priority = 0
                },
                new Event()
                {
                    Name = "Cleveland Caval. - Detroit Pistons",
                    Sport = Sports[1],
                    StartsAt = new DateTime(2023, 2, 9, 1, 0, 0),
                    EndsAt = new DateTime(2023, 2, 9, 3, 0, 0),
                    Status = EventStatuses[0],
                    Priority = 0
                },
                new Event()
                {
                    Name = "N.Y.Rangers - Vancouver Canuc.",
                    Sport = Sports[8],
                    StartsAt = new DateTime(2023, 2, 9, 2, 0, 0),
                    EndsAt = new DateTime(2023, 2, 9, 4, 0, 0),
                    Status = EventStatuses[0],
                    Priority = 0
                },
                new Event()
                {
                    Name = "Boston Celtics - Philadelphia 76ers",
                    Sport = Sports[1],
                    StartsAt = new DateTime(2023, 2, 9, 1, 30, 0),
                    EndsAt = new DateTime(2023, 2, 9, 3, 30, 0),
                    Status = EventStatuses[0],
                    Priority = 0
                },
                new Event()
                {
                    Name = "L.A.Clippers - Dallas Mavericks",
                    Sport = Sports[1],
                    StartsAt = new DateTime(2023, 2, 9, 4, 0, 0),
                    EndsAt = new DateTime(2023, 2, 9, 6, 0, 0),
                    Status = EventStatuses[0],
                    Priority = 0
                },
                new Event()
                {
                    Name = "Baez S. - Darderi L.",
                    Sport = Sports[12],
                    StartsAt = new DateTime(2023, 2, 9, 23, 50, 0),
                    EndsAt = new DateTime(2023, 2, 10, 1, 50, 0),
                    Status = EventStatuses[0],
                    Priority = 0
                }
            };

        public static Fixture[] Fixtures { get; } =
            new Fixture[]
            {
                new Fixture()
                {
                    Event = Events[0],
                    Type = FixtureTypes[0],
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 8, 21, 43, 0)
                },
                new Fixture()
                {
                    Event = Events[1],
                    Type = FixtureTypes[0],
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 8, 21, 8, 0)
                },
                new Fixture()
                {
                    Event = Events[2],
                    Type = FixtureTypes[0],
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 8, 21, 8, 0)
                },
                new Fixture()
                {
                    Event = Events[3],
                    Type = FixtureTypes[0],
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 1, 33, 0)
                },
                new Fixture()
                {
                    Event = Events[4],
                    Type = FixtureTypes[0],
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 1, 28, 0)
                },
                new Fixture()
                {
                    Event = Events[5],
                    Type = FixtureTypes[0],
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 2, 58, 0)
                },
                new Fixture()
                {
                    Event = Events[6],
                    Type = FixtureTypes[0],
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 0, 58, 0)
                },
                new Fixture()
                {
                    Event = Events[7],
                    Type = FixtureTypes[0],
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 1, 58, 0)
                },
                new Fixture()
                {
                    Event = Events[8],
                    Type = FixtureTypes[0],
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 1, 28, 0)
                },
                new Fixture()
                {
                    Event = Events[9],
                    Type = FixtureTypes[0],
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 3, 58, 0)
                },
                new Fixture()
                {
                    Event = Events[10],
                    Type = FixtureTypes[0],
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 23, 48, 0)
                }
            };

        public static Market[] Markets { get; } =
            new Market[]
            {
                new Market()
                {
                    Fixture = Fixtures[0],
                    Type = MarketTypes[0],
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 8, 21, 43, 0)
                },
                new Market()
                {
                    Fixture = Fixtures[1],
                    Type = MarketTypes[0],
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 8, 21, 8, 0)
                },
                new Market()
                {
                    Fixture = Fixtures[2],
                    Type = MarketTypes[0],
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 8, 21, 8, 0)
                },
                new Market()
                {
                    Fixture = Fixtures[3],
                    Type = MarketTypes[0],
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 1, 33, 0)
                },
                new Market()
                {
                    Fixture = Fixtures[4],
                    Type = MarketTypes[0],
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 1, 28, 0)
                },
                new Market()
                {
                    Fixture = Fixtures[5],
                    Type = MarketTypes[0],
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 2, 58, 0)
                },
                new Market()
                {
                    Fixture = Fixtures[6],
                    Type = MarketTypes[0],
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 0, 58, 0)
                },
                new Market()
                {
                    Fixture = Fixtures[7],
                    Type = MarketTypes[0],
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 1, 58, 0)
                },
                new Market()
                {
                    Fixture = Fixtures[8],
                    Type = MarketTypes[0],
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 1, 28, 0)
                },
                new Market()
                {
                    Fixture = Fixtures[9],
                    Type = MarketTypes[0],
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 3, 58, 0)
                },
                new Market()
                {
                    Fixture = Fixtures[10],
                    Type = MarketTypes[0],
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 23, 48, 0)
                }
            };

        public static Outcome[] Outcomes { get; } =
            new Outcome[]
            {
                new Outcome()
                {
                    Market = Markets[0],
                    Type = OutcomeTypes[0],
                    Odds = 11.00M,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 8, 21, 43, 0)
                },
                new Outcome()
                {
                    Market = Markets[0],
                    Type = OutcomeTypes[1],
                    Odds = 5.50M,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 8, 21, 43, 0)
                },
                new Outcome()
                {
                    Market = Markets[0],
                    Type = OutcomeTypes[2],
                    Odds = 1.25M,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 8, 21, 43, 0)
                },
                new Outcome()
                {
                    Market = Markets[0],
                    Type = OutcomeTypes[3],
                    Odds = 3.60M,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 8, 21, 43, 0)
                },
                new Outcome()
                {
                    Market = Markets[0],
                    Type = OutcomeTypes[4],
                    Odds = 1.05M,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 8, 21, 43, 0)
                },
                new Outcome()
                {
                    Market = Markets[0],
                    Type = OutcomeTypes[5],
                    Odds = 1.10M,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 8, 21, 43, 0)
                },
                new Outcome()
                {
                    Market = Markets[1],
                    Type = OutcomeTypes[0],
                    Odds = 3.10M,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 8, 21, 8, 0)
                },
                new Outcome()
                {
                    Market = Markets[1],
                    Type = OutcomeTypes[1],
                    Odds = 3.60M,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 8, 21, 8, 0)
                },
                new Outcome()
                {
                    Market = Markets[1],
                    Type = OutcomeTypes[2],
                    Odds = 2.50M,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 8, 21, 8, 0)
                },
                new Outcome()
                {
                    Market = Markets[1],
                    Type = OutcomeTypes[3],
                    Odds = 1.65M,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 8, 21, 8, 0)
                },
                new Outcome()
                {
                    Market = Markets[1],
                    Type = OutcomeTypes[4],
                    Odds = 1.50M,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 8, 21, 8, 0)
                },
                new Outcome()
                {
                    Market = Markets[1],
                    Type = OutcomeTypes[5],
                    Odds = 1.40M,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 8, 21, 8, 0)
                },
                new Outcome()
                {
                    Market = Markets[2],
                    Type = OutcomeTypes[0],
                    Odds = 1.20M,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 8, 21, 8, 0)
                },
                new Outcome()
                {
                    Market = Markets[2],
                    Type = OutcomeTypes[1],
                    Odds = null,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 8, 21, 8, 0)
                },
                new Outcome()
                {
                    Market = Markets[2],
                    Type = OutcomeTypes[2],
                    Odds = 3.60M,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 8, 21, 8, 0)
                },
                new Outcome()
                {
                    Market = Markets[2],
                    Type = OutcomeTypes[3],
                    Odds = null,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 8, 21, 8, 0)
                },
                new Outcome()
                {
                    Market = Markets[2],
                    Type = OutcomeTypes[4],
                    Odds = null,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 8, 21, 8, 0)
                },
                new Outcome()
                {
                    Market = Markets[2],
                    Type = OutcomeTypes[5],
                    Odds = null,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 8, 21, 8, 0)
                },
                new Outcome()
                {
                    Market = Markets[3],
                    Type = OutcomeTypes[0],
                    Odds = 1.50M,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 1, 33, 0)
                },
                new Outcome()
                {
                    Market = Markets[3],
                    Type = OutcomeTypes[1],
                    Odds = 3.50M,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 1, 33, 0)
                },
                new Outcome()
                {
                    Market = Markets[3],
                    Type = OutcomeTypes[2],
                    Odds = 6.30M,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 1, 33, 0)
                },
                new Outcome()
                {
                    Market = Markets[3],
                    Type = OutcomeTypes[3],
                    Odds = 1.05M,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 1, 33, 0)
                },
                new Outcome()
                {
                    Market = Markets[3],
                    Type = OutcomeTypes[4],
                    Odds = 2.30M,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 1, 33, 0)
                },
                new Outcome()
                {
                    Market = Markets[3],
                    Type = OutcomeTypes[5],
                    Odds = 1.20M,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 1, 33, 0)
                },
                new Outcome()
                {
                    Market = Markets[4],
                    Type = OutcomeTypes[0],
                    Odds = 1.20M,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 1, 28, 0)
                },
                new Outcome()
                {
                    Market = Markets[4],
                    Type = OutcomeTypes[1],
                    Odds = 17.00M,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 1, 28, 0)
                },
                new Outcome()
                {
                    Market = Markets[4],
                    Type = OutcomeTypes[2],
                    Odds = 5.50M,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 1, 28, 0)
                },
                new Outcome()
                {
                    Market = Markets[4],
                    Type = OutcomeTypes[3],
                    Odds = 1.10M,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 1, 28, 0)
                },
                new Outcome()
                {
                    Market = Markets[4],
                    Type = OutcomeTypes[4],
                    Odds = 4.15M,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 1, 28, 0)
                },
                new Outcome()
                {
                    Market = Markets[4],
                    Type = OutcomeTypes[5],
                    Odds = null,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 1, 28, 0)
                },
                new Outcome()
                {
                    Market = Markets[5],
                    Type = OutcomeTypes[0],
                    Odds = 1.35M,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 2, 58, 0)
                },
                new Outcome()
                {
                    Market = Markets[5],
                    Type = OutcomeTypes[1],
                    Odds = 4.30M,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 2, 58, 0)
                },
                new Outcome()
                {
                    Market = Markets[5],
                    Type = OutcomeTypes[2],
                    Odds = 7.30M,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 2, 58, 0)
                },
                new Outcome()
                {
                    Market = Markets[5],
                    Type = OutcomeTypes[3],
                    Odds = 1.03M,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 2, 58, 0)
                },
                new Outcome()
                {
                    Market = Markets[5],
                    Type = OutcomeTypes[4],
                    Odds = 2.70M,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 2, 58, 0)
                },
                new Outcome()
                {
                    Market = Markets[5],
                    Type = OutcomeTypes[5],
                    Odds = 1.15M,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 2, 58, 0)
                },
                new Outcome()
                {
                    Market = Markets[6],
                    Type = OutcomeTypes[0],
                    Odds = 1.15M,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 0, 58, 0)
                },
                new Outcome()
                {
                    Market = Markets[6],
                    Type = OutcomeTypes[1],
                    Odds = 18.00M,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 0, 58, 0)
                },
                new Outcome()
                {
                    Market = Markets[6],
                    Type = OutcomeTypes[2],
                    Odds = 6.70M,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 0, 58, 0)
                },
                new Outcome()
                {
                    Market = Markets[6],
                    Type = OutcomeTypes[3],
                    Odds = 1.10M,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 0, 58, 0)
                },
                new Outcome()
                {
                    Market = Markets[6],
                    Type = OutcomeTypes[4],
                    Odds = 4.90M,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 0, 58, 0)
                },
                new Outcome()
                {
                    Market = Markets[6],
                    Type = OutcomeTypes[5],
                    Odds = null,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 0, 58, 0)
                },
                new Outcome()
                {
                    Market = Markets[7],
                    Type = OutcomeTypes[0],
                    Odds = 1.70M,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 1, 58, 0)
                },
                new Outcome()
                {
                    Market = Markets[7],
                    Type = OutcomeTypes[1],
                    Odds = 4.40M,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 1, 58, 0)
                },
                new Outcome()
                {
                    Market = Markets[7],
                    Type = OutcomeTypes[2],
                    Odds = 3.70M,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 1, 58, 0)
                },
                new Outcome()
                {
                    Market = Markets[7],
                    Type = OutcomeTypes[3],
                    Odds = 1.20M,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 1, 58, 0)
                },
                new Outcome()
                {
                    Market = Markets[7],
                    Type = OutcomeTypes[4],
                    Odds = 2.00M,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 1, 58, 0)
                },
                new Outcome()
                {
                    Market = Markets[7],
                    Type = OutcomeTypes[5],
                    Odds = 1.15M,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 1, 58, 0)
                },
                new Outcome()
                {
                    Market = Markets[8],
                    Type = OutcomeTypes[0],
                    Odds = 1.60M,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 1, 28, 0)
                },
                new Outcome()
                {
                    Market = Markets[8],
                    Type = OutcomeTypes[1],
                    Odds = 14.00M,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 1, 28, 0)
                },
                new Outcome()
                {
                    Market = Markets[8],
                    Type = OutcomeTypes[2],
                    Odds = 2.60M,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 1, 28, 0)
                },
                new Outcome()
                {
                    Market = Markets[8],
                    Type = OutcomeTypes[3],
                    Odds = 1.45M,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 1, 28, 0)
                },
                new Outcome()
                {
                    Market = Markets[8],
                    Type = OutcomeTypes[4],
                    Odds = 2.20M,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 1, 28, 0)
                },
                new Outcome()
                {
                    Market = Markets[8],
                    Type = OutcomeTypes[5],
                    Odds = null,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 1, 28, 0)
                },
                new Outcome()
                {
                    Market = Markets[9],
                    Type = OutcomeTypes[0],
                    Odds = 1.35M,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 3, 58, 0)
                },
                new Outcome()
                {
                    Market = Markets[9],
                    Type = OutcomeTypes[1],
                    Odds = 16.00M,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 3, 58, 0)
                },
                new Outcome()
                {
                    Market = Markets[9],
                    Type = OutcomeTypes[2],
                    Odds = 3.70M,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 3, 58, 0)
                },
                new Outcome()
                {
                    Market = Markets[9],
                    Type = OutcomeTypes[3],
                    Odds = 1.25M,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 3, 58, 0)
                },
                new Outcome()
                {
                    Market = Markets[9],
                    Type = OutcomeTypes[4],
                    Odds = 3.00M,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 3, 58, 0)
                },
                new Outcome()
                {
                    Market = Markets[9],
                    Type = OutcomeTypes[5],
                    Odds = null,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 3, 58, 0)
                },
                new Outcome()
                {
                    Market = Markets[10],
                    Type = OutcomeTypes[0],
                    Odds = 1.50M,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 23, 48, 0)
                },
                new Outcome()
                {
                    Market = Markets[10],
                    Type = OutcomeTypes[1],
                    Odds = null,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 23, 48, 0)
                },
                new Outcome()
                {
                    Market = Markets[10],
                    Type = OutcomeTypes[2],
                    Odds = 2.30M,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 23, 48, 0)
                },
                new Outcome()
                {
                    Market = Markets[10],
                    Type = OutcomeTypes[3],
                    Odds = null,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 23, 48, 0)
                },
                new Outcome()
                {
                    Market = Markets[10],
                    Type = OutcomeTypes[4],
                    Odds = null,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 23, 48, 0)
                },
                new Outcome()
                {
                    Market = Markets[10],
                    Type = OutcomeTypes[5],
                    Odds = null,
                    AvailableFrom = new DateTime(2023, 1, 1),
                    AvailableUntil = new DateTime(2023, 2, 9, 23, 48, 0)
                }
            };
    }
}
