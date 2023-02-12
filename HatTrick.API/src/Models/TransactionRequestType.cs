using System;

namespace HatTrick.API.Models
{
    public enum TransactionRequestType
    {
        Unspecified = 0,
        Withdrawal = -1,
        Deposit = 1
    }
}
