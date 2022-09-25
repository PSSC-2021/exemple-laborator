using System;
using System.Runtime.Serialization;

namespace Exemple.Domain.Models
{
    [Serializable]
    internal class InvalidGradeException : Exception
    {
        public InvalidGradeException()
        {
        }

        public InvalidGradeException(string? message) : base(message)
        {
        }

        public InvalidGradeException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected InvalidGradeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}