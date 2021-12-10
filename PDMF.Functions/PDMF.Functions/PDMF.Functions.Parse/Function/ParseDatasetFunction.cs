using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using PDMF.Data.Azure.Models.QueueMessages;

namespace PDMF.Functions.Parse.Function
{
    public partial class ParseDatasetFunction
    {
        private const string MessageQueueConnectionString = "AzureStorageConnection";
        
        [Function("ParseDatasetFunction")]
        public async Task ProcessParseDatasetMessage(
            [QueueTrigger("parse", Connection = MessageQueueConnectionString)]
            ParseQueueMessage message,
            FunctionContext executionContext)
        {
            await ProcessMessage(message);
        }
    }
}