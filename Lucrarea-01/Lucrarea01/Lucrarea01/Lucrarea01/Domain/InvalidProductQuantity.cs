using System;
using System.Runtime.Serialization;

namespace Lucrarea01.Domain
{
    [Serializable]
    internal class InvalidProductQuantity : Exception
    {
        public InvalidProductQuantity()
        {
        }

        public InvalidProductQuantity(string message) : base(message)
        {
        }

        public InvalidProductQuantity(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidProductQuantity(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}