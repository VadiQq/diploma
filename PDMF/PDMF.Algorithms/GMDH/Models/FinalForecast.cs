namespace PDMF.Algorithms.GMDH.Models
{
    public class FinalForecast
    {
        public double[] ForecastResults { get; set; }
        public double[] DatasetResults { get; set; }
        public double AverageMissForecast { get; set; }
    }
}