using System.IO;
using System.Threading.Tasks;
using PDMF.Parsing.Models;

namespace PDMF.Parsing.Services.Abstract
{
    public interface IDatasetParser
    {
        Task<ParsedDataset> Parse(Stream dataset, ParseOptions parseOptions = null);
    }
}