using PDMF.Data.Enums;

namespace PDMF.Data.Azure.Models.QueueMessages
{
    public class ParseQueueMessage : QueueMessage
    {
        public ParseMode Mode { get; set; }
        public string TaskId { get; set; }
        public string DatasetId { get; set; }
        public string DataSeparator { get; set; }
    }
}