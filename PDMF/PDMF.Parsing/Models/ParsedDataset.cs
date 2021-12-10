using PDMF.Data.Algorithms.Matrices;

namespace PDMF.Parsing.Models
{
    public class ParsedDataset
    {
        public string[] Headers { get; set; }
        public Matrix Values { get; set; }
    }
}