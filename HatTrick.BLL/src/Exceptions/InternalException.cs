using System;
using System.Runtime.Serialization;

namespace HatTrick.BLL.Exceptions
{
    [Serializable]
    public sealed class InternalException : Exception, ISerializable
    {
        private readonly InternalExceptionReason _reason;

        public InternalExceptionReason Reason =>
            _reason;

        public InternalException() :
            base()
        {
            _reason = InternalExceptionReason.None;
        }

        public InternalException(
            InternalExceptionReason reason
        ) :
            base()
        {
            _reason = reason;
        }

        public InternalException(
            InternalExceptionReason reason,
            string? message
        ) :
            base(message)
        {
            _reason = reason;
        }

        public InternalException(
            InternalExceptionReason reason,
            string? message,
            Exception? innerException
        ) :
            base(message, innerException)
        {
            _reason = reason;
        }

        private InternalException(
            SerializationInfo info,
            StreamingContext context
        ) :
            base(info, context)
        {
            _reason = (InternalExceptionReason)info.GetInt32(nameof(Reason));
        }

        public override void GetObjectData(
            SerializationInfo info,
            StreamingContext context
        )
        {
            base.GetObjectData(info, context);

            info.AddValue(nameof(Reason), (int)_reason);
        }
    }
}
