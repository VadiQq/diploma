using System;

namespace PDMF.WebApi.Models.Identity.Exceptions
{
    public class InvalidJWTException : Exception
    {
        public string Token { get; set; }

        public Exception InitialException { get; set; }
        
        public InvalidJWTException(string token, Exception exception)
        {
            Token = token;
            InitialException = exception;
        }
    }
}