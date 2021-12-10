using PDMF.Data.Enums;

namespace PDMF.Data.Azure.Models.QueueMessages
{
    public class ModelingQueueMessage : QueueMessage
    {
        public string ModelingTaskId { get; set; }
        public string DatasetId { get; set; }
        public ModelType ModelType { get; set; }
        public int DesiredColumn { get; set; }
    }
}