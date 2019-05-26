using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersistenceFramework.Exceptions
{
    public class NotDeclaredEntityException : Exception
    {
        public NotDeclaredEntityException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
