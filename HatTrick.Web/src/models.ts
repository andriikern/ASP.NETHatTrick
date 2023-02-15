class TaxGrade {
  id: number = 0;
  lowerBound: number | null = null;
  upperBound: number | null = null;
  rate: number = 0
}

class Sport {
  id: number = 0;
  name: string = '';
  priority: number = 0
}

class EventStatus {
  id: number = 0;
  name: string = ''
}

class FixtureType {
  id: number = 0;
  name: string = '';
  isPromoted: boolean = false;
  priority: number = 0
}

class MarketType {
  id: number = 0;
  name: string = '';
  priority: number = 0
}

class OutcomeType {
  id: number = 0;
  name: string = '';
  priority: number = 0
}

class TicketStatus {
  id: number = 0;
  name: string = ''
}

class TransactionType {
  id: number = 0;
  name: string = ''
}

class User {
  id: number = 0;
  username: string = '';
  name: string = '';
  surname: string = '';
  sex: string | null = null;
  email: string = '';
  address: string = '';
  city: string = '';
  country: string = '';
  phone: string = '';
  birthdate: Date | string = '';
  deactivatedOn: Date | string | null = null;
  balance: number = 0;
  tickets: Ticket[] = [];
  transactions: Transaction[] = []
}

class Event_ {
  id: number = 0;
  name: string = '';
  sport: Sport = new Sport();
  startsAt: Date | string = '';
  endsAt: Date | string = '';
  status: EventStatus = new EventStatus();
  priority: number = 0;
  fixtures: Fixture[] = []
}

class Fixture {
  id: number = 0;
  type: FixtureType = new FixtureType();
  markets: Market[] = [];
  availableFrom: Date | string = '';
  availableUntil: Date | string = ''
}

class Market {
  id: number = 0;
  type: MarketType = new MarketType();
  value: string | null = null;
  availableFrom: Date | string = '';
  availableUntil: Date | string = '';
  outcomes: Outcome[] = []
}

class Outcome {
  id: number = 0;
  type: OutcomeType = new OutcomeType();
  value: string | null = null;
  odds: number | null = null;
  availableFrom: Date | string = '';
  availableUntil: Date | string = '';
  isResolved: Boolean = false;
  isWinning: boolean | null = null
}

class Ticket {
  id: number = 0;
  selections: Outcome[] = [];
  payInAmount: number = 0;
  payInTime: Date | string = '';
  totalOdds: number = 0;
  status: TicketStatus = new TicketStatus();
  isResolved: boolean = false;
  resolvedTime: Date | string | null = null;
  costAmount: number | null = null;
  winAmount: number | null = null;
  payOutTime: Date | string | null = null
}

class Transaction {
  id: number = 0;
  type: TransactionType = new TransactionType();
  ticket: Ticket | null = null;
  time: Date | string = '';
  amount: number = 0
}

class TicketFinancialAmounts {
  manipulativeCostRate: number = 0;
  payInAmount: number = 0;
  activeAmount: number = 0;
  totalOdds: number = 0;
  grossPotentialWinAmount: number = 0;
  tax: number = 0;
  netPotentialWinAmount: number = 0
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
