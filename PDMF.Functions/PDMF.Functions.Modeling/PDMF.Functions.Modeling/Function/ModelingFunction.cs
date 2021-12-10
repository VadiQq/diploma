using System;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using PDMF.Data.Azure.Models.QueueMessages;
using Serilog;

namespace PDMF.Functions.Modeling.Function
{
    public partial class ModelingFunction
    {
        private const string MessageQueueConnectionString = "AzureStorageConnection";

        [Function("ModelingFunction")]
        public async Task ProcessModelingMessage(
            [QueueTrigger("modeling", Connection = MessageQueueConnectionString)]
            ModelingQueueMessage message,
            FunctionContext executionContext)
        {
            try
            {
                await ProcessMessage(message);
            }
            catch (Exception e)
            {
                Log.Error($"Error on batch setup: {e.Message}. Stack: {e.StackTrace}.");
            }
        }

        [Function("ModelingMonitoringFunction")]
        public async Task ProcessModelingMonitoringMessage(
            [QueueTrigger("modeling-monitoring", Connection = MessageQueueConnectionString)]
            ModelingMonitoringQueueMessage message,
            FunctionContext executionContext)
        {
            try
            {
                await ProcessMessage(message);
            }
            catch (Exception e)
            {
                Log.Error($"Error on batch setup: {e.Message}. Stack: {e.StackTrace}.");
            }
        }

        [Function("ModelingCleanupFunction")]
        public async Task ProcessModelingCleanupMessage(
            [QueueTrigger("modeling-cleanup", Connection = MessageQueueConnectionString)]
            ModelingCleanupQueueMessage message,
            FunctionContext executionContext)
        {
            try
            {
                await ProcessMessage(message);
            }
            catch (Exception e)
            {
                Log.Error($"Error on batch setup: {e.Message}. Stack: {e.StackTrace}.");
            }
        }
    }
}