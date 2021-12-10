using System;

namespace PDMF.Data.Exceptions
{
    public class ConfigurationException : Exception
    {
        public Exception InitialException { get; set; }
        public Type? InitialExceptionType { get; set; }

        public ConfigurationException(string message, Exception exception = null) : base(message)
        {
            InitialException = exception;
            InitialExceptionType = exception?.GetType();

        }
    }
}