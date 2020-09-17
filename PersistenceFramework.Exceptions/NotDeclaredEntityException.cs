using System;

namespace PersistenceFramework.Exceptions
{
    public class NotDeclaredEntityException : Exception
    {
        public NotDeclaredEntityException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
