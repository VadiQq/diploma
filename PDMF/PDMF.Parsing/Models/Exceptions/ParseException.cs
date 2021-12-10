using System;

namespace PDMF.Parsing.Models.Exceptions
{
    public class ParseException : Exception
    {
        public string DatasetId { get; set; }
        public Exception InitialException { get; set; }
        public Type InitialExceptionType { get; set; }
    }
}