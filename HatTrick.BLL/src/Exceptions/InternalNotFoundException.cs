using System;
using System.Runtime.Serialization;

namespace HatTrick.BLL.Exceptions
{
    [Serializable]
    public sealed class InternalNotFoundException : InternalException
    {
        public InternalNotFoundException() :
            base()
        {
        }

        public InternalNotFoundException(
            string? message
        ) :
            base(message)
        {
        }

        public InternalNotFoundException(
            string? message,
            Exception? innerException
        ) :
            base(message, innerException)
        {
        }

        private InternalNotFoundException(
            SerializationInfo info,
            StreamingContext context
        ) :
            base(info, context)
        {
        }
    }
}
