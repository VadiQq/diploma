using PDMF.Data.Entities;

namespace PDMF.Functions.Forecasting.Models
{
    public class ForecastingData
    {
        public ForecastTask ForecastTask { get; set; }
        public ModelingResult ModelingResult { get; set; }
        public ModelingTask ModelingTask { get; set; }
        public Dataset Dataset { get; set; }
    }
}