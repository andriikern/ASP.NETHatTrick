using System;
using System.Runtime.Serialization;

namespace HatTrick.BLL.Exceptions
{
    [Serializable]
    public sealed class InternalBadInputException : InternalException
    {
        public InternalBadInputException() :
            base()
        {
        }

        public InternalBadInputException(
            string? message
        ) :
            base(message)
        {
        }

        public InternalBadInputException(
            string? message,
            Exception? innerException
        ) :
            base(message, innerException)
        {
        }

        private InternalBadInputException(
            SerializationInfo info,
            StreamingContext context
        ) :
            base(info, context)
        {
        }
    }
}
