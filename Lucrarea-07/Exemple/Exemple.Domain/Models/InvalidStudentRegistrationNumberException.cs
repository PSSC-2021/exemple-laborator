using System;
using System.Runtime.Serialization;

namespace Exemple.Domain.Models
{
    [Serializable]
    internal class InvalidStudentRegistrationNumberException : Exception
    {
        public InvalidStudentRegistrationNumberException()
        {
        }

        public InvalidStudentRegistrationNumberException(string? message) : base(message)
        {
        }

        public InvalidStudentRegistrationNumberException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected InvalidStudentRegistrationNumberException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}