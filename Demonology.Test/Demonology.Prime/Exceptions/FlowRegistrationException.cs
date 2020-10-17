using System;
using System.Runtime.Serialization;

namespace Demonology.Prime.Exceptions
{
    internal class FlowRegistrationException : Exception
    {
        public FlowRegistrationException()
        {
        }

        public FlowRegistrationException(string message) : base(message)
        {
        }

        public FlowRegistrationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected FlowRegistrationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
