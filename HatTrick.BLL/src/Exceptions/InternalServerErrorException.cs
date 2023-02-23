using System;
using System.Runtime.Serialization;

namespace HatTrick.BLL.Exceptions
{
    [Serializable]
    public sealed class InternalServerErrorException : InternalException
    {
        public InternalServerErrorException() :
            base()
        {
        }

        public InternalServerErrorException(
            string? message
        ) :
            base(message)
        {
        }

        public InternalServerErrorException(
            string? message,
            Exception? innerException
        ) :
            base(message, innerException)
        {
        }

        private InternalServerErrorException(
            SerializationInfo info,
            StreamingContext context
        ) :
            base(info, context)
        {
        }
    }
}
