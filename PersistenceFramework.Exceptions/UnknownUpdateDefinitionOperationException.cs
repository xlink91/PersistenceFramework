using System;

namespace PersistenceFramework.Exceptions
{
    public class UnknownUpdateDefinitionOperationException : Exception
    {
        public UnknownUpdateDefinitionOperationException(string operation) 
            : base($"Unknown update builder operation [{operation}].")
        {
        }
    }
}
