using System;
using System.Runtime.Serialization;

namespace Demonology.Prime.Exceptions
{
    internal class FactoryRegistrationException : Exception
    {
        public FactoryRegistrationException()
        {
        }

        public FactoryRegistrationException(string message) : base(message)
        {
        }

        public FactoryRegistrationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected FactoryRegistrationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
