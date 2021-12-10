using System;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using Newtonsoft.Json;
using PDMF.Data.Azure.Abstract;
using PDMF.Data.Azure.Models;

namespace PDMF.Data.Azure.Base
{
    public class AzureQueueClient<T> : IAzureQueueClient<T> where T : QueueMessage
    {
        private readonly AzureQueueClientConfiguration<T> _configuration;
        
        private QueueClient _queue;
        
        public AzureQueueClient(AzureQueueClientConfiguration<T> configuration)
        {
            _configuration = configuration;
        }
        
        public async Task AddMessage(T message, TimeSpan? delayToStart = null, string correlationId = null)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            
            try
            {
                string content = SerializeMessage(message);
                QueueClient queue = await GetQueue();
                
                await queue.SendMessageAsync(content, delayToStart);
            }
            catch (Exception e)
            {
                throw;
            }
        }
        
        private string SerializeMessage(T message)
        {
            var jsonSettings = new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore};
            return JsonConvert.SerializeObject(message, jsonSettings);
        }

        private async Task<QueueClient> GetQueue()
        {
            if (_queue == null)
            {
                var options = new QueueClientOptions {MessageEncoding = QueueMessageEncoding.Base64};
                var client = new QueueServiceClient(_configuration.ConnectionString, options);

                _queue = client.GetQueueClient(_configuration.QueueName);
                await _queue.CreateIfNotExistsAsync();
            }

            return _queue;
        }
    }
}