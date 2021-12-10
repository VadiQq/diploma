using PDMF.Data.Enums;

namespace PDMF.WebApi.Controllers.Datasets.Models
{
    public class ParseRequest
    {
        public string DatasetId { get; set; }
        public ParseMode Mode { get; set; }
        public string DataSeparator { get; set; }
    }
}