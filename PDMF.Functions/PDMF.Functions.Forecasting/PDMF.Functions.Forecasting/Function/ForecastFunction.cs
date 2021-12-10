using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using PDMF.Data.Azure.Models.QueueMessages;

namespace PDMF.Functions.Forecasting.Function
{
    public partial class ForecastFunction
    {
        private const string MessageQueueConnectionString = "AzureStorageConnection";
        
        [Function("ForecastingFunction")]
        public async Task ProcessForecastingMessage(
            [QueueTrigger("forecasting", Connection = MessageQueueConnectionString)]
            ForecastingQueueMessage message,
            FunctionContext executionContext)
        {
            await ProcessMessage(message);
        }
    }
}