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
                        throw new InternalException(
                            InternalExceptionReason.BadInput,
                            "Unspecified transaction type is not allowed."
                        ),
                TransactionRequestType.Withdrawal => false,
                TransactionRequestType.Deposit => true,
                _ => throw new InternalException(
                    InternalExceptionReason.BadInput,
                    $"Unrecognised transaction type specifier {type}."
                )
            };

        public static bool IsDebosit(
            this TransactionRequestType type
        ) =>
            (bool)IsDebosit(type, false)!;
    }
}
