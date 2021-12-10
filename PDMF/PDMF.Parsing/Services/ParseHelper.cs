using System.Linq;

namespace PDMF.Parsing.Services
{
    public static class ParseHelper
    {
        public static string[] CreateDefaultHeaders(int length)
        {
            return Enumerable.Range(1, length).Select(i => $"Column_{i}").ToArray();
        }
        
        public static string ValidateDataSeparator(string dataSeparator)
        {
            switch (dataSeparator)
            {
                case @"\t" : return "\t";
                default: return dataSeparator;
            }
        }
    }
}