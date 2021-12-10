using PDMF.Data.Enums;

namespace PDMF.Parsing.Models
{
    public class ParseOptions
    {
        public string DataSeparator { get; set; }
        public ParseMode Mode { get; set; }

        public static ParseOptions CreateDefault()
        {
            return new ParseOptions
            {
                DataSeparator = " ",
                Mode = ParseMode.Default
            };
        }
    }
}