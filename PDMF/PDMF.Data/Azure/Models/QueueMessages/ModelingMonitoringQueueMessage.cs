namespace PDMF.Data.Azure.Models.QueueMessages
{
    public class ModelingMonitoringQueueMessage : QueueMessage
    {
        public string ModelingTaskId { get; set; }
        public string DatasetId { get; set; }
    }
}