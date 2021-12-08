using System;
using System.Runtime.Serialization;

namespace Exemple.Domain.Models
{
    [Serializable]
    internal class InvalidCodCartException : Exception
    {
        public InvalidCodCartException()
        {
        }

        public InvalidCodCartException(string? message) : base(message)
        {
        }

        public InvalidCodCartException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected InvalidCodCartException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}