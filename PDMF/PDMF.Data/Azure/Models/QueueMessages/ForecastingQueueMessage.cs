using PDMF.Data.Enums;

namespace PDMF.Data.Azure.Models.QueueMessages
{
    public class ForecastingQueueMessage : QueueMessage
    {
        public string ModelingResultId { get; set; }
        public string ForecastTaskId { get; set; }
        public ForecastMode Mode { get; set; }
    }
}