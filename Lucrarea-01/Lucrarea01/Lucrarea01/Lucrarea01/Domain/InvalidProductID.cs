using System;
using System.Runtime.Serialization;

namespace Lucrarea01.Domain
{
    [Serializable]
    internal class InvalidProductID : Exception
    {
        public InvalidProductID()
        {
        }

        public InvalidProductID(string message) : base(message)
        {
        }

        public InvalidProductID(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidProductID(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}