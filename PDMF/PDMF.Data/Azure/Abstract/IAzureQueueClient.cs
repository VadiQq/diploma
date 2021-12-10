using System;
using System.Threading.Tasks;
using PDMF.Data.Azure.Models;

namespace PDMF.Data.Azure.Abstract
{
    public interface IAzureQueueClient<T> where T : QueueMessage
    {
        Task AddMessage(T message, TimeSpan? delayToStart = null, string correlationId = null);
    }
}