class TaxGrade {
  id: Number = 0;
  lowerBound: Number | null = null;
  upperBound: Number | null = null;
  rate: Number = 0
}

class Sport {
  id: Number = 0;
  name: String = '';
  priority: Number = 0
}

class EventStatus {
  id: Number = 0;
  name: String = ''
}

class FixtureType {
  id: Number = 0;
  name: String = '';
  isPromoted: Boolean = false;
  priority: Number = 0
}

class MarketType {
  id: Number = 0;
  name: String = '';
  priority: Number = 0
}

class OutcomeType {
  id: Number = 0;
  name: String = '';
  priority: Number = 0
}

class TicketStatus {
  id: Number = 0;
  name: String = ''
}

class TransactionType {
  id: Number = 0;
  name: String = ''
}

class User {
  id: Number = 0;
  username: String = '';
  name: String = '';
  surname: String = '';
  sex: String | null = null;
  email: String = '';
  address: String = '';
  city: String = '';
  country: String = '';
  phone: String = '';
  birthdate: Date | String = '';
  deactivatedOn: Date | String | null = null;
  balance: Number = 0;
  tickets: Ticket[] = new Array<Ticket>();
  transactions: Transaction[] = new Array<Transaction>()
}

class Event_ {
  id: Number = 0;
  name: String = '';
  sport: Sport = new Sport();
  startsAt: Date | String = '';
  endsAt: Date | String = '';
  status: EventStatus = new EventStatus();
  priority: Number = 0;
  fixtures: Fixture[] = new Array<Fixture>()
}

class Fixture {
  id: Number = 0;
  type: FixtureType = new FixtureType();
  markets: Market[] = new Array<Market>();
  availableFrom: Date | String = '';
  availableUntil: Date | String = ''
}

class Market {
  id: Number = 0;
  type: MarketType = new MarketType();
  value: String | null = null;
  availableFrom: Date | String = '';
  availableUntil: Date | String = '';
  outcomes: Outcome[] = new Array<Outcome>()
}

class Outcome {
  id: Number = 0;
  type: OutcomeType = new OutcomeType();
  value: String | null = null;
  odds: Number | null = null;
  availableFrom: Date | String = '';
  availableUntil: Date | String = '';
  isWinning: Boolean | null = null
}

class Ticket {
  id: Number = 0;
  selections: Outcome[] = new Array<Outcome>();
  payInAmount: Number = 0;
  payInTime: Date | String = '';
  totalOdds: Number = 0;
  status: TicketStatus = new TicketStatus();
  isResolved: Boolean = false;
  resolvedTime: Date | String | null = null;
  costAmount: Number | null = null;
  winAmount: Number | null = null;
  payOutTime: Date | String | null = null
}

class Transaction {
  id: Number = 0;
  type: TransactionType = new TransactionType();
  ticket: Ticket | null = null;
  time: Date | String = '';
  amount: Number = 0
}

class TicketFinancialAmounts {
  manipulativeCostRate: Number = 0;
  payInAmount: Number = 0;
  activeAmount: Number = 0;
  totalOdds: Number = 0;
  grossPotentialWinAmount: Number = 0;
  tax: Number = 0;
  netPotentialWinAmount: Number = 0
}

// Export declared types.

export {
  TaxGrade,
  Sport,
  EventStatus,
  FixtureType,
  MarketType,
  OutcomeType,
  TicketStatus,
  TransactionType,
  User,
  Event_,
  Fixture,
  Market,
  Outcome,
  Ticket,
  Transaction,
  TicketFinancialAmounts
}
