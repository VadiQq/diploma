using Microsoft.AspNetCore.Http;
using PDMF.Data.Enums;

namespace PDMF.WebApi.Controllers.Forecasting.Models
{
    public class CreateForecastRequest
    {
        public string ModelingTaskId { get; set; }
        public ForecastMode Mode { get; set; }
        public IFormFile ForecastDataset { get; set; }
        public ParseMode ParseMode { get; set; }
        public string DataSeparator { get; set; }
    }
}