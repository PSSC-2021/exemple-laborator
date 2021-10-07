using System;
using System.Runtime.Serialization;

namespace Lucrarea01.Domain
{
    [Serializable]
    internal class InvalidProdusID : Exception
    {
        public InvalidProdusID()
        {
        }

        public InvalidProdusID(string message) : base(message)
        {
        }

        public InvalidProdusID(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidProdusID(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}