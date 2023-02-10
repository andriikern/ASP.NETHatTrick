using System;

namespace HatTrick.BLL.Exceptions
{
    [Flags]
    public enum InternalExceptionReason
    {
        None = 0,

        ServerError = 1,

        BadInput = 2,
        NotFound = 4,

        All = -1
    }
}
