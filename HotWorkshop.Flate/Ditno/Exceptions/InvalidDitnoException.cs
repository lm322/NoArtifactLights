using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotWorkshop.Flate.Ditno.Exceptions
{
    /// <summary>
    /// An exception thrown when the ditno usage is invalid.
    /// </summary>
    [Serializable]
    public class InvalidDitnoException : Exception
    {
        public InvalidDitnoException(string message) : base(message)
        {
        }

        public InvalidDitnoException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public InvalidDitnoException() : base(ExceptionTranslations.ExceptionDitnoInvalids)
        {
        }

        protected InvalidDitnoException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
        {
            throw new NotImplementedException();
        }
    }
}
