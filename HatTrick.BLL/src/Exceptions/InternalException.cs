using System;
using System.Runtime.Serialization;

namespace HatTrick.BLL.Exceptions
{
    [Serializable]
    public sealed class InternalException : Exception
    {
        public InternalException() : base()
        {
        }

        public InternalException(
            string? message
        ) :
            base(message)
        {
        }

        public InternalException(
            string? message,
            Exception? innerException
        ) :
            base(message, innerException)
        {
        }

        private InternalException(
            SerializationInfo info,
            StreamingContext context
        ) :
            base(info, context)
        {
        }
    }
}
