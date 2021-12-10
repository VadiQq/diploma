namespace PDMF.Data.Azure.Models
{
    public class AzureQueueClientConfiguration<T> where T : QueueMessage
    {
        public string QueueName { get; set; }
        public string ConnectionString { get; set; }
    }
}