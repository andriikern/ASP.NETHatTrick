using HatTrick.BLL.Exceptions;

namespace HatTrick.API.Models
{
    public static class TransactionRequestTypeExtensions
    {
        public static bool? IsDebosit(
            this TransactionRequestType type,
            bool allowUnspecified
        ) =>
            type switch
            {
                TransactionRequestType.Unspecified =>
                    allowUnspecified ?
                        null :
                        throw new InternalBadInputException(
                            "Unspecified transaction type is not allowed."
                        ),
                TransactionRequestType.Withdrawal => false,
                TransactionRequestType.Deposit => true,
                _ => throw new InternalBadInputException(
                    $"Unrecognised transaction type specifier {type}."
                )
            };

        public static bool IsDebosit(
            this TransactionRequestType type
        ) =>
            (bool)IsDebosit(type, false)!;
    }
}
